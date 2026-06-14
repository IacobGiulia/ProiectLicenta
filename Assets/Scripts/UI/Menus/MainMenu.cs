using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject howToPlayPanel;
    public GameObject mainMenuPanel;
    public GameObject creditsPanel;
    public void OpenHowToPlay()
    {

        howToPlayPanel.SetActive(true);
    }

    public void CloseHowToPlay()
    {
        howToPlayPanel.SetActive(false);
    }
    public void BackToMainMenu()
    {
        howToPlayPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
        
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
    }
    public void NewGame()
    {
        Time.timeScale = 1f;
        PlayerPrefs.DeleteKey("LoadGame");


        SceneManager.LoadScene("GymScene");
    }

    public void LoadGame()
    {
        Time.timeScale = 1f;

        PlayerPrefs.SetInt("LoadGame", 1);

        SceneManager.LoadScene("GymScene");
    }

}