using UnityEngine;
using UnityEngine.UI; // Needed to talk to the UI Images

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("UI References")]
    public Image[] heartIcons; // Drag your 3 Heart images here in the Inspector
    public Sprite fullHeart;   // Drag your red heart sprite here
    public Sprite emptyHeart;  // Drag your grey/empty heart sprite here

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHearts();
    }

    // Call this when a drone hits Elara!
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (currentHealth < 0) currentHealth = 0;
        
        UpdateHearts();

        if (currentHealth == 0)
        {
            Debug.Log("Elara passed out! Restarting at checkpoint...");
            // You can add your respawn logic here later!
        }
    }

    void UpdateHearts()
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            // If the heart index is less than our current health, make it full. Otherwise, empty.
            if (i < currentHealth)
                heartIcons[i].sprite = fullHeart;
            else
                heartIcons[i].sprite = emptyHeart;
        }
    }
}