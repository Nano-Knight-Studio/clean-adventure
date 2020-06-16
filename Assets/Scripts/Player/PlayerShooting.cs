using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerShooting : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float force;
    [SerializeField] private float shootDelay;
    [SerializeField] private float spread;
    [Header("Audio")]
    [SerializeField] private string[] soundNames;
    [SerializeField] private string outOfAmmoSound;
    [SerializeField] private string reloadSound;
    [Header("Ammo")]
    [SerializeField] private int ammo;
    [SerializeField] private int maxAmmo;
    [Header("Other References")]
    [SerializeField] private KeyCode[] keys;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Animator animator;
    //----- INTERNAL -----//
    [HideInInspector] public bool keyPressed = false;
    private bool canShoot = true;
    private bool mobile = false;
    private Vector3 shootPointDefaultRotation;

    void Awake ()
    {
        #if UNITY_ANDROID
        mobile = true;
        #endif
        #if UNITY_IOS
        mobile = true;
        #endif
    }

    void Start ()
    {
        shootPointDefaultRotation = shootPoint.localEulerAngles;
    }

    void Update ()
    {
        // Detecting key press
        if (mobile)
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton2))
            {
                keyPressed = true;
            }
            if (Input.GetKeyUp(KeyCode.JoystickButton2))
            {
                keyPressed = false;
            }
            if (CrossPlatformInputManager.GetButtonDown("Fire"))
            {
                keyPressed = true;
            }
            if (CrossPlatformInputManager.GetButtonUp("Fire"))
            {
                keyPressed = false;
            }
        }
        else
        {
            keyPressed = false;
            foreach (KeyCode k in keys)
            {
                if (Input.GetKey(k))
                {
                    keyPressed = true;
                }
            }
        }

        // Animation(s)
        animator.SetBool("Shooting", keyPressed);

        // Shoot
        if (canShoot && keyPressed)
        {
            StartCoroutine(Shoot());
        }
    }

    public void Reload()
    {
        AudioManager.instance.PlaySound(reloadSound);
        ammo = maxAmmo;
        UserInterface.instance.RefreshPlayerStats();
    }

    IEnumerator Shoot ()
    {
        // Blocking parallel coroutines
        canShoot = false;
        if (ammo < 0)
        {
            AudioManager.instance.SetPitch(outOfAmmoSound, Random.Range(1.25f, 1.6f));
            AudioManager.instance.PlaySound(outOfAmmoSound);
        }
        else
        {
            // Consuming ammo
            ammo--;

            // Spread
            shootPoint.localEulerAngles = shootPointDefaultRotation + new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0);

            //TODO Object Pooling
            // Instantiate bullet
            GameObject obj = Instantiate(prefab, shootPoint.position, shootPoint.rotation);

            // Sound
            string selectedSound = soundNames[Random.Range(0, soundNames.Length)];
            AudioManager.instance.SetPitch(selectedSound, Random.Range(0.4f, 0.6f));
            AudioManager.instance.PlaySound(selectedSound);

            // Applying force
            obj.GetComponent<Rigidbody>().velocity = shootPoint.forward * force;

            // Refreshing UI
            UserInterface.instance.RefreshPlayerStats();
        }
        // Wait and allow next shot
        yield return new WaitForSeconds(shootDelay);
        canShoot = true;
    }

    public float GetAmmoPercentage()
    {
        return (float)ammo / (float) maxAmmo;
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.tag == "Ammo")
        {
            Reload();
            Destroy(other.gameObject);
        }
    }    
}