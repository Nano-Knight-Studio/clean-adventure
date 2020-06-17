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
            UserInterface.instance.Lose();
        }
        else
        {
            string selectedAudio = damageSounds[Random.Range(0, damageSounds.Length)];
            AudioManager.instance.SetPitch(selectedAudio, Random.Range(0.95f, 1.05f));
            AudioManager.instance.PlaySound(selectedAudio);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Goal")
        {
            foreach (SimpleRotation s in other.gameObject.GetComponentsInChildren<SimpleRotation>())
            {
                s.enabled = false;
                // TODO fix
                s.gameObject.transform.localEulerAngles = new Vector3(12.439f, -28.887f, 58.768f);
                s.gameObject.transform.localPosition = new Vector3(-0.0101f, 0.0243f, 0.0017f);
            }
            UserInterface.instance.CollectGoal();
            other.gameObject.transform.SetParent(leftHand);
            other.gameObject.transform.localPosition = Vector3.zero;
            other.gameObject.transform.localEulerAngles = Vector3.zero;
            other.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        if (other.gameObject.tag == "DifficultyTrigger")
        {
            EnemyGlobalSettings.NextLevel();
            Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "Health")
        {
            currentLife = maxLife;
            UserInterface.instance.RefreshPlayerStats();
            other.gameObject.GetComponent<Collectable>().Collect();
            AudioManager.instance.PlaySound("Heart_Collect");
        }
    }

    public float GetHealthPercentage()
    {
        return currentLife / maxLife;
    }
}
