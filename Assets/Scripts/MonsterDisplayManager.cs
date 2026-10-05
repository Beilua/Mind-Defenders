using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct MonsterSpriteMapping
{
    public string monsterName;
    public Sprite monsterSprite;
}

public class MonsterDisplayManager : MonoBehaviour
{
    public Image monsterDisplayUI;
    public List<MonsterSpriteMapping> monsterSprites = new List<MonsterSpriteMapping>();

    void Start()
    {
        UpdateMonsterDisplay();
    }

    void UpdateMonsterDisplay()
    {
        if (monsterDisplayUI == null) return;

        // Find the sprite matching BattleData.MainMonsterName
        MonsterSpriteMapping mapping = monsterSprites.Find(
            m => m.monsterName.Equals(BattleData.MainMonsterName, System.StringComparison.OrdinalIgnoreCase)
        );

        if (mapping.monsterSprite != null)
        {
            monsterDisplayUI.sprite = mapping.monsterSprite;
            monsterDisplayUI.enabled = true;

        }
    }
}