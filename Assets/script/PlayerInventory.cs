using UnityEngine;
using TMPro; // Required for modern Unity UI text

public class PlayerInventory : MonoBehaviour
{
    public int bioDataCount = 0;
    public TextMeshProUGUI scoreText; // Drag your UI text here in the Inspector

    public void AddBioData(int amount)
    {
        bioDataCount += amount;
        scoreText.text = "Bio-Data: " + bioDataCount; // Updates the screen
        Debug.Log("Collected! Total: " + bioDataCount);
    }
}