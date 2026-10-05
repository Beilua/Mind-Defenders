using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Background Music")]
    public AudioClip defaultBgm;

    [Header("Combat & Event SFX Registry")]
    public AudioClip heroAttackSfx;
    public AudioClip monsterAttackSfx;
    public AudioClip heroAppearSfx;
    public AudioClip monsterAppearSfx;
    public AudioClip buttonClickSfx;

    private void Awake()
    {
        // Keep single instance alive across all scene transitions
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (defaultBgm != null)
        {
            PlayBGM(defaultBgm);
        }
    }

    // --- BGM Logic ---
    public void PlayBGM(AudioClip bgmClip)
    {
        if (bgmClip == null || bgmSource == null) return;

        // Prevent restarting the exact same track if already playing
        if (bgmSource.isPlaying && bgmSource.clip == bgmClip) return;

        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }

    // --- SFX Logic ---
    public void PlaySFX(AudioClip sfxClip)
    {
        if (sfxClip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(sfxClip);
        }
    }

    // --- Quick Shortcut Methods for Combat ---
    public void PlayHeroAttack() => PlaySFX(heroAttackSfx);
    public void PlayMonsterAttack() => PlaySFX(monsterAttackSfx);
    public void PlayHeroAppear() => PlaySFX(heroAppearSfx);
    public void PlayMonsterAppear() => PlaySFX(monsterAppearSfx);
    public void PlayButtonClick() => PlaySFX(buttonClickSfx);
}