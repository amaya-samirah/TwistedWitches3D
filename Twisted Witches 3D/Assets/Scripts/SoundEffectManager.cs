using UnityEngine;
using UnityEngine.UI;

public class SoundEffectManager : MonoBehaviour
{
    private static SoundEffectManager Instance;
    private static AudioSource audioSource;
    private static AudioSource voiceAudioSource;
    private static SoundEffectLibrary soundEffectLibrary;

    [SerializeField] private Slider sfxSlider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Set up variables...
            AudioSource[] audioSources = GetComponents<AudioSource>();
            audioSource = audioSources[0];
            voiceAudioSource = audioSources[1];
            soundEffectLibrary = GetComponent<SoundEffectLibrary>();

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Play sound effects
    public static void PlaySFX(string groupName)
    {
        AudioClip audioClip = soundEffectLibrary.GetRandomClip(groupName);
        if (audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }

    // Play voice for NPC dialogue
    public static void PlayVoice(AudioClip audioClip, float pitch = 1f)
    {
        voiceAudioSource.pitch = pitch;
        voiceAudioSource.PlayOneShot(audioClip);
    }

    void Start()
    {
        sfxSlider.onValueChanged.AddListener(delegate { OnValueChanged(); });
    }

    // Sets volume for regular sound effects & dialouge sound effects
    public static void SetVolume(float volume)
    {
        audioSource.volume = volume;
        voiceAudioSource.volume = volume;
    }

    public void OnValueChanged()
    {
        SetVolume(sfxSlider.value);
    }
}
