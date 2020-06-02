using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public bool active = true;
    public Transform [] targets;
    [Header("Position")]
    public float movementSpeed;
    public Vector3 offset;
    [Header("Rotation")]
    public float lookSpeed;

    // INTERNAL

    Transform pointer;
    [HideInInspector] public Transform orientation;
    public static CameraBehaviour instance;

    void Awake ()
    {
        instance = this;
    }

    void Start ()
    {
        pointer = new GameObject("Pointer").transform;
        pointer.SetParent(transform);
        pointer.localPosition = Vector3.zero;
        orientation = new GameObject("Orientation").transform;
        orientation.SetParent(transform);
        orientation.localPosition = Vector3.zero;
    }

    void FixedUpdate ()
    {
        //Position
        transform.position = Vector3.Lerp(transform.position,
                                        GetAveragePosition() + offset,
                                        movementSpeed * Time.fixedDeltaTime);

        //Rotation
        // pointer.LookAt(Vector3.Lerp(focusPoint.position,
        //                             new Vector3(transform.position.x, focusPoint, GetAveragePosition().z),
        //                             playerInfluence));
        pointer.LookAt(GetAveragePosition());
        transform.rotation = Quaternion.Slerp(transform.rotation,
                                        pointer.rotation,
                                        lookSpeed * Time.fixedDeltaTime);

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
}