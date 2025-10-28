using UnityEngine;

public class MenuMusicManager : MonoBehaviour
{
    // group name = "StartMenu"
    private static MusicLibrary musicLibrary;

    private AudioSource audioSource;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        musicLibrary = GetComponent<MusicLibrary>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (audioSource != null) audioSource.Stop();

        // Start music
        PlayMusic();
    }

    public void PlayMusic()
    {
        if (audioSource == null) return;

        AudioClip audioClip = musicLibrary.GetRandomClip("StartMenu");

        if (audioClip != null) audioSource.clip = audioClip;

        if (audioSource.clip != null) audioSource.Play();
    }
}
