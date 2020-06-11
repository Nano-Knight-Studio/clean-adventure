using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    public float speed;
    public float acceleration;
    public Transform grabPosition;
    public float distanceFactor;
    public float dropDistance;
    Transform pointer;
    public KeyCode[] keys;
    public List<GameObject> objectsInRange = new List<GameObject>();
    GameObject grabbedObject;

    void Start ()
    {
        pointer = new GameObject("Pointer").transform;
        pointer.SetParent(transform);
    }

    void Update ()
    {
        //GETTING INPUT
        bool pressed = false;
        foreach (KeyCode k in keys)
        {
            if (Input.GetKeyDown(k))
            {
                pressed = true;
            }
        }

        //KEY PRESS
        if (pressed)
        {
            //DROP
            if (grabbedObject != null)
            {
                DropObject();
            }
            //GRAB
            else if (GetClosestObject() != null)
            {
                GrabObject();
            }
        }

        //Floating
        if (grabbedObject != null)
        {
            pointer.position = grabbedObject.transform.position;
            pointer.LookAt(grabPosition);
            grabbedObject.GetComponent<Rigidbody>().velocity = Vector3.Lerp(grabbedObject.GetComponent<Rigidbody>().velocity,
                                                                            pointer.forward * speed * Vector3.Distance(grabPosition.position, grabbedObject.transform.position) * distanceFactor,
                                                                            acceleration * Time.deltaTime) + GetComponent<Rigidbody>().velocity * 0.2f;
        }

        //Dropping by distance or wall
        if (grabbedObject != null)
        {
            //Distance
            if (Vector3.Distance(grabbedObject.transform.position, transform.position) > dropDistance)
            {
                GameObject tmp = grabbedObject;
                DropObject();
                RemoveObjectFromRange(tmp);
            }
            
            //Blocked by something
            if (!HasLineOfSightTo(grabbedObject))
            {
                DropObject();
            }
        }

        //TEMPORARIO
        CleanObjectList();
    }

    void CleanObjectList ()
    {
        for (int i=0; i < objectsInRange.Count; i++)
        {
            if (objectsInRange[i] == null)
            {
                objectsInRange.RemoveAt(i);
                CleanObjectList();
                return;
            }
        }
    }

    bool HasLineOfSightTo(GameObject obj)
    {
        if (obj == null)
            return false;
        pointer.position = obj.transform.position;
        pointer.LookAt(transform);
        pointer.Translate(Vector3.forward * 0, Space.Self);
        RaycastHit[] hits = Physics.RaycastAll(pointer.position, pointer.forward, Vector3.Distance(pointer.position, transform.position) * 1.5f);
        Debug.DrawRay(pointer.position, pointer.forward * Vector3.Distance(pointer.position, transform.position) * 1.5f, Color.blue);
        foreach (RaycastHit h in hits)
        {
            if (h.collider.gameObject.tag != "Player" && h.collider.gameObject.layer != 10)
            {
                return false;
            }
        }
        return true;
    }

    public GameObject GetClosestObject ()
    {
        //Local variables
        GameObject closestObject = null;
        float smallestDistance = -1.0f;

        //Looping through every object in range
        foreach (GameObject obj in objectsInRange)
        {
            float currentDistance = Vector3.Distance(grabPosition.position, obj.transform.position);
            if (currentDistance < smallestDistance || smallestDistance < 0)
            {
                smallestDistance = currentDistance;
                closestObject = obj;
            }
        }

        return closestObject;
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.GetComponent<Rigidbody>())
        {
            if (grabbedObject != null)
            {
                if (other.gameObject == grabbedObject)
                {
                    return;
                }
            }
            AddObjectToRange(other.gameObject);
        }
    }

    void OnTriggerExit (Collider other)
    {
        if (other.gameObject.GetComponent<Rigidbody>())
        {
            RemoveObjectFromRange(other.gameObject);
        }
    }

    void GrabObject()
    {
        if (GetClosestObject().layer == 0)
        {
            grabbedObject = GetClosestObject();
            grabbedObject.layer = 10;
            RemoveObjectFromRange(grabbedObject);
        }
    }

    void DropObject()
    {
        AddObjectToRange(grabbedObject);
        grabbedObject.layer = 0;
        grabbedObject = null;
    }

    void AddObjectToRange(GameObject obj)
    {
        if (!objectsInRange.Contains(obj))
        {
            objectsInRange.Add(obj);
        }
    }

    void RemoveObjectFromRange(GameObject obj)
    {
        if (objectsInRange.Contains(obj))
        {
            objectsInRange.Remove(obj);
        }
    }
}
