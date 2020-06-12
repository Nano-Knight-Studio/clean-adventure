using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxLife;
    [SerializeField] private float currentLife;
    [SerializeField] private string[] damageSounds;
    [SerializeField] private Transform leftHand;

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

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Goal")
        {
            other.gameObject.GetComponent<SimpleRotation>().enabled = false;
            UserInterface.instance.CollectGoal();
            other.gameObject.transform.SetParent(leftHand);
            other.gameObject.transform.localPosition = Vector3.zero;
            other.gameObject.transform.localEulerAngles = Vector3.zero;
        }
    }
}
