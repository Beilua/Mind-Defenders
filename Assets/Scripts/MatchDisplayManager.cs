using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MatchDisplayManager : MonoBehaviour
{
    [Header("Hero Frame Images (Left Side)")]
    public Image chosenHeroImage;
    public Image randomAllyHeroImage;

    [Header("Monster Frame Images (Right Side)")]
    public Image chosenMonsterImage;
    public Image randomSecondaryMonsterImage;
    void Start()
    {
        // Display Hero Sprites inside the frame slots
        if (BattleData.MainHeroSprite != null && chosenHeroImage != null)
            chosenHeroImage.sprite = BattleData.MainHeroSprite;

        if (BattleData.AllyHeroSprite != null && randomAllyHeroImage != null)
            randomAllyHeroImage.sprite = BattleData.AllyHeroSprite;

        // Display Monster Sprites inside the frame slots
        if (BattleData.MainMonsterSprite != null && chosenMonsterImage != null)
            chosenMonsterImage.sprite = BattleData.MainMonsterSprite;

        if (BattleData.SecondaryMonsterSprite != null && randomSecondaryMonsterImage != null)
            randomSecondaryMonsterImage.sprite = BattleData.SecondaryMonsterSprite;
    }
    // OnClick function for the START button in the Match scene
    public void OnStartBattleButtonClicked()
    {
        SceneManager.LoadScene("Battle");
    }
}