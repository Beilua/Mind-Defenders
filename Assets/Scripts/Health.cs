using UnityEngine;
using UnityEngine.UI;

// Define the two teams
public enum Faction { Hero, Monster }

public class Health : MonoBehaviour
{
    [Header("Faction Team")]
    public Faction faction = Faction.Hero; // Default to Hero

    [Header("HP Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI Reference")]
    public Slider healthSlider;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Tile parentTile = GetComponentInParent<Tile>();
        if (parentTile != null)
        {
            parentTile.isOccupied = false;
        }

        Destroy(gameObject);
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}