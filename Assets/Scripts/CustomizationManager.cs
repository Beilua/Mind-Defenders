using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationManager : MonoBehaviour
{
    [Header("Player Display")]
    public Image playerDisplayImage;

    [Header("All 6 Mirror Character UI Images")]
    // Drag in order: 0: Olivia, 1: Oliver, 2: Asep, 3: Chiyo, 4: Yaya, 5: Nahnul
    public List<Image> mirrorCharacterImages = new List<Image>();

    [Header("All 6 Character Sprites")]
    // Drag in order: 0: Olivia, 1: Oliver, 2: Asep, 3: Chiyo, 4: Yaya, 5: Nahnul
    public List<Sprite> allCharacters = new List<Sprite>();

    private int currentPage = 0; // 0 = (0,1), 1 = (2,3), 2 = (4,5)

    void Start()
    {
        // 1. Assign sprites to all mirror images
        UpdateMirrorSprites();

        // 2. Load saved selection (default to index 0)
        int savedIndex = PlayerPrefs.GetInt("PlayerSpriteIndex", 0);
        SelectCharacter(savedIndex);

        // 3. Display initial slide (0: Olivia & 1: Oliver)
        ShowPage(currentPage);
    }

    private void UpdateMirrorSprites()
    {
        for (int i = 0; i < mirrorCharacterImages.Count; i++)
        {
            if (mirrorCharacterImages[i] != null && i < allCharacters.Count)
            {
                mirrorCharacterImages[i].sprite = allCharacters[i];
            }
        }
    }

    // Call this from each character Button's OnClick()
    public void SelectCharacter(int characterIndex)
    {
        if (allCharacters != null && characterIndex >= 0 && characterIndex < allCharacters.Count)
        {
            Sprite chosenSprite = allCharacters[characterIndex];

            if (playerDisplayImage != null)
            {
                playerDisplayImage.sprite = chosenSprite;
            }

            // Save sprite choice for all gameplay scenes
            PlayerPrefs.SetInt("PlayerSpriteIndex", characterIndex);
            PlayerPrefs.Save();

            Debug.Log($"[Customization] Selected Character {characterIndex}: {chosenSprite.name}");
        }
    }

    // Call this from Next Arrow Button OnClick()
    public void NextMirrorPage()
    {
        currentPage = (currentPage + 1) % 3; // Cycles between 0, 1, and 2
        ShowPage(currentPage);
    }

    private void ShowPage(int page)
    {
        int activeFirst = page * 2;     // Page 0 -> 0; Page 1 -> 2; Page 2 -> 4
        int activeSecond = page * 2 + 1; // Page 0 -> 1; Page 1 -> 3; Page 2 -> 5

        for (int i = 0; i < mirrorCharacterImages.Count; i++)
        {
            if (mirrorCharacterImages[i] != null)
            {
                bool shouldBeActive = (i == activeFirst || i == activeSecond);
                mirrorCharacterImages[i].gameObject.SetActive(shouldBeActive);
            }
        }
    }
}