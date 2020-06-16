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
        UserInterface.instance.RefreshPlayerStats();
        CameraBehaviour.instance.CameraShake("CameraShake0");
        if (currentLife <= 0.0f)
        {
            GetComponentInParent<PlayerMovement>().enabled = false;
            GetComponentInParent<PlayerShooting>().enabled = false;
            GetComponentInParent<Rigidbody>().constraints = RigidbodyConstraints.None;
            GetComponentInParent<Rigidbody>().AddTorque(new Vector3(Random.Range(-30, 30), Random.Range(-30, 30), Random.Range(-30, 30)));
            GetComponentInParent<PlayerMovement>().animator.SetFloat("Walk", 0.0f);
            GetComponentInParent<PlayerMovement>().animator.SetBool("Walking", false);
        }
        else
        {
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

        if (other.gameObject.tag == "DifficultyTrigger")
        {
            EnemyGlobalSettings.NextLevel();
            Destroy(other.gameObject);
        }
    }

    public float GetHealthPercentage()
    {
        return currentLife / maxLife;
    }
}
