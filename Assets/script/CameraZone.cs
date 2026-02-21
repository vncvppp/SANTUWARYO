using UnityEngine;
using Unity.Cinemachine; // This is the fix for your "Invalid Script" error!

public class CameraZone : MonoBehaviour
{
    [Header("The Camera for this specific area")]
    public CinemachineCamera targetCamera; // Assign the camera here

    // When Player WALKS IN
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Make this camera the "King"
            targetCamera.Priority = 20; 
        }
    }

    // When Player WALKS OUT
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Dethrone this camera (It reverts to the default one automatically)
            targetCamera.Priority = 0;
        }
    }
}