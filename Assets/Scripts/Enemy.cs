using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Life")]
    public float maxLife;
    public float currentLife;
    public Slider lifeBar;
    [Header("Attack")]
    public Transform attackPoint;
    public float attackRange;
    public float damage;
    public LayerMask attackLayerMask;
    //----- INTERNAL -----//
    NavMeshAgent navMeshAgent;
    Transform target;
    bool attacking = false;
    Transform pointer;

    void Start ()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        pointer = new GameObject("Pointer").transform;
        pointer.SetParent(transform);
        pointer.localPosition = Vector3.zero;
    }

    void Update ()
    {
        if (target)
        {
            // Arrived
            if (Vector3.Distance(transform.position, target.position) < navMeshAgent.stoppingDistance && !attacking)
            {
                Attack();
            }
            pointer.LookAt(target);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, pointer.eulerAngles.y, 0)), 3 * Time.deltaTime);
        }
    }

    void Attack()
    {
        Debug.Log("Enemy: Starting attack");
        attacking = true;
        //TODO animation
        //Animation will call this:
        ApplyDamage();
    }

    void ApplyDamage()
    {
        Debug.Log("Enemy: Applying damage");
        attacking = false;
        foreach (Collider c in Physics.OverlapSphere(attackPoint.position, attackRange, attackLayerMask))
        {
            if (c.gameObject.tag == "Player")
            {
                c.gameObject.GetComponent<PlayerStats>().TakeDamage(damage);
            }
        }
    }

    public void DetectPlayer (GameObject player)
    {
        try
        {
            navMeshAgent.SetDestination(player.transform.position);
            target = player.transform;
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