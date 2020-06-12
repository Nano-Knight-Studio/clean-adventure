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
    [SerializeField] private SpriteRenderer lifeBar;
    [Header("Attack")]
    [SerializeField] private float attackRange;
    [SerializeField] private float damage;
    [SerializeField] private LayerMask attackLayerMask;
    [Header("Taking Damage")]
    [SerializeField] private float damageScaleMultiplier;
    [SerializeField] private float damageScaleSpeed;
    [Header("Graphics")]
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material damageMaterial;
    [SerializeField] private GameObject deathParticles;
    [SerializeField] private MeshRenderer mainRenderer;
    [Header("Audio")]
    [SerializeField] private string[] damageSounds;
    [SerializeField] private string[] deathSounds;
    [SerializeField] private string[] idleSounds;
    [SerializeField] private string[] angrySounds;
    //----- INTERNAL -----//
    private Vector3 desiredScale;
    private Vector3 defaultScale;
    private NavMeshAgent navMeshAgent;
    private Transform target;
    private bool attacking = false;
    private Transform pointer;
    private bool stunned = false;

    void Start ()
    {
        defaultScale = transform.localScale;
        desiredScale = defaultScale;
        navMeshAgent = GetComponent<NavMeshAgent>();
        pointer = new GameObject("Pointer").transform;
        pointer.SetParent(transform);
        pointer.localPosition = Vector3.zero;
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, UnityEngine.Random.Range(0.0f, 360.0f), transform.localEulerAngles.z);
        StartCoroutine(PlayIdleSound());
    }

    void Update ()
    {
        // Lerping scale
        // Used on damage effect
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, damageScaleSpeed * Time.deltaTime);
    }

    void FixedUpdate ()
    {
        if (target)
        {
            pointer.LookAt(target);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, pointer.eulerAngles.y, 0)), turnSpeed * Time.fixedDeltaTime);
        }
    }

    IEnumerator PlayIdleSound ()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(5.0f, 60.0f));
        string selectedSound = idleSounds[UnityEngine.Random.Range(0, idleSounds.Length)];
        AudioManager.instance.SetPitch(selectedSound, UnityEngine.Random.Range(0.7f, 1.3f));
        AudioManager.instance.PlaySound(selectedSound, transform.position);
        StartCoroutine(PlayIdleSound());
    }

    void ApplyDamage()
    {
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
                string selectedSound = angrySounds[UnityEngine.Random.Range(0, angrySounds.Length)];
                // AudioManager.instance.SetPitch(selectedSound, UnityEngine.Random.Range(0.7f, 1.3f));
                // AudioManager.instance.PlaySound(selectedSound, transform.position);
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
            string selectedSound = damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)];
            AudioManager.instance.SetPitch(selectedSound, UnityEngine.Random.Range(0.7f, 1.3f));
            AudioManager.instance.PlaySound(selectedSound, transform.position);
            StartCoroutine(Stun(1.5f));
            lifeBar.size = new Vector2(currentLife / maxLife, lifeBar.size.y);
            lifeBar.transform.localPosition = new Vector3(Mathf.Lerp(0.5f, 0.0f, lifeBar.size.x), 0, 0);
        }
    }

    IEnumerator BlinkDamage()
    {
        mainRenderer.material = damageMaterial;
        desiredScale = defaultScale * damageScaleMultiplier;
        yield return new WaitForSeconds(0.15f);
        desiredScale = defaultScale;
        mainRenderer.material = defaultMaterial;
    }

    void Die()
    {
        //TODO death effects
        string selectedSound = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
        AudioManager.instance.SetPitch(selectedSound, UnityEngine.Random.Range(0.8f, 1.2f));
        AudioManager.instance.PlaySound(selectedSound, transform.position);
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