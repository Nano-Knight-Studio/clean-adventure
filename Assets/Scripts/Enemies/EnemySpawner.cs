using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float height;
    Collider[] colliders;
    private List<GameObject> enemies = new List<GameObject>();
    int currentDifficulty;

    void Start ()
    {
        colliders = GetComponentsInChildren<BoxCollider>();
        currentDifficulty = EnemyGlobalSettings.GetDifficulty();
        SpawnAll();
        StartCoroutine(Respawn());
    }

    public void SpawnAll()
    {
        CleanList();
        foreach(Collider c in colliders)
        {
            for (int i=enemies.Count; i < EnemyGlobalSettings.GetDensity(); i++)
            {
                Vector3 pos = GenerateRandomPointAtCollider(c);
                pos = new Vector3(pos.x, height, pos.z);
                enemies.Add(Instantiate(EnemyGlobalSettings.GetEnemy(), pos, Quaternion.identity));
            }
        }
    }

    public void RemoveAll()
    {
        CleanList();
        foreach (GameObject obj in enemies)
        {
            obj.GetComponent<Enemy>().Die(false, false);
        }
        CleanList();
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(Random.Range(3.0f, 6.0f));
        if (EnemyGlobalSettings.GetDifficulty() != currentDifficulty)
        {
            currentDifficulty = EnemyGlobalSettings.GetDifficulty();
            RemoveAll();
            SpawnAll();
        }
        else if (Vector3.Distance(transform.position, PlayerMovement.instance.transform.position) > 3.0f)
        {
            SpawnAll();
        }
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
