using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public Animator anim;
    public Transform attackPoint;    
    public LayerMask enemyLayer;     

    [Header("Combat Settings")]
    public float attackRange = 1.2f;
    public int attackDamage = 1;
    public float comboResetTime = 1.0f; 
    public float attackCooldown = 0.4f; 

    private int comboStep = 0;
    private float lastAttackTime = 0f;

    // 🟢 CALL THIS FROM YOUR UI BUTTON
    public void TriggerAttack()
    {
        // Check if the cooldown has passed before executing the punch
        if (Time.time - lastAttackTime > attackCooldown)
        {
            PerformAttack();
        }
    }

    void Update()
    {
        // Reset the combo back to zero if we stop punching for too long
        if (Time.time - lastAttackTime > comboResetTime)
        {
            comboStep = 0;
        }

        // Hardware inputs (Mouse/Gamepad) have been completely removed!
    }

    void PerformAttack()
    {
        lastAttackTime = Time.time;
        comboStep++;

        // Clear old triggers so they don't queue up
        anim.ResetTrigger("Punch1");
        anim.ResetTrigger("Punch2");
        anim.ResetTrigger("Punch3");

        // Trigger the correct animation in the chain
        if (comboStep == 1) 
        {
            anim.SetTrigger("Punch1");
        }
        else if (comboStep == 2) 
        {
            anim.SetTrigger("Punch2");
        }
        else if (comboStep >= 3) 
        {
            anim.SetTrigger("Punch3");
            comboStep = 0; // Reset back to zero after the heavy 3rd hit
        }

        // The Invisible Hitbox
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

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}