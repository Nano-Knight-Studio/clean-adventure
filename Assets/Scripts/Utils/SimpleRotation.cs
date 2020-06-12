using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotation : MonoBehaviour
{
    public Vector3 rotation;
    public bool global = true;

    public void Update ()
    {
        if (global)
        {
            transform.Rotate(rotation * Time.deltaTime, Space.World);
        }
        else
        {
            transform.Rotate(rotation * Time.deltaTime, Space.Self);
        }
    }
}
