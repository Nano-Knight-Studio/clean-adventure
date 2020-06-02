using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class MapPoint : MonoBehaviour
{
    public bool debug = false;
    public MapPoint[] availablePoints;
    private Transform pointer;

    void Start()
    {
        //Instantiating Pointer
        pointer = new GameObject("Pointer").transform;
        pointer.SetParent(transform);
        pointer.localPosition = Vector3.zero;
    }

    void Update()
    {
        //DEBUG
        if (debug)
        {
            foreach (MapPoint p in availablePoints)
            {
                pointer.LookAt(p.transform);
                Debug.DrawRay(transform.position, pointer.forward * Vector3.Distance(transform.position, p.transform.position), Color.blue, 0.1f);
            }
        }
    }
}
