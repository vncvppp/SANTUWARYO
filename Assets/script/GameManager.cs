using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class GameManager : MonoBehaviour
{
    [Header("Menu Buttons")]
    public Button loadGameButton; 

    [Header("Menu Panels")]
    public GameObject bioDexPanel; 
    public GameObject creditsPanel; 

    // Type the EXACT name of your gameplay scene in the Unity Inspector!
    [Header("Scene To Load")]
    public string gameSceneName = "MainGame"; 

    void Start()
    {
        // 1. Force the game to unpause just in case it got stuck
        Time.timeScale = 1f;

        // 2. Gray out the Load Game button
        if (loadGameButton != null)
        {
            loadGameButton.interactable = false;
        }

        // 3. Hide pop-up panels
        if (bioDexPanel != null) bioDexPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    public void StartNewGame()
    {
        Debug.Log("Starting Santuwaryo...");
        
        // CRITICAL FIX: Ensure time is moving before loading the scene
        Time.timeScale = 1f; 
        
        // Loads the exact scene name instead of relying on the Build Index number
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenBioDex()
    {
        Debug.Log("Opening Bio-Dex...");
        if (bioDexPanel != null) bioDexPanel.SetActive(true);
    }

    public void OpenCredits()
    {
        Debug.Log("Opening Credits...");
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Game Quitting!");
        Application.Quit();
    }
}