﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] prefabs;
    public Transform streets;
    public float height;
    Collider[] streetColliders;
    public List<GameObject> enemies = new List<GameObject>();

    void Start ()
    {
        streetColliders = GetComponentsInChildren<BoxCollider>();
        SpawnAll();
        StartCoroutine(Respawn());
    }

    public void SpawnAll()
    {
        CleanList();
        foreach(Collider c in streetColliders)
        {
            for (int i=enemies.Count; i < EnemyGlobalSettings.density; i++)
            {
                Vector3 pos = GenerateRandomPointAtCollider(c);
                pos = new Vector3(pos.x, height, pos.z);
                enemies.Add(Instantiate(prefabs[Random.Range(0, prefabs.Length)], pos, Quaternion.identity));
            }
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(Random.Range(5.0f, 15.0f));
        SpawnAll();
        StartCoroutine(Respawn());
    }

    void CleanList()
    {
        for (int i=0; i < enemies.Count; i++)
        {
            if (enemies[i] == null)
            {
                enemies.RemoveAt(i);
                CleanList();
                return;
            }
        }
    }

    Vector3 GenerateRandomPointAtCollider(Collider c)
    {
        Vector3 center = c.bounds.center;
        Vector3 size = new Vector3(c.bounds.size.x, c.bounds.size.y, c.bounds.size.z);
        float x = center.x + Random.Range(-size.x / 2, size.x / 2);
        float y = center.y + Random.Range(-size.y / 2, size.y / 2);
        float z = center.z + Random.Range(-size.z / 2, size.z / 2);
        return new Vector3(x, y, z);
    }
}
