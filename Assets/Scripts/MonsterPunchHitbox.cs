using UnityEngine;

public class MonsterPunchHitbox : MonoBehaviour
{
    [Header("Punch Settings")]
    public int damage = 15;
    public float lifetime = 1.0f;       // Extended lifetime for debugging
    public float moveSpeed = 150f;      // Moves LEFT towards the hero (set to 0 if static)

    private SpriteRenderer sr;

    void Start()
    {
        Debug.Log($"<color=cyan>[Punch Debug] Spawned at World Position: {transform.position}</color>");

        // 1. Force visual visibility for debugging (Bright Red Box)
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = true;
            sr.color = new Color(1f, 0f, 0f, 0.65f); // Semi-transparent red
        }

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // 2. Drive the punch leftward toward the hero
        if (moveSpeed > 0)
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject hitObj = other.gameObject;
        Debug.Log($"[Punch Debug] Touched Collider: '{hitObj.name}' at position {transform.position}");

        // Silently ignore monsters, projectiles, cards, and grid tiles
        if (hitObj.name.Contains("Burnout") || hitObj.name.Contains("Sadness") ||
            hitObj.name.Contains("Distraction") || hitObj.name.Contains("Anxiety") ||
            hitObj.name.Contains("Hopelessness") || hitObj.name.Contains("Punch") ||
            hitObj.name.Contains("Water") || hitObj.name.Contains("Fireball") ||
            hitObj.name.Contains("Leaf") || hitObj.name.Contains("Tile") || 
            hitObj.name.Contains("Card"))
        {
            return;
        }

        // Search for Health script on touched hero or parent
        Health heroHealth = hitObj.GetComponentInParent<Health>();

        if (heroHealth != null)
        {
            Debug.Log($"<color=green>[Punch Debug] SUCCESS! Dealt {damage} damage to {heroHealth.gameObject.name}</color>");
            heroHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning($"[Punch Debug] Touched '{hitObj.name}', but couldn't find Health script!");
        }
    }

    private void OnDestroy()
    {
        Debug.Log($"[Punch Debug] Destroyed at Position: {transform.position}");
    }

    private void OnDrawGizmos()
    {
        // Highlight active physics hitbox in Scene view
        Gizmos.color = Color.magenta;
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)col.offset, col.size);
        }
    }
}