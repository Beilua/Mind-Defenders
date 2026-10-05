using UnityEngine;

public class PatienceHero : MonoBehaviour
{
    [Header("Healer Settings")]
    public float healCooldown = 3.0f;

    [Header("References")]
    public GameObject waterPrefab;       
    public Transform spawnPoint;         
    public Animator animator;

    private float nextHealTime = 0f;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.time >= nextHealTime)
        {
            CastHeal();
            nextHealTime = Time.time + healCooldown;
        }
    }

    void CastHeal()
    {
        if (waterPrefab == null) return;

        if (animator != null) animator.SetTrigger("Attack");

        // Spawn 0.8 units in front of Patience
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position + (Vector3.right * 0.8f);
        
        GameObject projectile = Instantiate(waterPrefab, spawnPos, Quaternion.identity);

        if (transform.parent != null)
        {
            projectile.transform.SetParent(transform.parent, true);
        }

        // Link the caster so the projectile ignores Patience's own colliders
        WaterProjectile waterScript = projectile.GetComponent<WaterProjectile>();
        if (waterScript != null)
        {
            waterScript.caster = gameObject;
        }
    }
}