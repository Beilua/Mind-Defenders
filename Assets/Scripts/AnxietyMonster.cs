using System.Collections;
using UnityEngine;

public class AnxietyMonster : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 0.8f;             // Slightly faster suicide-runner speed

    [Header("Explosion Settings")]
    public GameObject invisiblePunchPrefab;    // Reused Punch/Hitbox Prefab
    public Transform attackPoint;             // Spawn point for explosion damage
    public float explosionDelay = 0.35f;       // Delay to sync damage with burst sprite frame

    [Header("Melee Detection Zone")]
    public Vector2 boxSize = new Vector2(60f, 60f);
    public Vector2 boxOffset = new Vector2(-110f, -15f); // Negative X to detect heroes on the LEFT

    private Animator animator;
    private bool hasTriggeredExplosion = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (attackPoint == null) attackPoint = transform;
    }

    void Update()
    {
        if (hasTriggeredExplosion) return;

        if (IsHeroInMeleeRange())
        {
            StartCoroutine(ExplodeSequence());
        }
        else
        {
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

            // Silently skip self, child objects, tiles, cards, and projectiles
            if (hitObj == gameObject || hitObj.transform.IsChildOf(transform) || 
                hitObj.name.Contains("Punch") || hitObj.name.Contains("Fireball") || 
                hitObj.name.Contains("Leaf") || hitObj.name.Contains("Water") ||
                hitObj.name.Contains("Tile") || hitObj.name.Contains("Card"))
            {
                continue;
            }

            // Exclude fellow monsters
            if (hitObj.name.Contains("Distraction") || 
                hitObj.name.Contains("Sadness") || 
                hitObj.name.Contains("Burnout") || 
                hitObj.name.Contains("Anxiety") || 
                hitObj.name.Contains("Hopelessness"))
            {
                continue;
            }

            // Target confirmed if valid Hero with Health component is inside box
            Health heroHealth = hitObj.GetComponentInParent<Health>();
            if (heroHealth != null)
            {
                return true;
            }
        }

        return false;
    }

    void WalkForward()
    {
        if (animator != null) animator.SetBool("IsWalking", true);
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
    }

    IEnumerator ExplodeSequence()
    {
        hasTriggeredExplosion = true;

        // Stop movement and play self-destruct animation
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.SetTrigger("Attack");
        }

        // Wait for explosion frame impact
        yield return new WaitForSeconds(explosionDelay);

        Vector3 spawnPos = (attackPoint != null) ? attackPoint.position : (transform.position + (Vector3)boxOffset);

        if (invisiblePunchPrefab != null)
        {
            GameObject punch = Instantiate(invisiblePunchPrefab, spawnPos, Quaternion.identity);
            if (transform.parent != null)
            {
                punch.transform.SetParent(transform.parent, true); // Retain Canvas scaling if in UI
            }
        }

        // Instantly destroy Anxiety object upon exploding
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position + (Vector3)boxOffset;
        Gizmos.DrawWireCube(center, boxSize);
    }
}