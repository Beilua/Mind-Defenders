using UnityEngine;

public class HonestyBeam : MonoBehaviour
{
    public int damage = 10;
    public float moveSpeed = 300f;        // Speed toward the right
    public float slowMultiplier = 0.2f;   // Set to 0.2 (80% slow) for testing
    public float slowDuration = 4.0f;     // Slow duration in seconds
    public float lifetime = 3.0f;

    void Start()
    {
        Debug.Log($"<color=cyan>[Beam Debug] Beam spawned at position: {transform.position}</color>");
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject hitObj = other.gameObject;
        Debug.Log($"[Beam Debug] Touched collider: '{hitObj.name}'");

        // 1. IGNORE Self, Heroes, UI Cards, Tiles, and other projectiles
        if (hitObj == gameObject || hitObj.transform.IsChildOf(transform) ||
            hitObj.name.Contains("Honesty") || hitObj.name.Contains("Bravery") ||
            hitObj.name.Contains("Kindness") || hitObj.name.Contains("Happiness") ||
            hitObj.name.Contains("Determination") || hitObj.name.Contains("Patience") ||
            hitObj.name.Contains("Card") || hitObj.name.Contains("Tile") || 
            hitObj.name.Contains("Punch") || hitObj.name.Contains("Beam"))
        {
            return; // Exit silently
        }

        // 2. Search for Health component
        Health enemyHealth = hitObj.GetComponentInParent<Health>();

        if (enemyHealth != null)
        {
            GameObject monsterObj = enemyHealth.gameObject;

            // Confirm target is a monster
            if (monsterObj.name.Contains("Burnout") || monsterObj.name.Contains("Sadness") ||
                monsterObj.name.Contains("Anxiety") || monsterObj.name.Contains("Hopelessness") ||
                monsterObj.name.Contains("Distraction"))
            {
                Debug.Log($"<color=green>[Beam Debug] SUCCESS! Hit {monsterObj.name}. Applying Slow & Damage.</color>");

                // Apply Slow Debuff
                SlowDebuff debuff = monsterObj.GetComponent<SlowDebuff>();
                if (debuff == null)
                {
                    debuff = monsterObj.AddComponent<SlowDebuff>();
                }
                debuff.ApplySlow(slowMultiplier, slowDuration);

                // Apply Damage
                enemyHealth.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}