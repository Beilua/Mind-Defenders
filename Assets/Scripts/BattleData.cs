using UnityEngine;

public static class BattleData
{
    // Game State & Currency
    public static int Lives = 3;
    public static int Diamonds = 0;
    public static int BattlePhase = 1;

    // Persistent Battle State Across Phase Transitions
    public static int CurrentStars = 99;      // Retains stars when losing a life
    public static float RemainingTime = 60f;   // Retains lantern timer
    public static bool IsTimerInitialized = false;

    // Hero Lineup
    public static string MainHeroName;
    public static Sprite MainHeroSprite;

    public static string AllyHeroName;
    public static Sprite AllyHeroSprite;

    public static string ThirdHeroName;       // 3rd Hero for Phase 2
    public static Sprite ThirdHeroSprite;

    // Monster Lineup
    public static string MainMonsterName;
    public static Sprite MainMonsterSprite;

    public static string SecondaryMonsterName;
    public static Sprite SecondaryMonsterSprite;

    public static string ThirdMonsterName;   // 3rd Monster for Phase 2
    public static Sprite ThirdMonsterSprite;

    // Journal Texts
    public static string HeroJournalText;
    public static string MonsterJournalText;

    // Call this only when starting a NEW run from Main Menu!
    public static void FullReset()
    {
        Lives = 3;
        BattlePhase = 1;
        CurrentStars = 99;
        RemainingTime = 60f;
        IsTimerInitialized = false;

        MainHeroName = string.Empty;
        MainHeroSprite = null;
        AllyHeroName = string.Empty;
        AllyHeroSprite = null;
        ThirdHeroName = string.Empty;
        ThirdHeroSprite = null;

        MainMonsterName = string.Empty;
        MainMonsterSprite = null;
        SecondaryMonsterName = string.Empty;
        SecondaryMonsterSprite = null;
        ThirdMonsterName = string.Empty;
        ThirdMonsterSprite = null;
    }
}