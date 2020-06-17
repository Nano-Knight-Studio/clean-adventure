using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private float respawnTime = 30.0f;
    private MeshRenderer[] meshRenderers;
    private Collider[] colliders;
    public bool spawnedOnMap = true;

    void Start ()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        colliders = GetComponentsInChildren<Collider>();
    }

    public void Collect ()
    {
        // Respawn / Destroy
        if (spawnedOnMap)
        {
            StartCoroutine(Respawn());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator Respawn()
    {
        foreach (MeshRenderer mr in meshRenderers)
        {
            mr.enabled = false;
        }
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }
        yield return new WaitForSeconds(respawnTime);
        foreach (MeshRenderer mr in meshRenderers)
        {
            mr.enabled = true;
        }
        foreach (Collider c in colliders)
        {
            c.enabled = true;
        }
    }
}