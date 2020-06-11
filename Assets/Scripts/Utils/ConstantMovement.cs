using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantMovement : MonoBehaviour
{
    public Vector3 velocity;
    public Vector3 maxVelocity;
    public Vector3 increase;
    public float acceleration;
    public Vector3 currentVelocity;

    void FixedUpdate ()
    {
        velocity += increase * Time.deltaTime;
        velocity = new Vector3(Mathf.Clamp(velocity.x, 0, maxVelocity.x), Mathf.Clamp(velocity.y, 0, maxVelocity.y), Mathf.Clamp(velocity.z, 0, maxVelocity.z));
        currentVelocity = Vector3.Lerp(currentVelocity, velocity, acceleration * Time.fixedDeltaTime);
        transform.Translate(currentVelocity * Time.fixedDeltaTime, Space.World);
    }
}