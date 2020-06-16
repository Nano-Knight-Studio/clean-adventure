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
    [SerializeField] public RenderTexture fakeRender;
    [SerializeField] private GameObject fakeRenderImage;
    [Header("Player Stats")]
    [SerializeField] private StatIndicator healthStat;
    [SerializeField] private StatIndicator ammoStat;
    [Header("Settings Menu")]
    [SerializeField] private Toggle audioToggle;
    [SerializeField] private Toggle effectsToggle;
    [SerializeField] private Toggle controlsToggle;
    [SerializeField] private Toggle pixelEffectToggle;
    [Header("Songs")]
    [SerializeField] private GameObject mainMenuSong;
    [SerializeField] private GameObject inGameSong;
    [HideInInspector] public float customRenderScale = 1.0f;

    void Awake ()
    {
        instance = this;
        customRenderScale = 1.0f;
    }

    void Start ()
    {
        beforeGoalBarriers.SetActive(true);
        afterGoalBarriers.SetActive(false);

        // Getting settings from playerprefs
        RecoverSettings();
    }

    IEnumerator RefreshRenderTexture ()
    {
        fakeRender.width = (int) ((float)fakeRender.height/(float)Screen.height * (float)Screen.width);
		cameraCenter.GetComponentInChildren<Camera>().aspect = (float)fakeRender.width/(float)fakeRender.height;
        if (fakeRenderImage.activeSelf) customRenderScale = (float)fakeRender.width / (float)Screen.width;
        else customRenderScale = 1.0f;
        yield return new WaitForSeconds(10.0f);
        StartCoroutine(RefreshRenderTexture());
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
        if (PlayerPrefs.GetInt("PixelEffect") == 1)
        {
            pixelEffectToggle.isOn = true;
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

    public void RefreshPlayerStats()
    {
        healthStat.SetPercentage(player.GetComponent<PlayerStats>().GetHealthPercentage());
        ammoStat.SetPercentage(player.GetComponent<PlayerShooting>().GetAmmoPercentage());
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
        if (activate)
        {
            customRenderScale = 1.0f;
        }
        if (pixelEffectToggle.isOn && activate)
        {
            pixelEffectToggle.isOn = false;
        }
    }

    public void SwitchControls(bool activate)
    {
        PlayerPrefs.SetInt("ControlsOff", activate ? 0 : 1);
        mobileControls.SetActive(activate);
    }

    public void SwitchPixelEffect(bool activate)
    {
        PlayerPrefs.SetInt("PixelEffect", activate ? 1 : 0);
        if (activate)
        {
            bool wasOn = PlayerPrefs.GetInt("EffectsOff") == 1 ? false : true;
            effectsToggle.isOn = false;
            if (wasOn)
            {
                PlayerPrefs.SetInt("EffectsOff", 0);
            }
        }
        else
        {
            effectsToggle.isOn = PlayerPrefs.GetInt("EffectsOff") == 1 ? false : true;
        }
        if (activate)
        {
            StartCoroutine(RefreshRenderTexture());
            fakeRenderImage.SetActive(true);
            cameraCenter.GetComponentInChildren<Camera>().targetTexture = fakeRender;
        }
        else
        {
            StopCoroutine(RefreshRenderTexture());
            fakeRenderImage.SetActive(false);
            cameraCenter.GetComponentInChildren<Camera>().targetTexture = null;
        }
    }

    public void SetGrassDensity(float density)
    {
        terrain.detailObjectDensity = density;
    }

    #endregion
}