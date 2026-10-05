using UnityEngine;

public class PunchProjectile : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 25;
    public float lifetime = 0.4f; // Increased slightly so the punch animation is visible

    void Start()
    {
        // Destroy after lifetime if no hit occurs
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignore self and friendly heroes
        if (collision.gameObject.name.Contains("Determination") || 
            collision.gameObject.name.Contains("Bravery") || 
            collision.gameObject.name.Contains("Kindness") || 
            collision.gameObject.name.Contains("Happiness") ||
            collision.gameObject.name.Contains("Honesty") ||
            collision.gameObject.name.Contains("Patience"))
        {
            return;
        }

        // Target monster/enemy health components
        if (collision.TryGetComponent<Health>(out var health))
        {
            health.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}