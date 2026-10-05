using System.Collections;
using System.Reflection;
using UnityEngine;

public class SlowDebuff : MonoBehaviour
{
    private bool isSlowed = false;

    public void ApplySlow(float slowFactor, float duration)
    {
        if (!isSlowed)
        {
            StartCoroutine(SlowRoutine(slowFactor, duration));
        }
    }

    private IEnumerator SlowRoutine(float slowFactor, float duration)
    {
        isSlowed = true;

        MonoBehaviour monsterScript = null;
        FieldInfo speedField = null;

        // Search for the monster script containing moveSpeed
        foreach (var script in GetComponents<MonoBehaviour>())
        {
            FieldInfo field = script.GetType().GetField("moveSpeed");
            if (field != null)
            {
                monsterScript = script;
                speedField = field;
                break;
            }
        }

        if (monsterScript != null && speedField != null)
        {
            float originalSpeed = (float)speedField.GetValue(monsterScript);
            float newSpeed = originalSpeed * slowFactor;

            speedField.SetValue(monsterScript, newSpeed);
            Debug.Log($"<color=yellow>[Slow Debuff] SLOWED {gameObject.name}! Speed reduced from {originalSpeed} -> {newSpeed}</color>");

            // Visual feedback: Tint monster ICE BLUE
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            Color originalColor = Color.white;
            if (sr != null)
            {
                originalColor = sr.color;
                sr.color = new Color(0.2f, 0.8f, 1f, 1f); // Bright Ice Blue
            }

            yield return new WaitForSeconds(duration);

            // Restore original speed and color
            speedField.SetValue(monsterScript, originalSpeed);
            if (sr != null) sr.color = originalColor;

            Debug.Log($"<color=yellow>[Slow Debuff] RESTORED {gameObject.name} speed back to {originalSpeed}</color>");
        }
        else
        {
            Debug.LogError($"[Slow Debuff] Could not find a 'moveSpeed' field on {gameObject.name}!");
        }

        Destroy(this);
    }
}