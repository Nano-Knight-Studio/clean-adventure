using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField] private float respawnTime = 30.0f;
    private MeshRenderer[] meshRenderers;
    private Collider[] colliders;

    void Start ()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        colliders = GetComponentsInChildren<Collider>();
    }

    public void Respawn ()
    {
        StartCoroutine(RespawnCoroutine());
    }

    IEnumerator RespawnCoroutine()
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