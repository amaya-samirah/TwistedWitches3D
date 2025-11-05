using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance { get; private set; }

    public List<QuestProgress> activeQuests = new();
    public List<string> handInQuestIDs = new();

    private QuestUI questUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        questUI = FindAnyObjectByType<QuestUI>();
        // Subscribe to inventory change event
        // ... =+ [function that hapens when inventory changed]
        InventoryController.Instance.OnInventoryChanged += CheckInventoryForQuests;
    }

    public void CheckInventoryForQuests()
    {
        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();

        foreach (QuestProgress quest in activeQuests)
        {
            foreach (QuestObjective questObjective in quest.objectives)  // for every object we have
            {
                if (questObjective.type != ObjectiveType.CollectItem) continue;  // skip if quest objective isn't to collect item

                if (!int.TryParse(questObjective.objectiveID, out int itemID)) continue;  // if objectID can't be parsed to int (won't match with item IDs)

                int newAmount = itemCounts.TryGetValue(itemID, out int count) ? Mathf.Min(count, questObjective.requiredAmount) : 0;  // ex. will display 5/5 instead of 6/5

                if (questObjective.currentAmount != newAmount)
                {
                    questObjective.currentAmount = newAmount;
                }
            }
        }

        questUI.UpdateQuestUI();
    }

    public void AcceptQuest(Quest quest)
    {
        if (IsQuestActive(quest.questID))  // don't accept new quest that already exists
        {
            return;
        }
        activeQuests.Add(new QuestProgress(quest));  // construct new quest

        CheckInventoryForQuests();

        questUI.UpdateQuestUI();
    }

    // For if only want 1 quest of a certain type at a time...
    public bool IsQuestActive(string questID) => activeQuests.Exists(q => q.QuestID == questID);  // does this quest ID already exist in active quests

    // TODO:
    // - add CheckTalkNPCObjective()

    // Use for conditional NPC dialogue (i.e. if finished quest NPC says this)
    public bool IsQuestCompleted(string questID)
    {
        QuestProgress quest = activeQuests.Find(q => q.QuestID == questID);
        return quest != null && quest.objectives.TrueForAll(o => o.IsCompleted);
    }

    public void HandInQuest(string questID)
    {
        // Check required items
        if (!RemoveRequiredItems(questID))  // missing items --> can't complete quest
        {
            Debug.Log("Can't complete quest yet.");
            return;
        }

        // Remove quest from log
        QuestProgress quest = activeQuests.Find(q => q.QuestID == questID);
        if (quest != null)
        {
            //PlayerStats.Instance.IncreaseXPPoints(quest.quest.xpAmount);
            handInQuestIDs.Add(questID);
            activeQuests.Remove(quest);
            questUI.UpdateQuestUI();
        }
    }

    public bool RemoveRequiredItems(string questID)
    {
        QuestProgress quest = activeQuests.Find(q => q.QuestID == questID);
        if (quest == null) return false;

        Dictionary<int, int> requiredItems = new();

        // Check item requirements
        foreach (QuestObjective objective in quest.objectives)
        {
            if (objective.type == ObjectiveType.CollectItem && int.TryParse(objective.objectiveID, out int itemID))
            {
                requiredItems[itemID] = objective.requiredAmount;  // ex. require 5 donuts --> 3,5
            }
        }
        // Check have items
        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();

        foreach (var item in requiredItems)
        {
            if (itemCounts.GetValueOrDefault(item.Key) < item.Value)  // if items in inventory is less than required items
            {
                return false;
            }
        }

        // Remove required items
        foreach (var itemRequirement in requiredItems)
        {
            InventoryController.Instance.RemoveItems(itemRequirement.Key, itemRequirement.Value);
        }

        return true;
    }

    public bool IsQuestHandedIn(string questID)
    {
        return handInQuestIDs.Contains(questID);
    }

    public void LoadQuestProgresss(List<QuestProgress> savedQuests)
    {
        activeQuests = savedQuests ?? new();  // loads or sets new list if null

        CheckInventoryForQuests();
        questUI.UpdateQuestUI();
    }

    public void CheckTalkNPCObjective(NPC npc)
    {
        foreach (QuestProgress quest in activeQuests)
        {
            foreach (QuestObjective questObjective in quest.objectives)  // for every object we have
            {
                if (questObjective.type != ObjectiveType.TalkNPC) continue;

                if (!int.TryParse(questObjective.objectiveID, out int npcID)) continue;

                if (npc.ID == npcID)
                {
                    questObjective.currentAmount++;
                }
            }
        }

        questUI.UpdateQuestUI();
    }
}
