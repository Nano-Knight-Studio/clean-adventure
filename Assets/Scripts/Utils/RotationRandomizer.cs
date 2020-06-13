using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationRandomizer : MonoBehaviour
{
    [SerializeField] private bool global = true;
    [SerializeField] private bool X;
    [SerializeField] private bool Y;
    [SerializeField] private bool Z;

    void Start ()
    {
        if (global)
        {
            transform.eulerAngles = new Vector3(X ? Random.Range(0.0f, 360f) : transform.eulerAngles.x,
                                                Y ? Random.Range(0.0f, 360f) : transform.eulerAngles.y,
                                                Z ? Random.Range(0.0f, 360f) : transform.eulerAngles.z);
        }
        else
        {
            transform.localEulerAngles = new Vector3(X ? Random.Range(0.0f, 360f) : transform.localEulerAngles.x,
                                                     Y ? Random.Range(0.0f, 360f) : transform.localEulerAngles.y,
                                                     Z ? Random.Range(0.0f, 360f) : transform.localEulerAngles.z);
        }
    }
}
