using System.Collections;
using UnityEngine;

public class RangedHero : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform spawnPoint; // Position where leaf comes out
    public float attackInterval = 2.5f;

    [Header("Debuff Multiplier")]
    public float attackSpeedMultiplier = 1.0f; // 1.0 = normal speed, 2.0 = 2x slower
    private Animator animator;
    private Canvas canvas;

    

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        while (true)
        {
            // Multiply interval by multiplier (e.g. 2.5s * 2.0 = 5.0s between shots!)
            yield return new WaitForSeconds(attackInterval * attackSpeedMultiplier);

            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            SpawnProjectile();
        }
    }

    public void SpawnProjectile()
    {
        if (projectilePrefab != null && canvas != null)
        {
            GameObject newProj = Instantiate(projectilePrefab, canvas.transform);
            RectTransform projRect = newProj.GetComponent<RectTransform>();

            Vector2 spawnPos = spawnPoint != null ? 
                (Vector2)spawnPoint.position : (Vector2)transform.position;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                spawnPos,
                null,
                out Vector2 localPoint
            );

            projRect.anchoredPosition = localPoint;
        }
    }

    
}