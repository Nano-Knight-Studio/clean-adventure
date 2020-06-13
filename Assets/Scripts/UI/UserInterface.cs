using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour
{
    public static UserInterface instance;
    private bool collectedGoal = false;
    private int currentLevel = 0;
    [SerializeField] private GameObject beforeGoalBarriers;
    [SerializeField] private GameObject afterGoalBarriers;
    [SerializeField] private RenderTexture damageIndicatorsRenderTexture;

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

    void Start ()
    {
        beforeGoalBarriers.SetActive(true);
        afterGoalBarriers.SetActive(false);

        RefreshRenderTextures();
    }

    public void CollectGoal ()
    {
        collectedGoal = true;
        beforeGoalBarriers.SetActive(false);
        afterGoalBarriers.SetActive(true);
    }

    void RefreshRenderTextures()
    {
        damageIndicatorsRenderTexture.width = Screen.width;
        damageIndicatorsRenderTexture.height = Screen.height;
    }
}
