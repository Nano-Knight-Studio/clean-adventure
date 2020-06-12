using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxLife;
    [SerializeField] private float currentLife;
    [SerializeField] private string[] damageSounds;

    public void TakeDamage(float damage)
    {
        currentLife -= damage;
        currentLife = Mathf.Clamp(currentLife, 0, maxLife);
        CameraBehaviour.instance.CameraShake("CameraShake0");
        if (currentLife <= 0.0f)
        {
            //TODO death effects
        }
        else
        {
            //TODO refresh UI
            string selectedAudio = damageSounds[Random.Range(0, damageSounds.Length)];
            AudioManager.instance.SetPitch(selectedAudio, Random.Range(0.7f, 1.3f));
            AudioManager.instance.PlaySound(selectedAudio);
        }
    }
}
