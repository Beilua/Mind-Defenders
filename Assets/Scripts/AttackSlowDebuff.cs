using System.Collections;
using System.Reflection;
using UnityEngine;

public class AttackSlowDebuff : MonoBehaviour
{
    private bool isSlowed = false;

    public void ApplyAttackSlow(float slowFactor, float duration)
    {
        if (!isSlowed)
        {
            StartCoroutine(AttackSlowRoutine(slowFactor, duration));
        }
    }

    private IEnumerator AttackSlowRoutine(float slowFactor, float duration)
    {
        isSlowed = true;

        MonoBehaviour heroScript = null;
        FieldInfo cooldownField = null;

        // Dynamically find attackCooldown or attackSpeedMultiplier on hero script
        foreach (var script in GetComponents<MonoBehaviour>())
        {
            FieldInfo field = script.GetType().GetField("attackCooldown");
            if (field != null)
            {
                heroScript = script;
                cooldownField = field;
                break;
            }
        }

        if (heroScript != null && cooldownField != null)
        {
            float originalCooldown = (float)cooldownField.GetValue(heroScript);
            cooldownField.SetValue(heroScript, originalCooldown * slowFactor); // Increase cooldown = slower attacks

            // Visual feedback: Tint hero dark purple/blue
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            Color originalColor = (sr != null) ? sr.color : Color.white;
            if (sr != null) sr.color = new Color(0.6f, 0.5f, 0.9f, 1f);

            yield return new WaitForSeconds(duration);

            cooldownField.SetValue(heroScript, originalCooldown);
            if (sr != null) sr.color = originalColor;
        }

        Destroy(this);
    }
}