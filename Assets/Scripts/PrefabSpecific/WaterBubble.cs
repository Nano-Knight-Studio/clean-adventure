using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBubble : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private GameObject splashPrefab;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float knockbackRadius;

    void OnCollisionEnter (Collision col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            col.gameObject.GetComponent<Enemy>().TakeDamage(damage);
        }
        if (col.gameObject.GetComponent<Rigidbody>())
        {
            col.gameObject.GetComponent<Rigidbody>().AddExplosionForce(knockbackForce, transform.position, knockbackRadius);
        }
        if (splashPrefab) Instantiate(splashPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
