using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("⚙️ Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSmoothTime = 0.1f;
    public float jumpHeight = 3f;
    public float gravity = -25f; 
    public float fallMultiplier = 2.0f; 

    [Header("⚔️ Combat Settings")]
    public Transform attackPoint;    
    public LayerMask enemyLayer;     
    public float attackRange = 1.2f;
    public int attackDamage = 1;
    public float comboResetTime = 1.0f; 
    public float attackCooldown = 0.4f;

    [Header("🔗 References")]
    public Animator animator;      
    public Transform cameraTransform; 

    // UI Joystick Input
    [HideInInspector] public float uiInputX = 0f;
    [HideInInspector] public float uiInputZ = 0f;

    private CharacterController controller;
    private Vector3 velocity;
    private float turnSmoothVelocity;
    private bool isGrounded;
    
    // Universal triggers for UI and Hardware
    private bool jumpRequested = false; 
    private bool attackRequested = false;

    // Combat State Trackers
    private int comboStep = 0;
    private float lastAttackTime = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    // 🟢 CALL THESE FROM YOUR UI BUTTON EVENT TRIGGERS
    public void TriggerJump() { jumpRequested = true; }
    public void TriggerAttack() { attackRequested = true; }

    void Update()
    {
        // ==========================================
        // 1. GROUND CHECK
        // ==========================================
        isGrounded = controller.isGrounded;
        if (animator != null) animator.SetBool("isGrounded", isGrounded);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        // ==========================================
        // 2. MOVEMENT LOGIC
        // ==========================================
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        if (x == 0 && z == 0)
        {
            x = uiInputX;
            z = uiInputZ;
        }

        Vector3 inputDir = new Vector3(x, 0f, z).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
            
            if(animator != null) animator.SetBool("isRunning", true);
        }
        else
        {
            if(animator != null) animator.SetBool("isRunning", false);
        }   

        // ==========================================
        // 3. COMBAT LOGIC
        // ==========================================
        // Reset combo if we wait too long
        if (Time.time - lastAttackTime > comboResetTime)
        {
            comboStep = 0;
        }

        // Hardware Input: Left Click or Gamepad 'X/Square' (Fire1)
        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire1"))
        {
            attackRequested = true;
        }

        // Execute Attack
        if (attackRequested && (Time.time - lastAttackTime > attackCooldown))
        {
            PerformAttack();
        }

        // ==========================================
        // 4. JUMP LOGIC
        // ==========================================
        if (Input.GetButtonDown("Jump")) 
        {
            jumpRequested = true;
        }

        if (jumpRequested && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (animator != null) animator.SetTrigger("Jump");
        }

        // Reset Universal Triggers every frame
        jumpRequested = false;
        attackRequested = false;

        // ==========================================
        // 5. GRAVITY LOGIC
        // ==========================================
        if (velocity.y < 0)
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        else
            velocity.y += gravity * Time.deltaTime;
        
        controller.Move(velocity * Time.deltaTime);
    }

    // ==========================================
    // COMBAT FUNCTIONS
    // ==========================================
    void PerformAttack()
    {
        lastAttackTime = Time.time;
        comboStep++;

        // Clear old triggers
        if (animator != null)
        {
            animator.ResetTrigger("Punch1");
            animator.ResetTrigger("Punch2");
            animator.ResetTrigger("Punch3");

            // Trigger correct animation
            if (comboStep == 1) animator.SetTrigger("Punch1");
            else if (comboStep == 2) animator.SetTrigger("Punch2");
            else if (comboStep >= 3) 
            {
                animator.SetTrigger("Punch3");
                comboStep = 0; 
            }
        }

        // Detect and Damage Enemies
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        foreach(Collider enemy in hitEnemies)
        {
            DroneEnemy drone = enemy.GetComponent<DroneEnemy>();
            if (drone != null)
            {
                drone.TakeDamage(attackDamage);
            }
        }
    }

    public void EnableHighJump()
    {
        jumpHeight = 6f; 
        Debug.Log("🐇 High Jump Ability Unlocked!");
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}