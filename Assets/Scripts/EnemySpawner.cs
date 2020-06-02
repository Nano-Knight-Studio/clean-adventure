using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] prefabs;
    public int density;
    public Transform streets;
    public float height;
    Collider[] streetColliders;

    void Start ()
    {
        streetColliders = streets.GetComponentsInChildren<BoxCollider>();
        SpawnAll();
    }

    void SpawnAll()
    {
        foreach(Collider c in streetColliders)
        {
            for (int i=0; i < density; i++)
            {
                Vector3 pos = GenerateRandomPointAtCollider(c);
                pos = new Vector3(pos.x, height, pos.z);
                Instantiate(prefabs[Random.Range(0, prefabs.Length)], pos, Quaternion.identity);
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
