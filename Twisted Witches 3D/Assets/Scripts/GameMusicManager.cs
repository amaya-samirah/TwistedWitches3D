using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Handles the background music for the gameplay scenes
public class GameMusicManager : MonoBehaviour
{
    public static GameMusicManager Instance { get; private set; }

    private static MusicLibrary musicLibrary;
    private AudioSource audioSource;
    [SerializeField] private Slider musicSlider;  // to set volume

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            audioSource = GetComponent<AudioSource>();
            musicLibrary = GetComponent<MusicLibrary>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (audioSource != null) audioSource.Stop();  // so music can change between scenes

        // Start music
        PlayMusic(false);

        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(delegate { SetVolume(musicSlider.value); });
        }
    }

    // To play background music
    // Optional clip param in casw want to change music
    public void PlayMusic(bool resetSong, AudioClip clip = null)
    {
        if (audioSource == null) return;

        AudioClip audioClip = clip != null ? clip : musicLibrary.GetRandomClip("Play");

        if (audioClip != null) audioSource.clip = audioClip;

        if (audioSource.clip != null)
        {
            if (resetSong)
            {
                // Stop song before playing again
                audioSource.Stop();
            }

            audioSource.Play();
        }
    }

    // To stop the music
    public void PauseMusic()
    {
        if (audioSource == null) return;

        audioSource.Pause();
    }

    // Manage volume of background music
    public void SetVolume(float volume)
    {
        if (Instance == null || Instance.audioSource == null) return;

        Instance.audioSource.volume = volume; 
    }

    private void OnDestroy()
    {
        // So won't try to use a destroyed gameobject
        if (Instance == this) Instance = null;

        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveListener(delegate { SetVolume(musicSlider.value); });
        }
    }
}
