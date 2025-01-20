using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gameracers.Movement
{
    public class BallMover : MonoBehaviour
    {
        [SerializeField] float speed = .3f;

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

        void FixedUpdate()
        {

        }

        //private void OnCollisionEnter(Collision collision)
        //{
        //    // calculate collisions here now except this time, i can use collision.point or CheckSphereExtra()

        //    Vector3 closestPoint, surfaceNormal;
        //    // reference pos is going to be the position of the ball at that frame. 
        //    Vector3 referencePos = transform.position;
        //    Debug.Log("Hit Something");
        //    if (CheckSphereExtra(collision.collider, sc, referencePos, out closestPoint, out surfaceNormal))
        //    {
        //        // This will likely be its own function and potentially repeated below in OnCollisionStay()

        //        // calculate if ball is currently falling, if the surface normal is ground or some other surface, calculate bounce trajectory and the resulting force and velocity due to the layer of the collider

        //        // take rb.velocity.magnitude and compare the angle to decide if we should bounce or roll. It appears Unity default is roll. 
        //        float angle = Vector3.Angle(collision.impulse, surfaceNormal);
        //        // if (trigonometry of vel.magn and angle is > something. Could be dependent on surface)
        //        if (collision.impulse.magnitude * Mathf.Cos(Mathf.Deg2Rad * angle) > 1f)
        //        {
        //            float bouncePercentage = 1f;
        //            if (collision.gameObject.layer == LayerMask.GetMask("Grass"))
        //            {
        //                bouncePercentage = grassBounce;
        //            }
        //            else if (collision.gameObject.layer == LayerMask.GetMask("Stone"))
        //            {
        //                bouncePercentage = stoneBounce;
        //            }
        //            else if (collision.gameObject.layer == LayerMask.GetMask("Ceramic"))
        //            {
        //                bouncePercentage = ceramicBounce;
        //            }

                    
        //            rb.AddForce(Vector3.Reflect(collision.impulse, surfaceNormal) * bouncePercentage, ForceMode.VelocityChange);
        //            Debug.Log("Boom");
        //        }
        //        // else
        //            //continue or do nothing
        //    }
        //}

        //private void OnCollisionStay(Collision collision)
        //{
        //    Vector3 closestPoint, surfaceNormal;
        //    // reference pos is going to be the position of the ball at that frame. 
        //    Vector3 referencePos = transform.position;

        //    if (CheckSphereExtra(collision.collider, sc, referencePos, out closestPoint, out surfaceNormal))
        //    {
        //        // This will likely be its own function and potentially repeated below in OnCollisionStay()

        //        // calculate if ball is currently falling, if the surface normal is ground or some other surface, calculate bounce trajectory and the resulting force and velocity due to the layer of the collider

        //        // take rb.velocity.magnitude and compare the angle to decide if we should bounce or roll. It appears Unity default is roll. 
        //        float angle = Vector3.Angle(rb.velocity, surfaceNormal);
        //        // if (trigonometry of vel.magn and angle is > something. Could be dependent on surface)
        //        if (rb.velocity.magnitude * Mathf.Cos(Mathf.Deg2Rad * angle) > 1f)
        //        {
        //            float bouncePercentage = 1f;
        //            if (collision.gameObject.layer == LayerMask.GetMask("Grass"))
        //            {
        //                bouncePercentage = grassBounce;
        //            }
        //            else if (collision.gameObject.layer == LayerMask.GetMask("Stone"))
        //            {
        //                bouncePercentage = stoneBounce;
        //            }
        //            else if (collision.gameObject.layer == LayerMask.GetMask("Ceramic"))
        //            {
        //                bouncePercentage = ceramicBounce;
        //            }

        //            rb.AddForce(Vector3.Reflect(rb.velocity, surfaceNormal) * bouncePercentage);
        //        }
        //        // else
        //        //continue or do nothing
        //    }
        //}

        private float CalculateBounce(int layerNum)
        {
            return 1f;
        }

        public Vector3 GetVelocity()
        {
            return rb.velocity;
        }

        public void ResetVelocity()
        {
            return;
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
}