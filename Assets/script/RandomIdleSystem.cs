using UnityEngine;

public class NewIdleSystem : MonoBehaviour
{
    [Header("Setup")]
    public Animator animator;

    [Header("Settings")]
    public float timeToBecomeWaiting = 5f; // Time to switch from Normal -> Waiting
    public float minBoredTime = 5f;        // Time between bored actions
    public float maxBoredTime = 10f;

    // State Tracking
    private float mainTimer;   // Counts up to 5s
    private float boredTimer;  // Counts up for random actions
    private float nextBoredGoal;
    private bool isInWaitingMode = false;

    void Start()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        ResetSystem();
    }

    void Update()
    {
        if (animator == null) return;

        // 1. RESET if moving
        if (animator.GetBool("isRunning") == true)
        {
            ResetSystem();
            return;
        }

        // 2. Logic Flow
        if (!isInWaitingMode)
        {
            // --- STAGE 1: Counting down to "Waiting Mode" ---
            mainTimer += Time.deltaTime;

            if (mainTimer >= timeToBecomeWaiting)
            {
                EnterWaitingMode();
            }
        }
        else
        {
            // --- STAGE 2: We are now in "Waiting Mode" (New Idle) ---
            // Count down to random bored animations
            boredTimer += Time.deltaTime;

            if (boredTimer >= nextBoredGoal)
            {
                PlayRandomBoredom();
            }
        }
    }

    void EnterWaitingMode()
    {
        Debug.Log("💤 Switching to Long Idle (Waiting Mode)");
        isInWaitingMode = true;
        
        // Switch the Animator State
        animator.SetBool("isLongIdle", true);
        
        // Reset bored timer so she doesn't act immediately
        boredTimer = 0;
        nextBoredGoal = Random.Range(minBoredTime, maxBoredTime);
    }

    void PlayRandomBoredom()
    {
        // Pick Bored 1 (ID 2) or Bored 2 (ID 3)
        int randomID = Random.Range(2, 6);
        Debug.Log("🥱 Bored Action: " + randomID);

        // Trigger the animation
        animator.SetInteger("IdleID", randomID);
        animator.SetTrigger("TriggerIdle");

        // Reset the timer for the NEXT bored action
        boredTimer = 0;
        nextBoredGoal = Random.Range(minBoredTime, maxBoredTime);
    }

    void ResetSystem()
    {
        // Only reset if needed (Optimization)
        if (!isInWaitingMode && mainTimer == 0) return;

        isInWaitingMode = false;
        mainTimer = 0;
        boredTimer = 0;

        // Tell Animator to go back to Main Breathing Idle
        animator.SetBool("isLongIdle", false);
    }
}