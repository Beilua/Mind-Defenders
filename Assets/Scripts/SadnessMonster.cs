using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SadnessMonster : MonoBehaviour
{
    [Header("Debuff & Aura Settings")]
    public float attackSlowMultiplier = 2.0f; // Multiplies hero cooldown (2.0 = 50% slower attacks)
    public Color laneAuraColor = new Color(0.2f, 0.4f, 0.9f, 0.35f); // Translucent gloomy blue

    [Header("Lane Range Settings")]
    public float laneWidth = 60f;            // Tile height tolerance
    public float maxLaneDistance = 600f;     // Distance to cover to the left

    private GameObject visualAuraOverlay;
    private HashSet<AttackSlowDebuff> debuffedHeroes = new HashSet<AttackSlowDebuff>();

    void Start()
    {
        CreateLaneAuraVisual();
    }

    void Update()
    {
        ApplyAuraToLaneHeroes();
    }

    void CreateLaneAuraVisual()
    {
        // Creates a translucent UI/Sprite overlay extending left down the lane
        visualAuraOverlay = new GameObject("SadnessLaneAura");
        visualAuraOverlay.transform.SetParent(transform, false);

        // Position aura stretching left from Sadness
        visualAuraOverlay.transform.localPosition = new Vector3(-maxLaneDistance / 2f, 0f, 0f);

        // UI Image setup (works inside Canvas grid)
        Image auraImage = visualAuraOverlay.AddComponent<Image>();
        auraImage.color = laneAuraColor;
        auraImage.raycastTarget = false; // Prevents blocking mouse clicks/cards

        RectTransform rt = visualAuraOverlay.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.sizeDelta = new Vector2(maxLaneDistance, laneWidth);
        }
    }

    void ApplyAuraToLaneHeroes()
    {
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(-maxLaneDistance / 2f, 0f);
        Vector2 boxSize = new Vector2(maxLaneDistance, laneWidth);

        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f);

        foreach (var col in hitColliders)
        {
            GameObject hitObj = col.gameObject;

            // Ignore monsters, UI tiles, and non-hero objects
            if (hitObj.name.Contains("Burnout") || hitObj.name.Contains("Sadness") ||
                hitObj.name.Contains("Anxiety") || hitObj.name.Contains("Hopelessness") ||
                hitObj.name.Contains("Distraction") || hitObj.name.Contains("Tile") || 
                hitObj.name.Contains("Card"))
            {
                continue;
            }

            Health heroHealth = hitObj.GetComponentInParent<Health>();
            if (heroHealth != null)
            {
                GameObject heroObj = heroHealth.gameObject;

                // Dynamically apply attack speed debuff
                AttackSlowDebuff debuff = heroObj.GetComponent<AttackSlowDebuff>();
                if (debuff == null)
                {
                    debuff = heroObj.AddComponent<AttackSlowDebuff>();
                }

                debuff.ApplyAttackSlow(attackSlowMultiplier, 0.2f); // Maintains debuff while hero stays in lane
            }
        }
    }

    void OnDestroy()
    {
        // Clean up visual overlay when Sadness dies
        if (visualAuraOverlay != null)
        {
            Destroy(visualAuraOverlay);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 center = transform.position + new Vector3(-maxLaneDistance / 2f, 0f, 0f);
        Gizmos.DrawWireCube(center, new Vector3(maxLaneDistance, laneWidth, 1f));
    }
}