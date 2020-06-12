using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeBillboard : MonoBehaviour
{
    private Transform camera;

    void Start ()
    {
        camera = CameraBehaviour.instance.GetComponentInChildren<Camera>().transform;
    }

    void LateUpdate ()
    {
        transform.LookAt(camera);
    }
}
