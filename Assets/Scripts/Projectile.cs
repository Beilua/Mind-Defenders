using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 300f;
    public float damage = 20f;
    public float lifetime = 5f;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move left toward heroes
        rectTransform.anchoredPosition -= new Vector2(speed * Time.deltaTime, 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Fireball Debug] Collided with: {other.gameObject.name}");
        Health targetHealth = other.GetComponentInParent<Health>();

        // ONLY damage targets that belong to the HERO faction!
        if (targetHealth != null && targetHealth.faction == Faction.Hero)
        {
            targetHealth.TakeDamage(damage);
            Destroy(gameObject); // Destroy fireball on impact with hero
        }
    }
}