using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace gameracers.Movement
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] float speed = .3f;

        float gravity = -9.81f;
        [SerializeField] float grassFriction = .85f;
        [SerializeField] float grassBounce = .6f;
        [SerializeField] float stoneFriction = .90f;
        [SerializeField] float stoneBounce = .8f;
        [SerializeField] float ceramicFriction = .95f;
        [SerializeField] float ceramicBounce = .8f;
        [SerializeField] float allBounceStrength = .8f;
        [SerializeField] float rollMult = 1.05f;

        [SerializeField] float bounceMin = .5f;

        [SerializeField] Grounded contactType = Grounded.Air;
        bool hasCalculatedTouch = false;

        //float downVel = 0;
        float airTime;
        float lastBounceVelocity = 0f;
        float lastBounceYPos = Mathf.Infinity;

        Rigidbody rb;
        SphereCollider sc;

        Vector3 velocity;

        [SerializeField] LayerMask groundMask;

        List<Vector3> movePoints = new List<Vector3>();

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            sc = GetComponent<SphereCollider>();
        }

        private void FixedUpdate()
        {
            if (movePoints.Count > 0)
            {
                transform.position = movePoints[^1];
            }

            hasCalculatedTouch = false;
            movePoints.Clear();
            CalculateGravity();

            float remainingPower = CheckCollision(velocity.magnitude * Time.fixedDeltaTime);
            int loopProtection = 0;
            while (remainingPower > 0 && loopProtection < 10)
            {
                remainingPower = CheckCollision(remainingPower);
                loopProtection += 1;
            }
        }

        public void Strike(float power, Vector3 moveDir)
        {
            velocity = moveDir * speed * power;
        }

        private void CalculateGravity()
        {
            if (contactType == Grounded.Air)
            {
                velocity += Vector3.up * gravity * (Time.time - airTime);
                return;
            }
        }

        private float CheckCollision(float remainingPower)
        {
            Vector3 closestPoint, surfaceNormal;

            Vector3 referencePos = transform.position;
            if (movePoints.Count > 0)
            {
                referencePos = movePoints[^1];
            }

            if (remainingPower < .05f)
            {
                Collider[] rollCheckColl = Physics.OverlapSphere(referencePos, sc.radius + 0.1f, groundMask);
                rollCheckColl.Reverse();
                foreach (Collider coll in rollCheckColl)
                {
                    if (GameObject.ReferenceEquals(coll.gameObject, this.gameObject))
                        continue;

                    if (CheckSphereExtra(coll, sc, referencePos, out closestPoint, out surfaceNormal))
                    {
                        SetGroundType(coll.gameObject.layer);
                        return CalculateRoll(remainingPower, closestPoint, surfaceNormal);
                    }
                }

                return 0f;
            }

            RaycastHit[] hits = Physics.SphereCastAll(referencePos, sc.radius, (velocity + Vector3.down * .001f).normalized, remainingPower, groundMask);
            if (hits.Length == 0 || (hits.Length == 1 && GameObject.ReferenceEquals(hits[0].collider.gameObject, this.gameObject)))
                hits = Physics.SphereCastAll(referencePos, sc.radius - .1f, velocity.normalized, remainingPower, groundMask);

            float newPower = remainingPower;

            // check raycast out, test for correct hits, test for ground (right below player) 
            foreach (RaycastHit hit in hits)
            {
                if (GameObject.ReferenceEquals(this.gameObject, hit.collider.gameObject))
                    continue;

                if (CheckSphereExtra(hit.collider, sc, hit.point, out closestPoint, out surfaceNormal) || (hit.point == Vector3.zero && CheckSphereExtra(hit.collider, sc, referencePos, out closestPoint, out surfaceNormal)))
                {
                    // Test for if ball is already moving away from the surface
                    if (Vector3.Angle(velocity, surfaceNormal) < 90f)
                    {
                        continue;
                    }

                    // Test if the ball is moving towards or away from the surface, if it is closer, than that means the ball is going to collide with the collider
                    if ((closestPoint - referencePos).magnitude < (referencePos - (referencePos + velocity.normalized * .2f)).magnitude)
                    {
                        continue;
                    }

                    // Check collision layer to set contactType
                    SetGroundType(hit.collider.gameObject.layer);

                    // Add point of collision to MovePoints
                    Vector3 collidePointCenter = closestPoint + surfaceNormal * sc.radius;
                    newPower = remainingPower - (collidePointCenter - referencePos).magnitude;
                    if (newPower < 0)
                    {
                        // Runs out of energy as it hits the wall. 
                        // TODO i believe the glitchiness down hill and stuff occurs here, but if I remove it, it will teleport player to the Origin. 
                        Debug.Log("It limitted out");
                        Debug.Log("Hit Point: " + hit.point);

                        remainingPower = Friction(remainingPower);
                        Vector3 preWallStop = collidePointCenter - (referencePos - collidePointCenter).normalized * remainingPower;
                        movePoints.Add(TestPos(collidePointCenter));
                        return 0;
                    }
                    movePoints.Add(TestPos(collidePointCenter));
                    referencePos = movePoints[^1];

                    velocity = Vector3.Reflect(velocity, surfaceNormal.normalized);

                    if (Vector3.Angle(velocity.normalized, surfaceNormal.normalized) < 70f && (velocity.magnitude * Mathf.Cos(Vector3.Angle(surfaceNormal, velocity.normalized) * Mathf.PI / 180f) / velocity.magnitude) >= bounceMin)
                    {
                        // The upward velocity away from the bounce plain is large, meaning CalculateBounce
                        return CalculateBounce(remainingPower, collidePointCenter);
                    }
                    else
                    {
                        // The upward velocity away from the bounce plain is small, meaning calculate rolling. 
                        return CalculateRoll(remainingPower, closestPoint, surfaceNormal);
                    }
                }
            }

            if (contactType != Grounded.Air)
            {
                airTime = Time.time;
                contactType = Grounded.Air;
            }

            movePoints.Add(TestPos(referencePos + velocity.normalized * remainingPower));
            return 0f;
        }

        private float CalculateBounce(float remainingPower, Vector3 collidePointCenter)
        {
            // First calculate any change in velocity
            if (hasCalculatedTouch == false)
            {
                remainingPower = Friction(remainingPower);
                hasCalculatedTouch = true;
            }

            // Bounce strength limiter to prevent infinite bounces
            float tempVel = velocity.magnitude;

            if (lastBounceVelocity == 0) // First Bounce
            {
                tempVel = Mathf.Min(tempVel, velocity.magnitude);
                lastBounceYPos = collidePointCenter.y;
            }
            else // second and rest of bounces
            {
                // New Bounce Pos is lower than the old bounce pos
                {
                    lastBounceVelocity = tempVel;
                    lastBounceYPos = collidePointCenter.y;
                }

                // new bounce pos is higher than the old bounce pos, sets the next calculations to be higher and therefore more acurate. 
                if (collidePointCenter.y > lastBounceYPos)
                {
                    lastBounceYPos = collidePointCenter.y;
                }

                while (tempVel >= lastBounceVelocity)
                {
                    tempVel = velocity.magnitude * allBounceStrength;
                    velocity = velocity * allBounceStrength;
                }
            }

            if (lastBounceVelocity == 0)
                lastBounceVelocity = tempVel;
            lastBounceVelocity = Mathf.Min(tempVel, lastBounceVelocity);

            lastBounceYPos = collidePointCenter.y;

            return remainingPower;
        }

        private float CalculateRoll(float remainingPower, Vector3 closestPoint, Vector3 surfaceNormal)
        {
            float newPower = remainingPower;

            if (hasCalculatedTouch == false)
            {
                if (Vector3.Angle(Vector3.up, surfaceNormal) < 15f)
                {
                    newPower = Friction(newPower);
                }

                hasCalculatedTouch = true;
            }
            velocity = Vector3.ProjectOnPlane(velocity + Vector3.up * gravity * Time.fixedDeltaTime * rollMult, surfaceNormal);

            Vector3 roughFinalPos = closestPoint + velocity.normalized * remainingPower + surfaceNormal * sc.radius;

            movePoints.Add(TestPos(roughFinalPos));
            //Debug.Log("Vel: " + velocity + " vel mag: " + velocity.magnitude);

            // Test if velocity.magnitude is ridiculously low, if so, stop ze ball
            if (velocity.magnitude < .07f)
            {
                velocity = Vector3.zero;
                lastBounceYPos = 0;
                lastBounceYPos = Mathf.Infinity;
            }

            return 0f;
        }

        private Vector3 TestPos(Vector3 testPos)
        {
            /* Summary
             * Tests the testPos to ensure it is not colliding with any objects besides a small touch. 
             * 
             * returns Vector3 that is not inside another collider with radius sc.radius
             */

            Vector3 closestPoint, surfaceNormal;

            Collider[] colliders = Physics.OverlapSphere(testPos, sc.radius);
            foreach (Collider coll in colliders)
            {
                if (GameObject.ReferenceEquals(coll.gameObject, this.gameObject)) continue;

                if (CheckSphereExtra(coll, sc, testPos, out closestPoint, out surfaceNormal))
                {
                    return surfaceNormal.normalized * sc.radius + closestPoint;
                }
            }

            return testPos;
        }

        private void SetGroundType(int layerNum)
        {
            if (layerNum == LayerMask.NameToLayer("Grass"))
            {
                //Debug.Log("Grass Layer");
                airTime = Mathf.Infinity;
                contactType = Grounded.Grass;
            }
            else if (layerNum == LayerMask.GetMask("Stone"))
            {
                //Debug.Log("Stone Layer");
                airTime = Mathf.Infinity;
                contactType = Grounded.Stone;
            }
            else if (layerNum == LayerMask.GetMask("Ceramic"))
            {
                //Debug.Log("Ceramic Layer");
                airTime = Mathf.Infinity;
                contactType = Grounded.Ceramic;
            }
            else
            {
                //Debug.Log("Catch Layer");
                airTime = Mathf.Infinity;
                contactType = Grounded.Ceramic;
            }
        }
        
        private float Friction(float newPower)
        {
            switch (contactType)
            {
                case Grounded.Grass:
                    velocity *= grassFriction;
                    newPower *= grassFriction;
                    break;
                case Grounded.Stone:
                    velocity *= stoneFriction;
                    newPower *= stoneFriction;
                    break;
                case Grounded.Ceramic:
                    velocity *= ceramicFriction;
                    newPower *= ceramicFriction;
                    break;
            }

            return newPower;
        }

        public Vector3 GetVelocity()
        {
            return velocity;
        }

        public void ResetVelocity()
        {
            velocity = Vector3.zero;
        }

        public bool CheckSphereExtra(Collider target_collider, SphereCollider sphere_collider, Vector3 sphere_cast_hit_pos, out Vector3 closest_point, out Vector3 surface_normal)
        {
            closest_point = Vector3.zero;
            surface_normal = Vector3.zero;
            float surface_penetration_depth = 0f;

            // Take the hit pos and offset it away from the collider by half of the radius along the negative direction of the velocity. This is to prevent the sphere collider being inside of the target collider and by extension, causing issues
            Vector3 cast_pos = sphere_cast_hit_pos + (velocity.normalized * -sc.radius * .5f);
            if (Physics.ComputePenetration(target_collider, target_collider.transform.position, target_collider.transform.rotation, sphere_collider, cast_pos, Quaternion.identity, out surface_normal, out surface_penetration_depth))
            {
                closest_point = cast_pos + (surface_normal * (sphere_collider.radius - surface_penetration_depth));

                surface_normal = -surface_normal;
                return true;
            }

            // move cast_pos around and test again
            cast_pos = sphere_cast_hit_pos + Vector3.down * sc.radius * .5f;
            if (Physics.ComputePenetration(target_collider, target_collider.transform.position, target_collider.transform.rotation, sphere_collider, cast_pos, Quaternion.identity, out surface_normal, out surface_penetration_depth))
            {
                closest_point = cast_pos + (surface_normal * (sphere_collider.radius - surface_penetration_depth));

                surface_normal = -surface_normal;
                return true;
            }

            // move cast_pos around and test again
            cast_pos = sphere_cast_hit_pos + Vector3.up * sc.radius * .5f;
            if (Physics.ComputePenetration(target_collider, target_collider.transform.position, target_collider.transform.rotation, sphere_collider, cast_pos, Quaternion.identity, out surface_normal, out surface_penetration_depth))
            {
                closest_point = cast_pos + (surface_normal * (sphere_collider.radius - surface_penetration_depth));

                surface_normal = -surface_normal;
                return true;
            }

            return false;
        }
    }

    public enum Grounded
    {
        Air,
        Grass,
        Stone,
        Ceramic
    }
}
