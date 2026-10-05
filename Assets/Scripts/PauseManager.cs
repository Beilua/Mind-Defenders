using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Popup Reference")]
    public GameObject pausePopupPanel;

    [Header("UI Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Audio Sources")]
    public AudioSource musicAudioSource;
    public AudioSource sfxAudioSource;

    void Start()
    {
        // Initialize sliders with stored values or default to max (1.0)
        if (musicSlider != null)
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            SetMusicVolume(musicSlider.value);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
            SetSFXVolume(sfxSlider.value);
        }
    }

    public void PauseGame()
    {
        if (pausePopupPanel != null) pausePopupPanel.SetActive(true);
        Time.timeScale = 0f; // Freeze game logic and physics
    }

    public void ResumeGame()
    {
        if (pausePopupPanel != null) pausePopupPanel.SetActive(false);
        Time.timeScale = 1f; // Resume game logic
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void SetMusicVolume(float value)
    {
        if (musicAudioSource != null) musicAudioSource.volume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        if (sfxAudioSource != null) sfxAudioSource.volume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
}