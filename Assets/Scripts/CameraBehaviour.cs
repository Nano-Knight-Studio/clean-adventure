using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Transform [] targets;
    [Header("Position")]
    public float smoothTime;
    public Vector3 offset;
    [Header("Rotation")]
    public float lookSpeed;
    [Header("Input")]
    public string horizontalInput;
    public string verticalInput;
    public Vector2 sensivity;
    public Vector2 joystickSensivity;
    // INTERNAL
    Transform pointer;
    [HideInInspector] public Vector2 inputVector;
    [HideInInspector] public Transform orientation;
    public static CameraBehaviour instance;
    float cooldown = 0.0f;
    float desiredXRotation = 0.0f;
    Vector3 currentVelocity;

    void Awake ()
    {
        instance = this;
        pointer = new GameObject("Pointer").transform;
        pointer.SetParent(transform);
        pointer.localPosition = Vector3.zero;
        orientation = new GameObject("Orientation").transform;
        orientation.SetParent(transform);
        orientation.localPosition = Vector3.zero;

        Cursor.visible = false;
    }

    void Update ()
    {
        inputVector = new Vector2(Input.GetAxisRaw(horizontalInput) * -joystickSensivity.x + Input.GetAxisRaw("Mouse X") * sensivity.x,
                                                    Input.GetAxisRaw(verticalInput) * joystickSensivity.y + Input.GetAxisRaw("Mouse Y") * -sensivity.y);
    }

    void LateUpdate ()
    {
        if (inputVector.magnitude > 0.05f)
        {
            cooldown = 2.0f;
            transform.Rotate(Vector3.up * inputVector.x  * Time.deltaTime, Space.World);
            desiredXRotation += inputVector.y  * Time.deltaTime;
            desiredXRotation = Mathf.Clamp(desiredXRotation, -10, 15);
            transform.localEulerAngles = new Vector3(desiredXRotation, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
    }

    void FixedUpdate ()
    {
        //Position
        transform.position = Vector3.SmoothDamp(transform.position, GetAveragePosition() + offset, ref currentVelocity, smoothTime);

        //Rotation
        if (cooldown <= 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                            targets[0].rotation,
                                            lookSpeed * GetAverageVelocity() * Time.fixedDeltaTime);
        }
        else
        {
            cooldown -= Time.fixedDeltaTime;
        }

        //Resetting orientation
        orientation.eulerAngles = transform.eulerAngles;
        orientation.eulerAngles = new Vector3(0, orientation.eulerAngles.y, 0);
    }

    Vector3 GetAveragePosition ()
    {
        Vector3 average = Vector3.zero;
        int activeTargets = 0;
        for(int i = 0; i < targets.Length; i++)
        {
            if (targets[i].gameObject.activeSelf)
            {
                average += targets[i].position;
                activeTargets++;
            }
        }
        if (activeTargets > 0) average /= activeTargets;
        return average;
    }

    float GetAverageVelocity ()
    {
        return 1;
        float average = 0.0f;
        int activeTargets = 0;
        for(int i = 0; i < targets.Length; i++)
        {
            if (targets[i].gameObject.activeSelf)
            {
                average += targets[i].gameObject.GetComponent<Rigidbody>().velocity.magnitude;
                activeTargets++;
            }
        }
        if (activeTargets > 0) average /= activeTargets;
        return average;
    }
}