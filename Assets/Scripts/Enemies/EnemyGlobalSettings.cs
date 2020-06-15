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
    public bool isGameplayActive = false;
    public static EnemyGlobalSettings instance;

    void Awake ()
    {
        instance = this;
    }

    public static void SetGameplayActive (bool active)
    {
        instance.isGameplayActive = active;
    }

    public static int GetDensity()
    {
        return instance.difficulties[instance.currentDifficulty].density;
    }

    public static GameObject GetEnemy()
    {
        return instance.difficulties[instance.currentDifficulty].enemies[Random.Range(0, instance.difficulties[instance.currentDifficulty].enemies.Length)];
    }

    public static int GetDifficulty()
    {
        return instance.currentDifficulty;
    }

    public static void NextLevel()
    {
        instance.currentDifficulty++;
        instance.currentDifficulty = Mathf.Clamp(instance.currentDifficulty, 0, instance.difficulties.Length -1);
    }
}