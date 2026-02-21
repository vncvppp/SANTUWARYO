using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Required to control UI

public class BioScanner : MonoBehaviour
{
    [Header("Scanner Settings")]
    public float scanRange = 10f;       // How far the laser shoots
    public float detectionRadius = 8f;  // How close you must be for button to appear
    
    [Header("References")]
    public LineRenderer laserLine;
    public GameObject scanButtonUI;     // Drag your UI Button object here!
    
    // Optimization: Only check for objects on this Layer
    public LayerMask scanLayer;

   void Update()
    {
        // 1. Proximity Check: Is there an animal nearby?
        CheckProximity();

        // 2. Keyboard Input (Keep this for testing)
        if (Input.GetKeyDown(KeyCode.E) && scanButtonUI.activeSelf) 
        {
            ActivateScan();
        }
    }

    void CheckProximity()
    {
        // Create an invisible sphere around player to check for colliders
        // We use the scanLayer to only look for "Animals" and ignore ground/walls
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, scanLayer);
        
        bool animalFound = false;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("AdaptationSource"))
            {
                animalFound = true;
                break; // We found one, no need to keep checking
            }
        }

        // Show/Hide the button based on result
        if (scanButtonUI != null)
        {
            scanButtonUI.SetActive(animalFound);
        }
    }

    public void ActivateScan()
    {
        StartCoroutine(ShootLaser());

        RaycastHit hit;
        // Check exact aim
        if (Physics.Raycast(transform.position, transform.forward, out hit, scanRange, scanLayer))
        {
            if (hit.collider.CompareTag("AdaptationSource"))
            {
                Debug.Log("Scanned: " + hit.collider.name);
                UnlockAbility(hit.collider.gameObject);
            }
        }
    }

    void UnlockAbility(GameObject animal)
    {
        // Check if the name CONTAINS "Frog" (Works for "Frog", "Green Frog", "Frog (1)")
        if (animal.name.Contains("Frog"))
        {
            // Try to find the PlayerController on THIS object (The Player)
            PlayerController player = GetComponent<PlayerController>();

            if (player != null) {
                player.EnableHighJump();
            }
            else {
                Debug.LogError("CRITICAL ERROR: No PlayerController found on this object!");
            }
        }
        else
        {
            Debug.Log("Scanned " + animal.name + " but it gives no ability.");
        }
    }

    IEnumerator ShootLaser()
    {
        if (laserLine != null)
        {
            laserLine.enabled = true;
            laserLine.SetPosition(0, transform.position);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, scanRange))
                laserLine.SetPosition(1, hit.point);
            else
                laserLine.SetPosition(1, transform.position + (transform.forward * scanRange));

            yield return new WaitForSeconds(0.1f);
            laserLine.enabled = false;
        }
    }
}