using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MonsterPrefabMapping
{
    public string monsterName;        // e.g., "Distraction", "Sadness"
    public GameObject monsterPrefab;  // The monster prefab to spawn
    public bool requiresHeroTarget;   // True for Sadness, false for generic spawners
}

public class MonsterSpawner : MonoBehaviour
{
    [Header("Monster Prefab Registry")]
    public List<MonsterPrefabMapping> allMonsterPrefabs = new List<MonsterPrefabMapping>();

    [Header("Spawn Settings")]
    public float spawnInterval = 5.0f; 
    [Range(0f, 1f)] public float spawnChance = 0.40f; 

    [Header("Target Grid Lanes")]
    public List<Tile> backlineTiles = new List<Tile>(); 

    private MonsterPrefabMapping activeMainMonster;
    private MonsterPrefabMapping activeSecondaryMonster;

    void Start()
    {
        // Fallbacks for scene testing
        if (string.IsNullOrEmpty(BattleData.MainMonsterName)) BattleData.MainMonsterName = "Distraction";
        if (string.IsNullOrEmpty(BattleData.SecondaryMonsterName)) BattleData.SecondaryMonsterName = "Sadness";

        // Assign active monster entries from BattleData
        activeMainMonster = GetMonsterMapping(BattleData.MainMonsterName);
        activeSecondaryMonster = GetMonsterMapping(BattleData.SecondaryMonsterName);

        StartCoroutine(SpawnRoutine());
    }

    MonsterPrefabMapping GetMonsterMapping(string name)
    {
        return allMonsterPrefabs.Find(m => m.monsterName.Equals(name, System.StringComparison.OrdinalIgnoreCase));
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (Random.value <= spawnChance)
            {
                // 50/50 pick between Main Monster and Secondary Monster
                MonsterPrefabMapping selectedMonster = (Random.value > 0.5f) ? activeMainMonster : activeSecondaryMonster;

                if (selectedMonster.monsterPrefab != null)
                {
                    if (selectedMonster.requiresHeroTarget)
                    {
                        TrySpawnTargetedMonster(selectedMonster.monsterPrefab);
                    }
                    else
                    {
                        TrySpawnBacklineMonster(selectedMonster.monsterPrefab);
                    }
                }
            }
        }
    }

    void TrySpawnBacklineMonster(GameObject prefab)
    {
        List<Tile> availableTiles = new List<Tile>();
        foreach (Tile tile in backlineTiles)
        {
            if (tile != null && !tile.isOccupied)
            {
                availableTiles.Add(tile);
            }
        }

        if (availableTiles.Count > 0)
        {
            Tile chosenTile = availableTiles[Random.Range(0, availableTiles.Count)];
            InstantiateMonsterOnTile(prefab, chosenTile);
        }
    }

    [Header("Targeting Settings")]
    [Tooltip("If TRUE: Spawns on the tile directly next to the hero. If FALSE: Spawns at the rightmost edge of the grid.")]
    public bool spawnImmediatelyInFront = true; 

    void TrySpawnTargetedMonster(GameObject prefab)
    {
        // 1. Find placed heroes
        Health[] allHealthComponents = FindObjectsByType<Health>(FindObjectsSortMode.None);
        List<GameObject> activeHeroes = new List<GameObject>();

        foreach (Health h in allHealthComponents)
        {
            string objName = h.name;
            if (!objName.Contains("Distraction") && !objName.Contains("Sadness") &&
                !objName.Contains("Anxiety") && !objName.Contains("Burnout") &&
                !objName.Contains("Hopelessness"))
            {
                activeHeroes.Add(h.gameObject);
            }
        }

        if (activeHeroes.Count == 0)
        {
            Debug.Log("[Spawner Debug] No active heroes found. Falling back to backline spawn.");
            TrySpawnBacklineMonster(prefab);
            return;
        }

        // 2. Target a random hero
        GameObject targetHero = activeHeroes[Random.Range(0, activeHeroes.Count)];
        Debug.Log($"<color=yellow>[Spawner Debug] Targeted Hero: '{targetHero.name}' at X:{targetHero.transform.position.x:F1}, Y:{targetHero.transform.position.y:F1}</color>");

        Tile[] allTiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        List<Tile> validTiles = new List<Tile>();

        foreach (Tile tile in allTiles)
        {
            float yDiff = Mathf.Abs(tile.transform.position.y - targetHero.transform.position.y);
            bool inSameLane = yDiff < 0.6f; // Y-tolerance threshold for lane alignment
            bool isInFront = tile.transform.position.x > targetHero.transform.position.x;

            if (inSameLane && isInFront && !tile.isOccupied)
            {
                validTiles.Add(tile);
            }
        }

        Debug.Log($"[Spawner Debug] Found {validTiles.Count} candidate tile(s) in front of {targetHero.name}.");

        if (validTiles.Count > 0)
        {
            if (spawnImmediatelyInFront)
            {
                // Sort ASCENDING by X position -> Selects tile CLOSEST to hero (1 tile away)
                validTiles.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
            }
            else
            {
                // Sort DESCENDING by X position -> Selects tile FURTHEST right (grid edge)
                validTiles.Sort((a, b) => b.transform.position.x.CompareTo(a.transform.position.x));
            }

            Tile chosenTile = validTiles[0];
            Debug.Log($"<color=green>[Spawner Debug] Spawning '{prefab.name}' on Tile '{chosenTile.name}' at X:{chosenTile.transform.position.x:F1}, Y:{chosenTile.transform.position.y:F1}</color>");

            InstantiateMonsterOnTile(prefab, chosenTile);
        }
        else
        {
            Debug.LogWarning($"[Spawner Debug] No empty tiles in front of {targetHero.name}. Spawning on backline.");
            TrySpawnBacklineMonster(prefab);
        }
    }

    void InstantiateMonsterOnTile(GameObject prefab, Tile chosenTile)
    {
        GameObject monster = Instantiate(prefab, chosenTile.transform);
        RectTransform monsterRect = monster.GetComponent<RectTransform>();
        if (monsterRect != null)
        {
            monsterRect.anchoredPosition = Vector2.zero;
        }

        chosenTile.isOccupied = true;
        Debug.Log($"[Spawner] {prefab.name} spawned successfully!");
    }
}