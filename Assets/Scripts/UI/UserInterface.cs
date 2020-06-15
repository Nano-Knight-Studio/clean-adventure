using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class UserInterface : MonoBehaviour
{
    public static UserInterface instance;
    private bool collectedGoal = false;
    private int currentLevel = 0;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject cameraCenter;
    [SerializeField] private Transform housePosition;
    [SerializeField] private GameObject beforeGoalBarriers;
    [SerializeField] private GameObject afterGoalBarriers;
    [SerializeField] private PostProcessLayer postProcessingLayer;
    [SerializeField] private GameObject mobileControls;
    [SerializeField] private Terrain terrain;
    [Header("Settings Menu")]
    [SerializeField] private Toggle audioToggle;
    [SerializeField] private Toggle effectsToggle;
    [SerializeField] private Toggle controlsToggle;
    [Header("Songs")]
    [SerializeField] private GameObject mainMenuSong;
    [SerializeField] private GameObject inGameSong;

    void Awake ()
    {
        instance = this;
    }

    void Start ()
    {
        beforeGoalBarriers.SetActive(true);
        afterGoalBarriers.SetActive(false);

        // Getting settings from playerprefs
        RecoverSettings();
    }

    void RecoverSettings ()
    {
        if (PlayerPrefs.GetInt("EffectsOff") == 1)
        {
            effectsToggle.isOn = false;
        }
        if (PlayerPrefs.GetInt("AudioOff") == 1)
        {
            audioToggle.isOn = false;
        }
        if (PlayerPrefs.GetInt("ControlsOff") == 1)
        {
            controlsToggle.isOn = false;
        }

        // Auto deactivate stuff depending on platform
        #if UNITY_ANDROID
            SwitchEffects(false);
            SetGrassDensity(0.3f);
        #endif
        #if UNITY_IOS
            SwitchEffects(false);
            SetGrassDensity(0.3f);
        #endif
        #if UNITY_STANDALONE
            SwitchControls(false);
            SetGrassDensity(1.0f);
        #endif
    }

    public void StartGame ()
    {
        player.GetComponent<PlayerMovement>().enabled = true;
        player.GetComponent<PlayerShooting>().enabled = true;
        player.GetComponent<PlayerNavigation>().enabled = true;
        cameraCenter.GetComponent<CameraBehaviour>().enabled = true;
        EnemyGlobalSettings.SetGameplayActive(true);
        mainMenuSong.SetActive(false);
        inGameSong.SetActive(true);
    }

    public void CollectGoal ()
    {
        collectedGoal = true;
        beforeGoalBarriers.SetActive(false);
        afterGoalBarriers.SetActive(true);
        player.GetComponent<PlayerNavigation>().target = housePosition;
    }

    #region PAUSE

    public void Pause ()
    {
        Time.timeScale = 0.0f;
    }

    public void Resume ()
    {
        Time.timeScale = 1.0f;
    }

    public void Home ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Resume();
    }

    #endregion

    #region SETTINGS

    public void SwitchAudio(bool activate)
    {
        PlayerPrefs.SetInt("AudioOff", activate ? 0 : 1);
        AudioListener.volume = activate ? 1 : 0;
    }

    public void SwitchEffects(bool activate)
    {
        PlayerPrefs.SetInt("EffectsOff", activate ? 0 : 1);
        postProcessingLayer.enabled = activate;
    }

    public void SwitchControls(bool activate)
    {
        PlayerPrefs.SetInt("ControlsOff", activate ? 0 : 1);
        mobileControls.SetActive(activate);
    }

    public void SetGrassDensity(float density)
    {
        terrain.detailObjectDensity = density;
    }

    #endregion
}
