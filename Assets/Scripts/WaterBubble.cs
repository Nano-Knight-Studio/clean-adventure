using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBubble : MonoBehaviour
{
    public float damage;
    public GameObject splashPrefab;

    void OnCollisionEnter (Collision col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            col.gameObject.GetComponent<Enemy>().TakeDamage(damage);
        }
        if (splashPrefab) Instantiate(splashPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
