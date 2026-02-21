using UnityEngine;

public class LineSpawner : MonoBehaviour
{
    [Header("What to Spawn")]
    public GameObject pickupPrefab;

    [Header("Line Settings")]
    public int amountToSpawn = 5;
    public Vector3 distanceBetweenItems = new Vector3(2f, 0f, 0f);

    // This magic tag lets you run this code while NOT playing the game!
    [ContextMenu("Generate Line")]
    public void GenerateLine()
    {
        // First, clear any old ones if we are regenerating
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        if (pickupPrefab == null)
        {
            Debug.LogWarning("You forgot to assign the Prefab!");
            return;
        }

        // Spawn the new line
        for (int i = 0; i < amountToSpawn; i++)
        {
            Vector3 spawnPosition = transform.position + (distanceBetweenItems * i);
            
            // Create the object and make it a child of this Spawner
            GameObject newPickup = Instantiate(pickupPrefab, spawnPosition, Quaternion.identity, transform);
            newPickup.name = "BioData_" + i;
        }
    }
}