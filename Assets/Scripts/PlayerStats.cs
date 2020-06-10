using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float maxLife;
    public float currentLife;

    public void TakeDamage(float damage)
    {
        currentLife -= damage;
        currentLife = Mathf.Clamp(currentLife, 0, maxLife);
        if (currentLife <= 0.0f)
        {
            //TODO death effects
        }
        else
        {
            //TODO refresh UI
        }
    }
}
