using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NavigationSystem))]
public class PlayerNavigation : MonoBehaviour
{
    public Transform target;
    public Transform pointer;
    public float minDistance = 1;

    void Update ()
    {
        if (Vector3.Distance(transform.position, target.position) >= minDistance)
        {
            pointer.gameObject.SetActive(true);
            pointer.LookAt(target);
        }
        else
        {
            pointer.gameObject.GetComponent<Animator>().SetTrigger("Disable");
        }
    }
}
