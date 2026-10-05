using UnityEngine;

public class PlayerBoundaryTrigger : MonoBehaviour
{
    public BattleManager battleManager;

    void Start()
    {
        if (battleManager == null)
        {
            battleManager = FindFirstObjectByType<BattleManager>();
            if(battleManager == null) Debug.LogError("<color=red>[PBT-Debug] CRITICAL: BattleManager not found in scene!</color>");
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col == null) Debug.LogError("<color=red>[PBT-Debug] CRITICAL: missing Collider2D!</color>");
        else if(!col.isTrigger) Debug.LogWarning("<color=yellow>[PBT-Debug] WARNING: BoxCollider2D 'Is Trigger' is NOT checked!</color>");
    
        Debug.Log("<color=white>[PBT-Debug] Boundary initialized and listening...</color>");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug Log A: Confirm PHYSICAL CONTACT detected
        string rawName = other.gameObject.name;
        string cleanName = rawName.Replace("(Clone)", "").Trim();
        
        Debug.Log($"<color=cyan>[PBT-Debug] >> CONTACT detected with raw: '{rawName}' (Cleaned: '{cleanName}')</color>");

        // Filter out Friendlies/Projectiles (Add anything you want ignored here)
        if (cleanName.Contains("Hero") || cleanName.Contains("Bravery") || 
            cleanName.Contains("Honesty") || cleanName.Contains("Lumina") || 
            cleanName.Contains("Tile") || cleanName.Contains("Card"))
        {
            Debug.Log($"[PBT-Debug] Ignored Object: '{cleanName}'. Stopping logic.");
            return; 
        }

        // Detect Monsters OR Monster Projectiles
        bool isMonsterBreach = cleanName.Contains("Distraction") || 
                                cleanName.Contains("Sadness") ||
                                cleanName.Contains("Burnout") || 
                                cleanName.Contains("Anxiety") ||
                                cleanName.Contains("Hopelessness") || 
                                cleanName.Contains("Punch") ||
                                cleanName.Contains("Tear") ||
                                cleanName.Contains("Fireball");
        if (isMonsterBreach)
        {
            // Debug Log B: Breach MATCHED! Life deduction attempt IMMINENT
            Debug.Log($"<color=orange>[PBT-Debug] >> BREACH confirmed by '{cleanName}'. Executing life loss logic...</color>");

            if (battleManager != null)
            {
                // CRITICAL CALL: Notify BattleManager immediately!
                battleManager.OnPlayerBoundaryBreached();
                
                // Debug Log C: Execution successfully sent to BattleManager
                Debug.Log($"<color=green>[PBT-Debug] Life deduction call successfully sent to BattleManager.</color>");
            }
            else
            {
                Debug.LogError($"<color=red>[PBT-Debug] CRITICAL ERROR: BattleManager reference is NULL on {gameObject.name}!</color>");
            }

            // CLEANUP (optional, often better if monster script handles this, see below)
            Debug.Log($"[PBT-Debug] Attempting Destroy() on '{cleanName}'");
            Destroy(other.gameObject); 
        }
        else
        {
            Debug.LogWarning($"<color=yellow>[PBT-Debug] Unrecognized object touched boundary: '{cleanName}'</color>");
        }
    }
}