using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaterBubble : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float damage;
    [Range(0, 100)]
    [SerializeField] private float damagePercentageVariation;
    [Range(0, 100)]
    [SerializeField] private float criticalChance;
    [SerializeField] private float criticalDamage;
    [Range(0, 100)]
    [SerializeField] private float criticalDamagePercentageVariation;
    [Header("Prefabs")]
    [SerializeField] private GameObject splashPrefab;
    [SerializeField] private GameObject damageIndicator;
    [SerializeField] private GameObject damageIndicatorCritical;
    [Header("Knockback")]
    [SerializeField] private float knockbackForce;
    [SerializeField] private float knockbackRadius;

    void OnCollisionEnter (Collision col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            //Calculating damage
            float dmg = 0;
            bool critical = (Random.Range(0.0f, 100.0f) <= criticalChance);
            if (critical)
            {
                dmg = criticalDamage * (1 + Random.Range(-criticalDamagePercentageVariation / 100.0f, criticalDamagePercentageVariation / 100.0f));
            }
            else
            {
                dmg = damage * (1 + Random.Range(-damagePercentageVariation / 100.0f, damagePercentageVariation / 100.0f));
            }
            dmg = (int)dmg;

            //Damage indicador
            GameObject obj;
            if (critical)
                obj = Instantiate(damageIndicatorCritical, transform.position, transform.rotation);
            else
                obj = Instantiate(damageIndicator, transform.position, transform.rotation);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = dmg.ToString("F0");

            //Applying damage
            col.gameObject.GetComponent<Enemy>().TakeDamage(dmg);
        }
        if (col.gameObject.GetComponent<Rigidbody>())
        {
            col.gameObject.GetComponent<Rigidbody>().AddExplosionForce(knockbackForce, transform.position, knockbackRadius);
        }
        if (splashPrefab) Instantiate(splashPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
