using UnityEngine;
using System.Collections;

public class PlayerSkills : MonoBehaviour
{
    [Header("Skill Status")]
    public bool hasPangolinRoll = false;
    public bool isRolling = false;
    public bool isKnockedBack = false;

    [Header("Roll Settings")]
    public float rollSpeed = 15f;
    public float rollDuration = 0.6f;

    [Header("References")]
    public CharacterController controller;
    public TrailRenderer rollTrail; 
    public Animator anim; 

    private float normalHeight;
    private Vector3 normalCenter;
    private Coroutine currentKnockback; 
    
    // Universal trigger for UI and Hardware
    private bool rollRequested = false;

    void Start()
    {
        if (controller == null) controller = GetComponent<CharacterController>();
        if (anim == null) anim = GetComponentInChildren<Animator>(); 
        
        normalHeight = controller.height;
        normalCenter = controller.center;

        if (rollTrail != null) rollTrail.emitting = false;
    }

    public void UnlockPangolinRoll()
    {
        hasPangolinRoll = true;
        Debug.Log("Pangolin Skill Unlocked!");
    }

    // 🟢 CALL THIS FROM YOUR UI BUTTON EVENT TRIGGER
    public void TriggerRoll()
    {
        rollRequested = true;
    }

    public void TakeKnockback(Vector3 direction, float force)
    {
        if (isRolling) return; 

        if (currentKnockback != null) StopCoroutine(currentKnockback);
        
        currentKnockback = StartCoroutine(KnockbackRoutine(direction, force));
    }

    private IEnumerator KnockbackRoutine(Vector3 direction, float force)
    {
        isKnockedBack = true;
        float knockbackTime = 0.2f; 
        float timer = 0f;

        while (timer < knockbackTime)
        {
            controller.Move(direction * force * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        
        isKnockedBack = false;
    }

    void Update()
    {
        // Check Hardware (Left Shift OR Gamepad Button)
        if (Input.GetButtonDown("Fire3")) 
        {
            rollRequested = true;
        }

        // Execute Roll
        if (hasPangolinRoll && rollRequested && !isRolling && !isKnockedBack)
        {
            StartCoroutine(PerformRoll());
        }

        // Reset trigger every frame
        rollRequested = false;
    }

    IEnumerator PerformRoll()
    {
        isRolling = true;
        
        if (anim != null) anim.SetBool("isRolling", true);
        if (rollTrail != null) rollTrail.emitting = true;

        controller.height = normalHeight / 3f;
        controller.center = new Vector3(normalCenter.x, normalCenter.y / 3f, normalCenter.z);

        float startTime = Time.time;
        while (Time.time < startTime + rollDuration)
        {
            controller.Move(transform.forward * rollSpeed * Time.deltaTime);
            yield return null; 
        }

        controller.height = normalHeight;
        controller.center = normalCenter;
        
        if (anim != null) anim.SetBool("isRolling", false); 
        if (rollTrail != null) rollTrail.emitting = false;
        
        isRolling = false;
    }
}