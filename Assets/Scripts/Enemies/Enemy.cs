using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float turnSpeed;
    [Header("Life")]
    [SerializeField] private float maxLife;
    [SerializeField] private float currentLife;
    [SerializeField] private Slider lifeBar;
    [Header("Attack")]
    [SerializeField] private float attackRange;
    [SerializeField] private float damage;
    [SerializeField] private LayerMask attackLayerMask;
    [Header("Graphics")]
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material damageMaterial;
    [SerializeField] private GameObject deathParticles;
    [SerializeField] private MeshRenderer mainRenderer;
    //----- INTERNAL -----//
    private NavMeshAgent navMeshAgent;
    private Transform target;
    private bool attacking = false;
    private Transform pointer;
    private bool stunned = false;

    void Start ()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        pointer = new GameObject("Pointer").transform;
        pointer.SetParent(transform);
        pointer.localPosition = Vector3.zero;
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, UnityEngine.Random.Range(0.0f, 360.0f), transform.localEulerAngles.z);
    }

    void FixedUpdate ()
    {
        if (target)
        {
            pointer.LookAt(target);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, pointer.eulerAngles.y, 0)), turnSpeed * Time.fixedDeltaTime);
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
            if (navMeshAgent.isActiveAndEnabled)
            {
                navMeshAgent.SetDestination(player.transform.position);
                target = player.transform;
            }
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
        mainRenderer.material = damageMaterial;
        yield return new WaitForSeconds(0.1f);
        mainRenderer.material = defaultMaterial;
    }

    void Die()
    {
        //TODO death effects
        if (deathParticles) Instantiate(deathParticles, transform.position, transform.rotation);
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