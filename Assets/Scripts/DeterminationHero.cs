using UnityEngine;

public class DeterminationHero : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackCooldown = 1.2f;

    [Header("Melee Detection Zone")]
    public Vector2 boxSize = new Vector2(2.0f, 1.5f);
    public Vector2 boxOffset = new Vector2(1.2f, 0f);

    [Header("Projectile & Animation")]
    public GameObject punchPrefab;
    public Animator animator;

    private float nextAttackTime = 0f;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (IsMonsterInMeleeRange())
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    bool IsMonsterInMeleeRange()
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

            // 2. EXCLUDE FRIENDLY HEROES (Prevents attacking allies in front)
            if (hitObj.name.Contains("Bravery") || 
                hitObj.name.Contains("Kindness") || 
                hitObj.name.Contains("Happiness") || 
                hitObj.name.Contains("Determination") || 
                hitObj.name.Contains("Patience") || 
                hitObj.name.Contains("Honesty") ||
                hitObj.name.Contains("Card"))
            {
                continue;
            }

            // 3. Check if target is a valid Monster with a Health component
            Health enemyHealth = hitObj.GetComponentInParent<Health>();
            if (enemyHealth != null)
            {
                return true; // Target is a confirmed monster!
            }
        }

        return false;
    }

    void Attack()
    {
        if (animator != null) animator.SetTrigger("Attack");

        Vector3 spawnPos = transform.position + (Vector3)boxOffset;
        if (punchPrefab != null)
        {
            Instantiate(punchPrefab, spawnPos, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position + (Vector3)boxOffset;
        Gizmos.DrawWireCube(center, boxSize);
    }
}