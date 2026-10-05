using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HeroCardMapping
{
    public string heroName;        // Matches string set in JournalingManager (e.g., "Bravery", "Patience")
    public GameObject cardPrefab;  // Drag BraveryCard, PatienceCard, etc. here
}

public class BattleLineupManager : MonoBehaviour
{
    [Header("Hero Prefab Registry")]
    public List<HeroCardMapping> availableHeroCards = new List<HeroCardMapping>();

    [Header("Card Container Transform")]
    public Transform cardParentTransform; // The UI layout container for cards

    void Start()
    {
        if (cardParentTransform == null) cardParentTransform = transform;

        // Fallbacks for direct scene testing without going through Journaling
        if (string.IsNullOrEmpty(BattleData.MainHeroName)) BattleData.MainHeroName = "Bravery";
        if (string.IsNullOrEmpty(BattleData.AllyHeroName)) BattleData.AllyHeroName = "Patience";
        if (string.IsNullOrEmpty(BattleData.ThirdHeroName)) BattleData.ThirdHeroName = "Honesty";

        SetupHeroCards();
    }

    public void SetupHeroCards()
    {
        // Clear any placeholder static cards in UI
        foreach (Transform child in cardParentTransform)
        {
            Destroy(child.gameObject);
        }

        // 1. Always spawn Main Hero Card
        if (!string.IsNullOrEmpty(BattleData.MainHeroName))
        {
            SpawnCardForHero(BattleData.MainHeroName);
        }

        // 2. Always spawn Ally Hero Card
        if (!string.IsNullOrEmpty(BattleData.AllyHeroName))
        {
            SpawnCardForHero(BattleData.AllyHeroName);
        }

        // 3. Spawn 3rd Hero Card ONLY if in Phase 2 or higher
        if (BattleData.BattlePhase >= 2 && !string.IsNullOrEmpty(BattleData.ThirdHeroName))
        {
            SpawnCardForHero(BattleData.ThirdHeroName);
        }
    }

    void SpawnCardForHero(string heroName)
    {
        HeroCardMapping match = availableHeroCards.Find(m => m.heroName.Equals(heroName, System.StringComparison.OrdinalIgnoreCase));

        if (match.cardPrefab != null)
        {
            Instantiate(match.cardPrefab, cardParentTransform);
            Debug.Log($"[Lineup] Successfully instantiated card for {heroName}");
        }
        else
        {
            Debug.LogWarning($"[Lineup Warning] No Card Prefab found in Inspector registry for hero: {heroName}");
        }
    }
}