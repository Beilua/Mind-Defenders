using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BattleManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI lanternTimerText;
    public TextMeshProUGUI livesText;

    [Header("Default Settings")]
    public float initialBattleDuration = 60f;
    private bool isBattleActive = true;

    void Start()
    {
        // Initialize timer state on fresh run
        if (!BattleData.IsTimerInitialized)
        {
            BattleData.RemainingTime = initialBattleDuration;
            BattleData.IsTimerInitialized = true;
        }

        UpdateLivesUI();
    }

    void Update()
    {
        if (!isBattleActive) return;

        if (BattleData.RemainingTime > 0)
        {
            BattleData.RemainingTime -= Time.deltaTime;
            if (lanternTimerText != null)
            {
                lanternTimerText.text = Mathf.CeilToInt(BattleData.RemainingTime).ToString();
            }
        }
        else
        {
            OnBattleVictory();
        }
    }

    public void OnPlayerBoundaryBreached()
    {
        if (!isBattleActive) return;
        isBattleActive = false;

        BattleData.Lives--;
        UpdateLivesUI();

        if (BattleData.Lives > 0)
        {
            BattleData.BattlePhase++;
            Debug.Log($"Life lost! Retaining {BattleData.CurrentStars} stars and {Mathf.CeilToInt(BattleData.RemainingTime)}s remaining.");
            SceneManager.LoadScene("Chat"); 
        }
        else
        {
            SceneManager.LoadScene("GameOverScene");
        }
    }

    void OnBattleVictory()
    {
        isBattleActive = false;
        BattleData.Diamonds += 10;
        SceneManager.LoadScene("YouWinScene");
    }

    void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = $"x{BattleData.Lives}";
        }
    }
}