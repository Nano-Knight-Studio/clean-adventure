using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Difficulty
{
    public GameObject[] enemies;
    public int density = 4;
}

public class EnemyGlobalSettings : MonoBehaviour
{
    public Difficulty[] difficulties;
    public int currentDifficulty = 0;
    public static EnemyGlobalSettings instance;

    void Awake ()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static int GetDensity()
    {
        return instance.difficulties[instance.currentDifficulty].density;
    }

    public static GameObject GetEnemy()
    {
        return instance.difficulties[instance.currentDifficulty].enemies[Random.Range(0, instance.difficulties[instance.currentDifficulty].enemies.Length)];
    }

    public static void NextLevel()
    {
        instance.currentDifficulty++;
        instance.currentDifficulty = Mathf.Clamp(instance.currentDifficulty, 0, instance.difficulties.Length -1);
    }
}