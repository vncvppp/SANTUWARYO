using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Drag your Player here
    public float smoothSpeed = 0.125f;
    public Vector3 offset; // Set this to (0, 10, -12) in Inspector

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        // The "Lerp" makes the camera movement smooth like Zelda
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        
        // Keep looking at player
        transform.LookAt(target);
    }
}