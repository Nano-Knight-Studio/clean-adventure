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
    [SerializeField] private Vector3 selfKnockback;
    [Header("Taking Damage")]
    [SerializeField] private float damageScaleMultiplier;
    [SerializeField] private float damageScaleSpeed;
    [SerializeField] private float stunTime;
    [Header("Graphics")]
    [SerializeField] private Material[] defaultMaterials;
    [SerializeField] private Material[] damageMaterials;
    [SerializeField] private GameObject deathParticles;
    [SerializeField] private MeshRenderer mainRenderer;
    [Header("Audio")]
    [SerializeField] private float soundPitchMin;
    [SerializeField] private float soundPitchMax;
    [SerializeField] private string[] damageSounds;
    [SerializeField] private string[] deathSounds;
    [SerializeField] private string[] idleSounds;
    [SerializeField] private string[] angrySounds;
    [Header("Drops")]
    [SerializeField] private GameObject[] drops;
    [SerializeField] private float[] probabilities;
    //----- INTERNAL -----//
    private Vector3 desiredScale;
    private Vector3 defaultScale;
    private NavMeshAgent navMeshAgent;
    public Transform target;
    private bool attacking = false;
    private Transform pointer;
    private bool stunned = false;
    private Rigidbody rb;
    private float timeOutsideCamera;
    public float defaultLifeBarSize;

    void Start ()
    {
        defaultScale = transform.localScale;
        desiredScale = defaultScale;
        navMeshAgent = GetComponent<NavMeshAgent>();
        pointer = new GameObject("Pointer").transform;
        pointer.SetParent(transform);
        pointer.localPosition = Vector3.zero;
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, UnityEngine.Random.Range(0.0f, 360.0f), transform.localEulerAngles.z);
        rb = GetComponent<Rigidbody>();
        StartCoroutine(PlayIdleSound());
        defaultLifeBarSize = lifeBar.size.x;
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
        AudioManager.instance.SetPitch(selectedSound, UnityEngine.Random.Range(soundPitchMin, soundPitchMax));
        AudioManager.instance.PlaySound(selectedSound, transform.position);
        StartCoroutine(PlayIdleSound());
    }

    void ApplyDamage()
    {
        attacking = false;
        rb.AddForce(transform.forward * selfKnockback.z + transform.right * selfKnockback.x + transform.up * selfKnockback.y);
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
                if (EnemyGlobalSettings.instance.isGameplayActive)
                {
                    target = player.transform;
                    navMeshAgent.SetDestination(player.transform.position);
                }
            }
        }
        catch (Exception ex) {}
    }

    public void TakeDamage(float damage)
    {
        currentLife -= damage;
        currentLife = Mathf.Clamp(currentLife, 0, maxLife);
        StopAllCoroutines();
        StartCoroutine(BlinkDamage());
        if (currentLife <= 0.0f)
        {
            Die(true, true);
        }
        else
        {
            string selectedSound = damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)];
            AudioManager.instance.SetPitch(selectedSound, UnityEngine.Random.Range(soundPitchMin, soundPitchMax));
            AudioManager.instance.PlaySound(selectedSound, transform.position);
            StartCoroutine(Stun(stunTime));
            lifeBar.size = new Vector2((currentLife / maxLife) * defaultLifeBarSize, lifeBar.size.y);
            lifeBar.transform.localPosition = new Vector3(Mathf.Lerp(defaultLifeBarSize / 2.0f, 0.0f, lifeBar.size.x), 0, 0);
        }
    }

    IEnumerator BlinkDamage()
    {
        mainRenderer.materials = damageMaterials;
        desiredScale = defaultScale * damageScaleMultiplier;
        yield return new WaitForSeconds(0.15f);
        desiredScale = defaultScale;
        mainRenderer.materials = defaultMaterials;
    }

    public void Die(bool makeSound, bool drop)
    {
        if (makeSound)
        {
            string selectedSound = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
            AudioManager.instance.SetPitch(selectedSound, UnityEngine.Random.Range(soundPitchMin, soundPitchMax));
            AudioManager.instance.PlaySound(selectedSound, transform.position);
        }
        if (deathParticles) 
        {
            Instantiate(deathParticles, transform.position, transform.rotation);
        }

        // Drops
        if (drop)
        {
            float rnd = UnityEngine.Random.Range(0.0f, 100.0f);
            print("drop: " + rnd);
            int selectedIndex = 0;
            for (int i=0; i < probabilities.Length; i++)
            {
                if (i < probabilities.Length -1)
                {
                    if (i == 0)
                    {
                        if (rnd <= probabilities[i])
                        {
                            selectedIndex = i;
                            break;
                        }
                    }
                    else
                    {
                        if (rnd > probabilities[i -1] &&
                            rnd < probabilities[i])
                        {
                            selectedIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    selectedIndex = i;
                    break;
                }
            }
            if (drops[selectedIndex] != null)
            {
                GameObject obj = Instantiate(drops[selectedIndex], transform.position, transform.rotation);
                obj.GetComponent<Collectable>().spawnedOnMap = false;
            }
        }

        Destroy(gameObject);
    }

    void OnCollisionEnter (Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            ApplyDamage();
        }
    }
}