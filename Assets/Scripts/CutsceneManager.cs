using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    [Header("Cutscene Video")]
    public VideoPlayer videoPlayer;
    public GameObject videoPanel; // Panel/RawImage displaying video

    [Header("Gender Choice UI")]
    public GameObject genderChoicePanel; // Image showing Female (Left) & Male (Right)

    [Header("Next Scene")]
    public string customizationSceneName = "Customize"; // Exact scene name

    void Start()
    {
        // Hide choice image while cutscene plays
        if (genderChoicePanel != null)
            genderChoicePanel.SetActive(false);

        if (videoPlayer != null && videoPlayer.clip != null)
        {
            if (videoPanel != null) videoPanel.SetActive(true);
            videoPlayer.loopPointReached += OnCutsceneFinished;
            videoPlayer.Play();
        }
        else
        {
            // Skip video if no video clip is assigned
            OnCutsceneFinished(null);
        }
    }

    void OnCutsceneFinished(VideoPlayer vp)
    {
        if (videoPanel != null) videoPanel.SetActive(false);
        if (genderChoicePanel != null) genderChoicePanel.SetActive(true);
    }

    // Called when clicking LEFT side
    public void SelectFemale()
    {
        PlayerPrefs.SetString("SelectedGender", "Female");
        PlayerPrefs.SetInt("PlayerSpriteIndex", 0); // Sets default to Index 0 (Olivia)
        PlayerPrefs.Save();

        SceneManager.LoadScene(customizationSceneName);
    }

    // Called when clicking RIGHT side
    public void SelectMale()
    {
        PlayerPrefs.SetString("SelectedGender", "Male");
        PlayerPrefs.SetInt("PlayerSpriteIndex", 1); // Sets default to Index 1 (Oliver)
        PlayerPrefs.Save();

        SceneManager.LoadScene(customizationSceneName);
    }

    public void SkipCutscene()
    {
        if (videoPlayer != null) videoPlayer.Stop();
        OnCutsceneFinished(null);
    }
}