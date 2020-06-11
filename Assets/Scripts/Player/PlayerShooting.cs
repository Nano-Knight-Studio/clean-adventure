using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private KeyCode[] keys;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float force;
    [SerializeField] private Animator animator;
    [SerializeField] private string[] soundNames;
    [SerializeField] private float shootDelay;
    [SerializeField] private float spread;
    //----- INTERNAL -----//
    [HideInInspector] public bool keyPressed = false;
    private bool canShoot = true;

    void Update ()
    {
        // Detecting key press
        keyPressed = false;
        foreach (KeyCode k in keys)
        {
            if (Input.GetKey(k))
            {
                keyPressed = true;
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

    IEnumerator Shoot ()
    {
        // Blocking parallel coroutines
        canShoot = false;

        // Spread
        shootPoint.localEulerAngles = new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0);

        //TODO Object Pooling
        // Instantiate bullet
        GameObject obj = Instantiate(prefab, shootPoint.position, shootPoint.rotation);

        // Sound
        string selectedSound = soundNames[Random.Range(0, soundNames.Length)];
        AudioManager.instance.SetPitch(selectedSound, Random.Range(0.9f, 1.1f));
        AudioManager.instance.PlaySound(selectedSound);

        // Applying force
        obj.GetComponent<Rigidbody>().velocity = shootPoint.forward * force;

        // Wait and allow next shot
        yield return new WaitForSeconds(shootDelay);
        canShoot = true;
    }
}