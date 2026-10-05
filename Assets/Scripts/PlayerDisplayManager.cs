using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDisplayManager : MonoBehaviour
{
    [Header("Display Components")]
    public Image playerImageUI;               // Used if Player is a UI element
    public SpriteRenderer playerSpriteRenderer; // Used if Player is a 2D World object

    [Header("Character Sprites (6 Characters)")]
    // Must match exact order: 0: Olivia, 1: Oliver, 2: Asep, 3: Chiyo, 4: Yaya, 5: Nahnul
    public List<Sprite> allCharacters = new List<Sprite>();

    [Header("Facing Direction")]
    public bool flipHorizontal = false;

    void Start()
    {
        if (playerImageUI == null) playerImageUI = GetComponent<Image>();
        if (playerSpriteRenderer == null) playerSpriteRenderer = GetComponent<SpriteRenderer>();

        UpdatePlayerSprite();
    }

    public void UpdatePlayerSprite()
    {
        int spriteIndex = PlayerPrefs.GetInt("PlayerSpriteIndex", 0);

        if (allCharacters != null && allCharacters.Count > 0)
        {
            spriteIndex = Mathf.Clamp(spriteIndex, 0, allCharacters.Count - 1);
            Sprite chosenSprite = allCharacters[spriteIndex];

            if (playerImageUI != null) playerImageUI.sprite = chosenSprite;
            if (playerSpriteRenderer != null) playerSpriteRenderer.sprite = chosenSprite;
        }

        transform.localScale = flipHorizontal ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f);
    }
}