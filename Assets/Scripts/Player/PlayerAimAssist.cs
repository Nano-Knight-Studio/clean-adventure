using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimAssist : MonoBehaviour
{
    List<GameObject> enemiesOnSight = new List<GameObject>();
    Transform pointer;

    void Start ()
    {
        pointer = new GameObject("Pointer").transform;
        pointer.SetParent(transform);
        pointer.localPosition = Vector3.zero;
    }

    void Update ()
    {
        //TODO optimization
        CleanList();
    }

    public Vector3 GetAimAssistDirection()
    {
        try
        {
            pointer.LookAt(GetClosestEnemy().transform);
            pointer.localEulerAngles = new Vector3(0.0f, pointer.localEulerAngles.y, 0.0f);
            return pointer.forward;
        }
        catch (NullReferenceException ex)
        {
            return Vector3.zero;
        }
    }

    public GameObject GetClosestEnemy()
    {
        float smallestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;
        foreach (GameObject obj in enemiesOnSight)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                closestEnemy = obj;
            }
        }
        return closestEnemy;
    }

    void CleanList()
    {
        for (int i=0; i < enemiesOnSight.Count; i++)
        {
            if (enemiesOnSight[i] == null)
            {
                enemiesOnSight.RemoveAt(i);
                CleanList();
                return;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            enemiesOnSight.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            enemiesOnSight.Remove(other.gameObject);
        }
    }
}
