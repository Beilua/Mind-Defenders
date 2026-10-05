using System.Collections;
using UnityEngine;

public class BurnoutMonster : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 0.6f;

    [Header("Attack Settings")]
    public float attackCooldown = 1.5f;       // Time between attacks
    public GameObject invisiblePunchPrefab;    // Melee invisible punch projectile
    public Transform attackPoint;             // Spawn point for the punch

    [Header("Melee Detection Zone")]
    public Vector2 boxSize = new Vector2(60f, 60f);
    public Vector2 boxOffset = new Vector2(-110f, -15f); // Negative X offset to look LEFT

    private Animator animator;
    private float lastAttackTime;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (attackPoint == null) attackPoint = transform;
    }

    void Update()
    {
        if (IsHeroInMeleeRange())
        {
            // Hero detected inside red box: stop walking and attack/idle
            animator.SetBool("IsWalking", false);

            if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
            {
                StartCoroutine(PerformPunchAttack());
            }
        }
        else if (!isAttacking)
        {
            // No hero in box: continue walking left
            WalkForward();
        }
    }

    bool IsHeroInMeleeRange()
    {
        Vector2 boxCenter = (Vector2)transform.position + boxOffset;
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f);

        foreach (var col in hitColliders)
        {
            GameObject hitObj = col.gameObject;

            // 1. Ignore self, child UI (HealthBar), and projectiles
            if (hitObj == gameObject || hitObj.transform.IsChildOf(transform) || 
                hitObj.name.Contains("Punch") || hitObj.name.Contains("Fireball") || hitObj.name.Contains("Leaf"))
            {
                continue;
            }

            // 2. EXCLUDE FELLOW MONSTERS (Prevents stopping/attacking friendly monsters)
            if (hitObj.name.Contains("Distraction") || 
                hitObj.name.Contains("Sadness") || 
                hitObj.name.Contains("Burnout") || 
                hitObj.name.Contains("Anxiety") || 
                hitObj.name.Contains("Hopelessness"))
            {
                continue;
            }

            // 3. Check if target is a valid Hero with a Health component
            Health heroHealth = hitObj.GetComponentInParent<Health>();
            if (heroHealth != null)
            {
                return true; // Target is a confirmed hero!
            }
        }

        return false;
    }

    void WalkForward()
    {
        animator.SetBool("IsWalking", true);
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
    }

    IEnumerator PerformPunchAttack()
    {
        isAttacking = true;
        animator.SetTrigger("Fight");

        // Align hit timing with impact frame
        yield return new WaitForSeconds(0.25f);

        Vector3 spawnPos = (attackPoint != null) ? attackPoint.position : (transform.position + (Vector3)boxOffset);

        if (invisiblePunchPrefab != null)
        {
            Instantiate(invisiblePunchPrefab, spawnPos, Quaternion.identity);
        }

        lastAttackTime = Time.time;

        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position + (Vector3)boxOffset;
        Gizmos.DrawWireCube(center, boxSize);
    }
}