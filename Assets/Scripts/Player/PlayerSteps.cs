using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSteps : MonoBehaviour
{
    [SerializeField] private string[] grassSounds;
    [SerializeField] private string[] stoneSounds;
    [SerializeField] private float groundDistance;
    [SerializeField] private LayerMask groundLayerMask;
    //----- INTERNAL -----//
    private Collider mainCollider;

    void Start ()
    {
        mainCollider = GetComponentInParent<Collider>();
    }

    public void PlayStepSound ()
    {
        GameObject grnd = GetGroundObject();
        if (grnd && grnd.GetComponent<GroundTypeInfo>())
        {
            switch (grnd.GetComponent<GroundTypeInfo>().groundType)
            {
                case GroundType.Grass:
                    AudioManager.instance.PlaySound(grassSounds[Random.Range(0, grassSounds.Length)]);
                    break;
                case GroundType.Stone:
                    AudioManager.instance.PlaySound(stoneSounds[Random.Range(0, stoneSounds.Length)]);
                    break;
            }
        }
    }

    GameObject GetGroundObject ()
    {
        // Debugging
        Debug.DrawRay (mainCollider.bounds.center + new Vector3(0, (-mainCollider.bounds.size.y * 0.6f) / 2, 0), Vector3.down * groundDistance, Color.blue, 0.1f);

        // Getting ray
        RaycastHit hit;
        Physics.Raycast(mainCollider.bounds.center + new Vector3(0, (-mainCollider.bounds.size.y * 0.6f) / 2, 0), Vector3.down, out hit, groundDistance, groundLayerMask);
        if (hit.collider != null) //Hits a object
        {
            return hit.collider.gameObject;
        }

        return null;
    }
}
