using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")]
public class NPCDialogue : ScriptableObject  // scriptable is a class that can be created as an opject to hold data
{
    public string npcName;
    public Sprite npcPortrait;
    public string[] dialogueLines;
    public bool[] autoProgressLines;
    public bool[] endDialogeLines;  // mark where dialogue meant to end
    public float typingSpeed = 0.5f;
    public AudioClip voiceSound;
    public float voicePitch = 1f;  // normal pitch of audio clip
    public float autoProgressDelay = 1.5f;

    public DialogueChoice[] choices;

    public int questInProgressIndex;  // conditional quest dialogue (i.e. if on quest npc might say "good luck")
    public int questCompletedIndex;  // what NPC will say once quest completed
    public Quest quest;  // quest NPC gives
}

[System.Serializable]
public class DialogueChoice
{
    public int dialogueIndex;  // Dialogue line where choices appear
    public string[] choices;  // Player responses
    public int[] nextDialogueIndex;  // What next after choice
    public bool[] givesQuest;  // show if choice gives a quest
}
