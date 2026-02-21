using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required to talk to UI Buttons!

public class GameManager : MonoBehaviour
{
    [Header("Menu Buttons")]
    public Button loadGameButton; // We need this reference to gray it out

    [Header("Menu Panels (For Later)")]
    public GameObject bioDexPanel; 
    public GameObject creditsPanel; 

    void Start()
    {
        // 1. Gray out the Load Game button automatically!
        if (loadGameButton != null)
        {
            loadGameButton.interactable = false;
        }

        // 2. Make sure our pop-up panels are hidden when the menu starts
        if (bioDexPanel != null) bioDexPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    // Connect this to the NEW GAME button
    public void StartNewGame()
    {
        Debug.Log("Starting Santuwaryo...");
        // Loads the actual gameplay scene (Make sure your jungle scene is Index 1 in Build Settings!)
        SceneManager.LoadScene(1);
    }

    // Connect this to the BIO-DEX button
    public void OpenBioDex()
    {
        Debug.Log("Opening Bio-Dex...");
        // if (bioDexPanel != null) bioDexPanel.SetActive(true);
    }

    // Connect this to the CREDITS button
    public void OpenCredits()
    {
        Debug.Log("Opening Credits...");
        // if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    // Optional: Connect this to a Quit button if you make one
    public void QuitGame()
    {
        Debug.Log("Game Quitting!");
        Application.Quit();
    }
}