using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public GameObject settingsPanel;

    public ThirdPersonCamera cameraController;

    private float defaultSensitivity = 3f;

    void Start()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("MasterVolume");
            AudioListener.volume = savedVolume;
        }     
        else
            AudioListener.volume = 1f;

        float sens = PlayerPrefs.HasKey("Sensitivity")
            ? PlayerPrefs.GetFloat("Sensitivity")
            : defaultSensitivity;

        if (PlayerPrefs.HasKey("Fullscreen"))
            Screen.fullScreen = PlayerPrefs.GetInt("Fullscreen") == 1;
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetSensitivity(float value)
    {

        PlayerPrefs.SetFloat("Sensitivity", value);

        if (cameraController != null)
            cameraController.SetSensitivity(value);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        if (isFullscreen)
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        else
            Screen.fullScreenMode = FullScreenMode.Windowed;

        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }
}