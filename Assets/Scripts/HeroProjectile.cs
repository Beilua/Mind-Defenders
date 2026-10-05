using UnityEngine;

public class HeroProjectile : MonoBehaviour
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
        // Move right toward monsters
        rectTransform.anchoredPosition += new Vector2(speed * Time.deltaTime, 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Health targetHealth = other.GetComponentInParent<Health>();

        // ONLY damage targets that belong to the MONSTER faction!
        if (targetHealth != null && targetHealth.faction == Faction.Monster)
        {
            targetHealth.TakeDamage(damage);
            Destroy(gameObject); // Destroy leaf on impact with monster
        }
    }
}