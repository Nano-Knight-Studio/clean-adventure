using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    NavMeshAgent navMeshAgent;

    void Start ()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void DetectPlayer (GameObject player)
    {
        navMeshAgent.SetDestination(player.transform.position);
    }
}