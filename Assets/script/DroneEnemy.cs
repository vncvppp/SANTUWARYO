using UnityEngine;

public class DroneEnemy : MonoBehaviour
{
    public enum DroneState { Patrol, Windup, Charging, Recoiling, Stunned, Cooldown, Rebooted }
    
    [Header("Current State")]
    public DroneState currentState = DroneState.Patrol;

    [Header("Procedural Animation")]
    public Transform droneVisuals; 
    public float hoverSpeed = 3f;
    public float hoverHeight = 0.5f;
    public float tiltAngle = 30f;  

    [Header("Patrol Settings")]
    public float patrolSpeed = 2f;
    public float patrolDistance = 3f;
    private Vector3 startPos;

    [Header("Combat Settings")]
    public int health = 3;
    public Transform player;
    public float detectRadius = 10f;
    public float chargeSpeed = 15f;
    public float knockbackForce = 15f;
    
    private Vector3 targetPosition;
    private Vector3 chargeDirection;
    private float stateTimer;

    [Header("Colors")]
    public MeshRenderer droneMesh; 
    public Color normalColor = Color.white;
    public Color warningColor = Color.red;
    public Color friendlyColor = Color.cyan;

    void Start()
    {
        startPos = transform.position;
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        if (droneMesh != null) droneMesh.material.color = normalColor;
    }

    void Update()
    {
        // 1. If it's rebooted, stop everything entirely.
        if (currentState == DroneState.Rebooted) return;

        // 2. PROCEDURAL ANIMATION
        if (droneVisuals != null)
        {
            float hoverY = Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
            Vector3 newPosition = new Vector3(0, hoverY, 0);

            if (currentState == DroneState.Windup)
            {
                newPosition += UnityEngine.Random.insideUnitSphere * 0.15f; // Shake!
            }

            droneVisuals.localPosition = newPosition;

            if (currentState == DroneState.Charging)
                droneVisuals.localRotation = Quaternion.Lerp(droneVisuals.localRotation, Quaternion.Euler(tiltAngle, 0, 0), Time.deltaTime * 10f);
            else 
                droneVisuals.localRotation = Quaternion.Lerp(droneVisuals.localRotation, Quaternion.identity, Time.deltaTime * 5f);
        }

        // 3. THE AI BRAIN
        switch (currentState)
        {
            case DroneState.Patrol:
                float offset = Mathf.Sin(Time.time * patrolSpeed) * patrolDistance;
                transform.position = startPos + (transform.right * offset);

                if (Vector3.Distance(transform.position, player.position) <= detectRadius)
                    ChangeState(DroneState.Windup);
                break;

            case DroneState.Windup:
                transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                {
                    targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
                    chargeDirection = (targetPosition - transform.position).normalized; 
                    ChangeState(DroneState.Charging);
                }
                break;

            case DroneState.Charging:
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, chargeSpeed * Time.deltaTime);
                
                stateTimer -= Time.deltaTime;
                if (Vector3.Distance(transform.position, targetPosition) < 0.1f || stateTimer <= 0)
                    ChangeState(DroneState.Cooldown);
                break;

            case DroneState.Recoiling:
                transform.position -= chargeDirection * (chargeSpeed / 2) * Time.deltaTime;
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0) ChangeState(DroneState.Cooldown);
                break;

            case DroneState.Stunned:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0) ChangeState(DroneState.Cooldown);
                break;

            case DroneState.Cooldown:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                {
                    startPos = transform.position;
                    ChangeState(DroneState.Patrol);
                }
                break;
        }
    }

    void ChangeState(DroneState newState)
    {
        currentState = newState;

        if (droneMesh != null)
        {
            if (newState == DroneState.Windup || newState == DroneState.Charging)
                droneMesh.material.color = warningColor;
            else if (newState != DroneState.Rebooted)
                droneMesh.material.color = normalColor;
        }

        if (newState == DroneState.Windup) stateTimer = 0.8f;     
        if (newState == DroneState.Charging) stateTimer = 1.5f;   
        if (newState == DroneState.Recoiling) stateTimer = 0.3f;  
        if (newState == DroneState.Stunned) stateTimer = 0.5f;   
        if (newState == DroneState.Cooldown) stateTimer = 1.5f;   
    }

    void OnTriggerEnter(Collider other)
    {
        if (currentState == DroneState.Charging && other.CompareTag("Player"))
        {
            PlayerSkills elara = other.GetComponent<PlayerSkills>();
            if (elara != null) elara.TakeKnockback(chargeDirection, knockbackForce);

            PlayerHealth elaraHealth = other.GetComponent<PlayerHealth>();
            if (elaraHealth != null) elaraHealth.TakeDamage(1);

            ChangeState(DroneState.Recoiling);
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentState == DroneState.Rebooted) return; 

        health -= damage;
        transform.position -= transform.forward * 1.5f;
        ChangeState(DroneState.Stunned);

        if (health <= 0)
        {
            RebootDrone();
        }
    }

    void RebootDrone()
    {
        currentState = DroneState.Rebooted;
        
        if (droneMesh != null) droneMesh.material.color = friendlyColor;
        
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }
}