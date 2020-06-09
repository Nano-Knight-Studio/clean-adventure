using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public Animator animator;
    public GameObject prefab;
    public Transform shootPoint;
    public float force;
    public string soundName;
    public KeyCode key;
    public float shootDelay;
    bool shooting = false;
    bool canShoot = true;
    public float spread;
    public float scaleRandomizationFactor;

    void Update ()
    {
        animator.SetBool("Shooting", Input.GetKey(key));

        if (canShoot && Input.GetKey(key))
        {
            StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot ()
    {
        canShoot = false;
        shootPoint.localEulerAngles = new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0);
        GameObject obj = Instantiate(prefab, shootPoint.position, shootPoint.rotation);
        obj.GetComponent<Rigidbody>().velocity = shootPoint.forward * force;
        // obj.transform.localScale = obj.transform.localScale * Random.Range(-scaleRandomizationFactor, +scaleRandomizationFactor);
        yield return new WaitForSeconds(shootDelay);
        canShoot = true;
    }
}