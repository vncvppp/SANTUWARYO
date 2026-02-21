using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // =========================================================
    // ⚙️ MOVEMENT SETTINGS
    // =========================================================
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSmoothTime = 0.1f;
    public float jumpHeight = 3f;
    public float gravity = -25f;
    public float fallMultiplier = 2.0f;

    // =========================================================
    // ⚔️ COMBAT SETTINGS
    // =========================================================
    [Header("Combat Settings")]
    public Transform attackPoint;
    public LayerMask enemyLayer;
    public float attackRange = 1.2f;
    public int attackDamage = 1;
    public float comboResetTime = 1.0f;
    public float attackCooldown = 0.4f;

    // =========================================================
    // 🔗 REFERENCES
    // =========================================================
    [Header("References")]
    public Animator animator;
    public Transform cameraTransform;

    // =========================================================
    // UI INPUT (Joystick)
    // =========================================================
    [HideInInspector] public float uiInputX = 0f;
    [HideInInspector] public float uiInputZ = 0f;

    // =========================================================
    // PRIVATE VARIABLES
    // =========================================================
    private CharacterController controller;
    private Vector3 velocity;
    private float turnSmoothVelocity;
    private bool isGrounded;

    // Universal triggers (UI + Hardware)
    private bool jumpRequested = false;
    private bool attackRequested = false;

    // Combat trackers
    private int comboStep = 0;
    private float lastAttackTime = 0f;

    // =========================================================
    // INITIALIZATION
    // =========================================================
    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    // =========================================================
    // UI BUTTON CALLS
    // =========================================================
    public void TriggerJump() => jumpRequested = true;
    public void TriggerAttack() => attackRequested = true;

    // =========================================================
    // UPDATE LOOP
    // =========================================================
    void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleCombat();
        HandleJump();
        ApplyGravity();
    }

    // =========================================================
    // GROUND CHECK
    // =========================================================
    void HandleGroundCheck()
    {
        isGrounded = controller.isGrounded;

        if (animator != null)
            animator.SetBool("isGrounded", isGrounded);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    // =========================================================
    // MOVEMENT (Keyboard + Gamepad + UI Joystick)
    // =========================================================
    void HandleMovement()
    {
        float keyboardX = Input.GetAxisRaw("Horizontal");
        float keyboardZ = Input.GetAxisRaw("Vertical");

        // BETTER LOGIC: Use whichever input is stronger/active
        float x = Mathf.Abs(keyboardX) > 0.1f ? keyboardX : uiInputX;
        float z = Mathf.Abs(keyboardZ) > 0.1f ? keyboardZ : uiInputZ;

        Vector3 inputDir = new Vector3(x, 0f, z).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            float targetAngle =
                Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg +
                cameraTransform.eulerAngles.y;

            float angle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref turnSmoothVelocity,
                rotationSmoothTime
            );

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir =
                Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);

            if (animator != null)
                animator.SetBool("isRunning", true);
        }
        else
        {
            if (animator != null)
                animator.SetBool("isRunning", false);
        }
    }

    // =========================================================
    // COMBAT SYSTEM
    // =========================================================
    void HandleCombat()
    {
        // Reset combo if waited too long
        if (Time.time - lastAttackTime > comboResetTime)
            comboStep = 0;

        // Hardware Input
        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire1"))
            attackRequested = true;

        if (attackRequested && Time.time - lastAttackTime > attackCooldown)
            PerformAttack();

        attackRequested = false;
    }

    void PerformAttack()
    {
        lastAttackTime = Time.time;
        comboStep++;

        if (animator != null)
        {
            animator.ResetTrigger("Punch1");
            animator.ResetTrigger("Punch2");
            animator.ResetTrigger("Punch3");

            if (comboStep == 1)
                animator.SetTrigger("Punch1");
            else if (comboStep == 2)
                animator.SetTrigger("Punch2");
            else
            {
                animator.SetTrigger("Punch3");
                comboStep = 0;
            }
        }

        Collider[] hitEnemies =
            Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            DroneEnemy drone = enemy.GetComponent<DroneEnemy>();
            if (drone != null)
                drone.TakeDamage(attackDamage);
        }
    }

    // =========================================================
    // JUMP SYSTEM
    // =========================================================
    void HandleJump()
    {
        if (Input.GetButtonDown("Jump"))
            jumpRequested = true;

        if (jumpRequested && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (animator != null)
                animator.SetTrigger("Jump");
        }

        jumpRequested = false;
    }

    // =========================================================
    // GRAVITY
    // =========================================================
    void ApplyGravity()
    {
        if (velocity.y < 0)
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        else
            velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    // =========================================================
    // ABILITY
    // =========================================================
    public void EnableHighJump()
    {
        jumpHeight = 6f;
        Debug.Log("High Jump Ability Unlocked!");
    }

    // =========================================================
    // GIZMOS
    // =========================================================
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}