using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    // NPC IDs for quests...
    // 0 - Hodor
    // 1 - Olive
    // 2 - Vander

    public NPCDialogue dialogueData;
    public int ID;

    private DialogueController dialogueUI;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    private enum QuestState
    {
        NotStarted,
        InProgress,
        Completed
    }

    private QuestState questState = QuestState.NotStarted;

    private void Start()
    {
        dialogueUI = DialogueController.Instance;
    }

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if (dialogueData == null || (PauseController.IsGamePaused && !isDialogueActive))
        {
            return;  // don't want dialogue to pop up over menu
        }

        if (isDialogueActive)
        {
            // Next line
            NextLine();
        }
        else
        {
            // Start Dialogue
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        // Sync with quest data
        SyncQuestState();

        // Set dialogue based on quest state
        if (questState == QuestState.NotStarted)
        {
            dialogueIndex = 0;
        }
        else if (questState == QuestState.InProgress)
        {
            dialogueIndex = dialogueData.questCompletedIndex;
        }
        else if (questState == QuestState.Completed)
        {
            dialogueIndex = dialogueData.questCompletedIndex;
        }

        isDialogueActive = true;

        dialogueUI.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);

        dialogueUI.ShowDialogueUI(true);
        PauseController.SetPause(true);

        // Type line
        DisplayCurrentLine();
    }

    private void SyncQuestState()
    {
        if (dialogueData.quest == null)
        {
            return;
        }

        string questID = dialogueData.quest.questID;

        if (QuestController.Instance.IsQuestCompleted(questID) || QuestController.Instance.IsQuestHandedIn(questID))
        {
            questState = QuestState.Completed;
        }
        else if (QuestController.Instance.IsQuestActive(questID))
        {
            questState = QuestState.InProgress;
        }
        else
        {
            questState = QuestState.NotStarted;
        }
    }

    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueUI.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]); // instead of line by line will autocomplete full line into text box
            isTyping = false;
        }

        // Clear choices
        dialogueUI.ClearChoices();

        // Check if end of dialogue
        if (dialogueData.endDialogeLines.Length > dialogueIndex && dialogueData.endDialogeLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }

        // Check if any choices --> display
        foreach (DialogueChoice dialogueChoice in dialogueData.choices)
        {
            if (dialogueChoice.dialogueIndex == dialogueIndex)
            {
                // Display choices
                DisplayChoices(dialogueChoice);
                return;
            }
        }

        if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            // End dialogue
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueUI.SetDialogueText("");

        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueUI.SetDialogueText(dialogueUI.dialogueText.text += letter);
            SoundEffectManager.PlayVoice(dialogueData.voiceSound, dialogueData.voicePitch);
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        // Auto progression
        if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);

            // Display next line
            NextLine();
        }
    }

    void DisplayChoices(DialogueChoice choice)
    {
        for (int i = 0; i < choice.choices.Length; i++)
        {
            int nextIndex = choice.nextDialogueIndex[i];

            bool givesQuest = choice.givesQuest[i];  // does this choice give a quest

            dialogueUI.CreatChoiceButton(choice.choices[i], () => ChooseOption(nextIndex, givesQuest));  // '() =>' passes function in
        }
    }

    void ChooseOption(int nextIndex, bool givesQuest)
    {
        if (givesQuest)
        {
            QuestController.Instance.AcceptQuest(dialogueData.quest);
            questState = QuestState.InProgress;
        }

        dialogueIndex = nextIndex;
        dialogueUI.ClearChoices();

        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        StopAllCoroutines();  // all text will stop typing & go to next line
        StartCoroutine(TypeLine());
    }

    public void EndDialogue()
    {
        // Conditional: if completed quest but not handed in yet
        if (questState == QuestState.Completed && !QuestController.Instance.IsQuestHandedIn(dialogueData.quest.questID))
        {
            HandleQuestCompleteion(dialogueData.quest);
        }

        StopAllCoroutines();
        isDialogueActive = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);
        PauseController.SetPause(false);
    }

    void HandleQuestCompleteion(Quest quest)
    {
        // Give reward
        RewardsController.Instance.GiveQuestReward(quest);
        QuestController.Instance.HandInQuest(quest.questID);
    }
}
