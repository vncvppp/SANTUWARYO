using UnityEngine;

public class AnimalScanner : MonoBehaviour
{
    private bool isPlayerNear = false;
    private PlayerSkills elaraSkills;

    [Header("Visual Cue")]
    public GameObject scanPromptUI; // This can be a text prompt OR a clickable UI Button!

    [Header("Bio-Dex Settings")]
    public GameObject bioDexPanel; 

    void Start()
    {
        if (scanPromptUI != null) scanPromptUI.SetActive(false);
        if (bioDexPanel != null) bioDexPanel.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            elaraSkills = other.GetComponent<PlayerSkills>();
            
            // Show the prompt/button when Elara walks into the radius
            if (scanPromptUI != null) scanPromptUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            elaraSkills = null;
            
            // Hide the prompt/button when she walks away
            if (scanPromptUI != null) scanPromptUI.SetActive(false);
        }
    }

    // 🟢 CALL THIS DIRECTLY FROM YOUR UI BUTTON
    public void TriggerScan()
    {
        // Safety check: Only scan if she is actually standing next to it
        if (isPlayerNear && elaraSkills != null)
        {
            ExecuteScan();
        }
    }

    void Update()
    {
        // Hardware Fallback: Check for Keyboard (F) or Gamepad (Fire2 / B-Button)
        if (isPlayerNear && (Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("Fire2")))
        {
            if (elaraSkills != null)
            {
                ExecuteScan();
            }
        }
    }

    void ExecuteScan()
    {
        // 1. Unlock the Ability
        elaraSkills.UnlockPangolinRoll();
        
        // 2. Hide UI Prompts
        if (scanPromptUI != null) scanPromptUI.SetActive(false);

        // 3. Open the Bio-Dex
        if (bioDexPanel != null)
        {
            bioDexPanel.SetActive(true);
            Time.timeScale = 0f; // Pause the game
            Cursor.lockState = CursorLockMode.None; // Free the mouse
        }

        // 4. Disable the scanner so you can't scan it twice
        this.gameObject.SetActive(false); 
    }
}