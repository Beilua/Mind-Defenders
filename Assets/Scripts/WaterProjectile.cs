using UnityEngine;

public class WaterProjectile : MonoBehaviour
{
    public float speed = 5f;
    public int healAmount = 25;
    public float lifetime = 5f;

    [HideInInspector] public GameObject caster; // Assigned automatically on spawn

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move right in World Space to ensure smooth movement under Canvas
        transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject hitObj = collision.gameObject;

        // 1. Ignore the caster (Patience) and all her child components (HealthBar, colliders, etc.)
        if (caster != null)
        {
            if (hitObj == caster || hitObj.transform.IsChildOf(caster.transform))
            {
                return;
            }
        }

        // 2. Ignore monsters completely (passes straight through them)
        if (hitObj.name.Contains("Distraction") || 
            hitObj.name.Contains("Sadness") || 
            hitObj.name.Contains("Anxiety") || 
            hitObj.name.Contains("Burnout") || 
            hitObj.name.Contains("Hopelessness"))
        {
            return; 
        }

        // 3. Ignore other projectiles
        if (hitObj.name.Contains("Water") || hitObj.name.Contains("Punch") || hitObj.name.Contains("Fireball"))
        {
            return;
        }

        // 4. Heal friendly heroes with a Health component
        Health heroHealth = hitObj.GetComponentInParent<Health>();
        if (heroHealth != null)
        {
            heroHealth.Heal(healAmount);
            Destroy(gameObject);
        }
    }
}