using System.Collections;
using UnityEngine;

public class HappinessHero : MonoBehaviour
{
    [Header("Sunflower Production Settings")]
    public float produceInterval = 8.0f; // Seconds between star drops
    public int minStars = 2;
    public int maxStars = 4;

    [Header("Click Interaction Settings")]
    public bool enableClickToHarvest = true;
    public float clickCooldown = 2.0f;
    private bool canClick = true;

    [Header("Visual References")]
    public Animator animator;
    public GameObject starPopEffectPrefab; // Optional pop visual effect

    private void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();

        // Start automatic periodic star generation (Sunflower behavior)
        StartCoroutine(ProduceStarsRoutine());
    }

    // Sunflower-style automatic producer
    private IEnumerator ProduceStarsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(produceInterval);
            ProduceStars();
        }
    }

    // Direct click handler (if player clicks on Happiness)
    private void OnMouseDown()
    {
        if (!enableClickToHarvest || !canClick) return;

        ProduceStars();
        StartCoroutine(ClickCooldownRoutine());
    }

    private void ProduceStars()
    {
        // Randomize 2, 3, or 4 stars
        int amount = Random.Range(minStars, maxStars + 1);

        // Add stars directly to your ResourceManager
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.AddStars(amount);
        }

        // Trigger produce animation trigger if available
        if (animator != null)
        {
            animator.SetTrigger("Produce");
        }

        // Optional visual FX spawn at hero location
        if (starPopEffectPrefab != null)
        {
            Instantiate(starPopEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    private IEnumerator ClickCooldownRoutine()
    {
        canClick = false;
        yield return new WaitForSeconds(clickCooldown);
        canClick = true;
    }
}