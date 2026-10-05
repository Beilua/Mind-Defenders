using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class HeroOption
{
    public string heroName;
    public Sprite heroSprite;                // Drag character artwork here!
    public string selectionButtonText;
    [TextArea(2, 4)] public string luminaReaction;
    [TextArea(2, 4)] public string journalPrompt;
}

[System.Serializable]
public class MonsterOption
{
    public string monsterName;
    public Sprite monsterSprite;             // Drag monster artwork here!
    public string selectionButtonText;
    [TextArea(2, 4)] public string luminaReaction;
    [TextArea(2, 4)] public string journalPrompt;
    [TextArea(3, 5)] public string encouragementMessage;
}

public class JournalingManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text fairySpeechText;
    public TMP_InputField journalInputField;

    [Header("Choice Containers & Buttons")]
    public GameObject choiceContainer;
    public Button choice1Button;
    public Button choice2Button;
    public Button choice3Button;
    
    [Header("Button Text References")]
    public TMP_Text choice1Text;
    public TMP_Text choice2Text;
    public TMP_Text choice3Text;

    [Header("Button Visual Feedback")]
    public Sprite defaultButtonSprite;
    public Sprite selectedButtonSprite;

    [Header("Panels")]
    public GameObject journalContainer;
    public GameObject battleContainer;

    [Header("Data Config")]
    public List<HeroOption> allHeroes = new List<HeroOption>();
    public List<MonsterOption> allMonsters = new List<MonsterOption>();

    [Header("Typewriter Settings")]
    public float typingSpeed = 0.03f;

    private List<HeroOption> currentHeroChoices = new List<HeroOption>();
    private List<MonsterOption> currentMonsterChoices = new List<MonsterOption>();

    private HeroOption selectedHero;
    private MonsterOption selectedMonster;

    private enum State { HeroSelect, HeroJournal, MonsterSelect, MonsterJournal, Finished }
    private State currentState;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    void Start()
    {
        choiceContainer.SetActive(false);
        journalContainer.SetActive(false);
        battleContainer.SetActive(false);
        fairySpeechText.text = "";

        StartCoroutine(DelayedHeroPhaseStart());
    }

    IEnumerator DelayedHeroPhaseStart()
    {
        yield return new WaitForSeconds(1.0f);
        StartHeroPhase();
    }

    void SetChoiceButtonsInteractable(bool interactable)
    {
        choice1Button.interactable = interactable;
        choice2Button.interactable = interactable;
        choice3Button.interactable = interactable;
    }

    // -------------------------------------------------------------
    // PHASE 1: HERO SELECTION (3 Random Choices)
    // -------------------------------------------------------------
    void StartHeroPhase()
    {
        currentState = State.HeroSelect;
        
        currentHeroChoices = GetRandomElements(allHeroes, 3);

        choice1Text.text = currentHeroChoices[0].selectionButtonText;
        choice2Text.text = currentHeroChoices[1].selectionButtonText;
        choice3Text.text = currentHeroChoices[2].selectionButtonText;

        ResetButtonVisuals();
        SetChoiceButtonsInteractable(true); // Enable clicks for Hero choices

        string greetingText = "Hai! Hal positif apa yang kamu rasakan hari ini?";
        StartCoroutine(TypeDialogueAndShowChoices(greetingText, choiceContainer));
    }

    // -------------------------------------------------------------
    // PHASE 2: MONSTER SELECTION (3 Random Choices)
    // -------------------------------------------------------------
    void StartMonsterPhase()
    {
        currentState = State.MonsterSelect;

        // Shuffle and pick 3 random unique monsters
        currentMonsterChoices = GetRandomElements(allMonsters, 3);

        choice1Text.text = currentMonsterChoices[0].selectionButtonText;
        choice2Text.text = currentMonsterChoices[1].selectionButtonText;
        choice3Text.text = currentMonsterChoices[2].selectionButtonText;

        ResetButtonVisuals();
        SetChoiceButtonsInteractable(true);

        string monsterPrompt = "Lalu, apakah ada perasaan berat atau tantangan yang sedang kamu hadapi?";
        StartCoroutine(TypeDialogueAndShowChoices(monsterPrompt, choiceContainer));
    }

    // -------------------------------------------------------------
    // SHUFFLE HELPER METHOD
    // -------------------------------------------------------------
    private List<T> GetRandomElements<T>(List<T> list, int count)
    {
        List<T> copy = new List<T>(list);
        List<T> result = new List<T>();

        for (int i = 0; i < count && copy.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, copy.Count);
            result.Add(copy[randomIndex]);
            copy.RemoveAt(randomIndex);
        }

        return result;
    }

    // -------------------------------------------------------------
    // BUTTON CLICK HANDLER
    // -------------------------------------------------------------
    public void OnChoiceButtonClicked(int choiceIndex)
    {
        if (isTyping) return;

        Button clickedButton = choiceIndex == 0 ? choice1Button : (choiceIndex == 1 ? choice2Button : choice3Button);
        HighlightButton(clickedButton);

        StartCoroutine(DelayedChoiceAction(choiceIndex));
    }

    IEnumerator DelayedChoiceAction(int choiceIndex)
    {
        yield return new WaitForSeconds(0.3f);

        // Disable buttons so player cannot click other options while journaling
        SetChoiceButtonsInteractable(false);

        if (currentState == State.HeroSelect)
        {
            selectedHero = currentHeroChoices[choiceIndex];

            BattleData.MainHeroName = selectedHero.heroName;
            BattleData.MainHeroSprite = selectedHero.heroSprite;

            List<HeroOption> remainingHeroes = new List<HeroOption>(allHeroes);
            remainingHeroes.Remove(selectedHero);
            HeroOption randomAlly = remainingHeroes[Random.Range(0, remainingHeroes.Count)];

            BattleData.AllyHeroName = randomAlly.heroName;
            BattleData.AllyHeroSprite = randomAlly.heroSprite;

            currentState = State.HeroJournal;
            // Keep choiceContainer active so buttons stay visible on screen!

            StartCoroutine(TypeDialogueOnly(selectedHero.luminaReaction, () => {
                journalInputField.text = "";
                journalContainer.SetActive(true);
            }));
        }
        else if (currentState == State.MonsterSelect)
        {
            selectedMonster = currentMonsterChoices[choiceIndex];

            BattleData.MainMonsterName = selectedMonster.monsterName;
            BattleData.MainMonsterSprite = selectedMonster.monsterSprite;

            List<MonsterOption> remainingMonsters = new List<MonsterOption>(allMonsters);
            remainingMonsters.Remove(selectedMonster);
            MonsterOption randomEnemy = remainingMonsters[Random.Range(0, remainingMonsters.Count)];

            BattleData.SecondaryMonsterName = randomEnemy.monsterName;
            BattleData.SecondaryMonsterSprite = randomEnemy.monsterSprite;

            currentState = State.MonsterJournal;
            // Keep choiceContainer active so buttons stay visible on screen!

            StartCoroutine(TypeDialogueOnly(selectedMonster.luminaReaction, () => {
                journalInputField.text = "";
                journalContainer.SetActive(true);
            }));
        }
    }

    IEnumerator TypeDialogueOnly(string dialogue, System.Action onComplete)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        isTyping = true;
        fairySpeechText.text = "";

        foreach (char letter in dialogue.ToCharArray())
        {
            fairySpeechText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
        onComplete?.Invoke();
    }

    IEnumerator TypeDialogueAndShowChoices(string dialogue, GameObject containerToEnable)
    {
        choiceContainer.SetActive(false);
        journalContainer.SetActive(false);

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        
        isTyping = true;
        fairySpeechText.text = "";

        foreach (char letter in dialogue.ToCharArray())
        {
            fairySpeechText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;

        containerToEnable.SetActive(true);
    }

    public void OnSubmitJournal()
    {
        if (isTyping) return;

        if (currentState == State.HeroJournal)
        {
            BattleData.HeroJournalText = journalInputField.text;
            journalContainer.SetActive(false);
            StartMonsterPhase();
        }
        else if (currentState == State.MonsterJournal)
        {
            BattleData.MonsterJournalText = journalInputField.text;
            currentState = State.Finished;

            journalContainer.SetActive(false);
            choiceContainer.SetActive(false); // Hide buttons when moving to final encouragement screen
            StartCoroutine(TypeDialogueAndShowBattleButton(selectedMonster.encouragementMessage));
        }
    }

    IEnumerator TypeDialogueAndShowBattleButton(string dialogue)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        isTyping = true;
        fairySpeechText.text = "";

        foreach (char letter in dialogue.ToCharArray())
        {
            fairySpeechText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;

        battleContainer.SetActive(true);
    }

    void HighlightButton(Button targetButton)
    {
        if (selectedButtonSprite != null)
        {
            targetButton.image.sprite = selectedButtonSprite;
        }
    }

    void ResetButtonVisuals()
    {
        Button[] allButtons = { choice1Button, choice2Button, choice3Button };
        foreach (var btn in allButtons)
        {
            if (btn != null && defaultButtonSprite != null)
            {
                btn.image.sprite = defaultButtonSprite;
                btn.image.color = Color.white;
            }
        }
    }

    // Send player to "Match" scene first instead of going straight to battle!
    public void LoadBattleScene()
    {
        SceneManager.LoadScene("Match");
    }
}