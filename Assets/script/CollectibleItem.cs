using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public float spinSpeed = 100f;
    public float floatAmplitude = 0.5f; // How high it bobs
    public float floatFrequency = 2f;   // How fast it bobs

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position; // Remember where we placed it
    }

    void Update()
    {
        // 1. Spin the object
        transform.Rotate(0, spinSpeed * Time.deltaTime, 0);

        // 2. Float the object up and down using a Sine wave
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    void OnTriggerEnter(Collider other)
    {
        // If the object that touched this has the "Player" tag...
        if (other.CompareTag("Player"))
        {
            // We will add the score logic here in Step 3!
            other.GetComponent<PlayerInventory>().AddBioData(1);
            
            // Destroy the collectible so it disappears
            Destroy(gameObject);
        }
    }
}