using UnityEngine;

public class AnimalScanner : MonoBehaviour
{
    private bool isPlayerNear = false;
    private PlayerSkills elaraSkills;

    [Header("Visual Cue")]
    public GameObject scanPromptUI; 

    [Header("Bio-Dex Settings")]
    public GameObject bioDexPanel; // Drag your Bio-Dex UI Panel here!
    
    // Universal trigger for UI/Keyboard/Gamepad
    private bool scanRequested = false;

    void Start()
    {
        if (scanPromptUI != null) scanPromptUI.SetActive(false);
        if (bioDexPanel != null) bioDexPanel.SetActive(false);
    }

    // 🟢 CALL THIS FROM YOUR UI BUTTON (Pointer Down event)
    public void TriggerScan()
    {
        scanRequested = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            elaraSkills = other.GetComponent<PlayerSkills>();
            if (scanPromptUI != null) scanPromptUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            elaraSkills = null;
            if (scanPromptUI != null) scanPromptUI.SetActive(false);
        }
    }

    void Update()
    {
        // Check for Keyboard (F) or Gamepad (Fire2 / B-Button)
        if (Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("Fire2"))
        {
            scanRequested = true;
        }

        if (isPlayerNear && scanRequested)
        {
            if (elaraSkills != null)
            {
                ExecuteScan();
            }
        }

        // Reset the trigger so it doesn't fire every frame
        scanRequested = false;
    }

    void ExecuteScan()
    {
        // 1. Unlock the Ability
        elaraSkills.UnlockPangolinRoll();
        
        // 2. Hide UI Prompts
        if (scanPromptUI != null) scanPromptUI.SetActive(false);

        // 3. Open the Bio-Dex (If assigned)
        if (bioDexPanel != null)
        {
            bioDexPanel.SetActive(true);
            Time.timeScale = 0f; // Pause the game so the player can read!
            Cursor.lockState = CursorLockMode.None; // Release mouse for UI
        }

        // 4. Remove the scanner (keep the animal model or destroy as needed)
        // Destroy(transform.parent.gameObject); 
        this.gameObject.SetActive(false); // Just disable the scan radius
    }
}