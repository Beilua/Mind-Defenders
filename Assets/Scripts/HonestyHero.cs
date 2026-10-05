using System.Collections;
using UnityEngine;

public class HonestyHero : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackCooldown = 2.0f;
    public GameObject beamPrefab;           // Assign Honesty Beam Prefab here
    public Transform attackPoint;          // Where the beam spawns

    [Header("Lane Detection Zone")]
    public Vector2 boxSize = new Vector2(500f, 60f);   // Long box looking RIGHT down the lane
    public Vector2 boxOffset = new Vector2(250f, 0f);  // Positive X offset to look RIGHT

    private Animator animator;
    private float lastAttackTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (attackPoint == null) attackPoint = transform;
    }

    void Update()
    {
        if (IsMonsterInLane())
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                StartCoroutine(ShootBeamSequence());
            }
        }
    }

    bool IsMonsterInLane()
    {
        Vector2 boxCenter = (Vector2)transform.position + boxOffset;
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f);

        foreach (var col in hitColliders)
        {
            GameObject hitObj = col.gameObject;

            // Check if any monster is in this lane
            if (hitObj.name.Contains("Burnout") || hitObj.name.Contains("Sadness") ||
                hitObj.name.Contains("Anxiety") || hitObj.name.Contains("Hopelessness") ||
                hitObj.name.Contains("Distraction"))
            {
                return true;
            }
        }

        return false;
    }

    IEnumerator ShootBeamSequence()
    {
        lastAttackTime = Time.time;

        if (animator != null)
        {
            animator.SetTrigger("Attack"); // Or "Attack" depending on your trigger name
        }

        // Wait for attack frame impact
        yield return new WaitForSeconds(0.25f);

        if (beamPrefab != null)
        {
            GameObject beam = Instantiate(beamPrefab, attackPoint.position, Quaternion.identity);
            if (transform.parent != null)
            {
                beam.transform.SetParent(transform.parent, true); // Keep Canvas scaling
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = transform.position + (Vector3)boxOffset;
        Gizmos.DrawWireCube(center, boxSize);
    }
}