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
    public float attackRange;
    public float damage;
    public LayerMask attackLayerMask;
    [Header("Graphics")]
    public Material defaultMaterial;
    public Material damageMaterial;
    //----- INTERNAL -----//
    NavMeshAgent navMeshAgent;
    Transform target;
    bool attacking = false;
    Transform pointer;
    bool stunned = false;
    MeshRenderer[] renderers;

    void Start ()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        pointer = new GameObject("Pointer").transform;
        pointer.SetParent(transform);
        pointer.localPosition = Vector3.zero;
        renderers = GetComponentsInChildren<MeshRenderer>();
    }

    void FixedUpdate ()
    {
        if (target)
        {
            pointer.LookAt(target);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, pointer.eulerAngles.y, 0)), 3 * Time.fixedDeltaTime);
        }
    }

    void ApplyDamage()
    {
        Debug.Log("Enemy: Applying damage");
        attacking = false;
        foreach (Collider c in Physics.OverlapSphere(transform.position, attackRange, attackLayerMask))
        {
            if (c.gameObject.tag == "Player")
            {
                c.gameObject.GetComponent<PlayerStats>().TakeDamage(damage);
            }
        }
    }

    IEnumerator Stun(float seconds)
    {
        navMeshAgent.enabled = false;
        yield return new WaitForSeconds(seconds);
        navMeshAgent.enabled = true;
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
        StartCoroutine(BlinkDamage());
        if (currentLife <= 0.0f)
        {
            Die();
        }
        else
        {
            StartCoroutine(Stun(1.5f));
            lifeBar.value = currentLife / maxLife;
        }
    }

    IEnumerator BlinkDamage()
    {
        foreach (MeshRenderer mr in renderers)
        {
            mr.material = damageMaterial;
        }
        yield return new WaitForSeconds(0.1f);
        foreach (MeshRenderer mr in renderers)
        {
            mr.material = defaultMaterial;
        }
    }

    void Die()
    {
        //TODO death effects
        Destroy(gameObject);
    }

    void OnCollisionEnter (Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            ApplyDamage();
            Die();
        }
    }
}