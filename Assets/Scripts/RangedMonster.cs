using System.Collections;
using UnityEngine;

public class RangedMonster : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform spawnPoint;
    public float attackInterval = 2.5f;

    private Animator animator;
    private Canvas canvas;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        FindCanvas();
        StartCoroutine(AttackRoutine());
    }

    private void FindCanvas()
    {
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            canvas = FindFirstObjectByType<Canvas>();
        }
    }

    IEnumerator AttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);

            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            SpawnProjectile();
        }
    }

    public void SpawnProjectile()
    {
        if (canvas == null)
        {
            FindCanvas();
        }

        if (projectilePrefab == null)
        {
            Debug.LogWarning($"[RangedMonster] Missing Projectile Prefab on {gameObject.name}!");
            return;
        }

        if (canvas != null)
        {
            // 1. Spawn projectile inside Canvas
            GameObject newProj = Instantiate(projectilePrefab, canvas.transform);
            
            // 2. Ensure it renders on top
            newProj.transform.SetAsLastSibling(); 

            // 3. Set world position directly to monster or spawn point position
            Vector3 targetPos = (spawnPoint != null) ? spawnPoint.position : transform.position;
            newProj.transform.position = targetPos;
        }
    }
}