using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform following;
    Rigidbody rb;
    [Range(0.0f, 1.0f)]
    public float interested;
    [Range(0.0f, 1.0f)]
    public float lerpSpd = .5f;

    private void Start()
    {
        rb = following.GetComponent<Rigidbody>();
    }

    private void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, following.position, interested);
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, rb.velocity.normalized, lerpSpd));
    }
}
