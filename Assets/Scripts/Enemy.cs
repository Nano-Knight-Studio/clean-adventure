using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float maxLife;
    public float currentLife;
    public Slider lifeBar;
    NavMeshAgent navMeshAgent;

    void Start ()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void DetectPlayer (GameObject player)
    {
        try
        {
            navMeshAgent.SetDestination(player.transform.position);
        }
        catch (Exception ex) {}
    }

    public void TakeDamage(float damage)
    {
        currentLife -= damage;
        currentLife = Mathf.Clamp(currentLife, 0, maxLife);
        if (currentLife <= 0.0f)
        {
            //TODO death effects
            Destroy(gameObject);
        }
        else
        {
            lifeBar.value = currentLife / maxLife;
        }
    }
}