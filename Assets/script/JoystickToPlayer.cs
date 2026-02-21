using UnityEngine;
// If the script can't find 'VariableJoystick', make sure the Joystick Pack is imported.

public class JoystickToPlayer : MonoBehaviour
{
    public PlayerController player; // Drag Elara here in the Inspector
    public VariableJoystick joystick; // Drag your UI Joystick here in the Inspector

    void Update()
    {
        if (player != null && joystick != null)
        {
            // Push values into your PlayerController variables
            player.uiInputX = joystick.Horizontal;
            player.uiInputZ = joystick.Vertical;
        }
    }
}