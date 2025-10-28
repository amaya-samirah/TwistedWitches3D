using System.Collections.Generic;
using UnityEngine;

// This controls the music library for the Start Menu scene and other scenes
public class MusicLibrary : MonoBehaviour
{
    [SerializeField] private MusicGroup[] musicGroups;

    // string = group name, list = collection of audio clips
    private Dictionary<string, List<AudioClip>> musicDictionary;

    private void Awake()
    {
        InitializeDictionary();
    }

    // Sets up the music groups in a dictionary
    private void InitializeDictionary()
    {
        musicDictionary = new Dictionary<string, List<AudioClip>>();

        foreach (MusicGroup musicGroup in musicGroups)
        {
            musicDictionary[musicGroup.name] = musicGroup.audioClips;
        }
    }
    
    // Gets a random audio clip from the collection of a certian music group
    public AudioClip GetRandomClip(string groupName)
    {
        if (musicDictionary.ContainsKey(groupName))
        {
            List<AudioClip> audioClips = musicDictionary[groupName];

            if (audioClips.Count > 0)
            {
                return audioClips[UnityEngine.Random.Range(0, audioClips.Count)];
            }
        }

        return null;  // no audio clips
    }
}

[System.Serializable]
public struct MusicGroup
{
    public string name;
    public List<AudioClip> audioClips;
}
