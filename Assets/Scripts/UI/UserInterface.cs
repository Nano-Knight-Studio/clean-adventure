using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour
{
    [SerializeField] Event collectGoalEvent;
    public static UserInterface instance;
    private bool collectedGoal = false;
    private int currentLevel = 0;

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

    public void CollectGoal ()
    {
        collectedGoal = true;
    }
}
