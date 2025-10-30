using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    public string questID;
    public string questName;
    public string description;
    public List<QuestObjective> objectives;
    public List<QuestReward> questRewards;

    // Generate unique quest id
    private void OnValidate()  // called whenever scriptable object is edited
    {
        if (string.IsNullOrEmpty(questID))
        {
            questID = questName + Guid.NewGuid().ToString();
        }
    }
}

[System.Serializable]
public class QuestObjective
{
    public string objectiveID;  // will be used to match with item/mob/etc's ID to link up with quest
    public string description;
    public ObjectiveType type;
    public int requiredAmount;
    public int currentAmount;
    public bool IsCompleted => currentAmount >= requiredAmount;  // if have >= required amount objective is complete
}

public enum ObjectiveType
{
    CollectItem,
    DefeatEnemy,
    ReachLocation,
    TalkNPC,
    Custom
}

[System.Serializable]
public class QuestProgress
{
    public Quest quest;
    public List<QuestObjective> objectives;

    public QuestProgress(Quest quest)
    {
        this.quest = quest;
        objectives = new List<QuestObjective>();

        // Deep copy to avoid modifying original
        foreach (var obj in quest.objectives)
        {
            objectives.Add(new QuestObjective
            {
                objectiveID = obj.objectiveID,
                description = obj.description,
                type = obj.type,
                requiredAmount = obj.requiredAmount,
                currentAmount = 0
            });
        }
    }

    public bool IsCompleted => objectives.TrueForAll(o => o.IsCompleted);
    public string QuestID => quest.questID;
}

[System.Serializable]
public class QuestReward
{
    public RewardType type;
    public int rewardID;  // ex. index from item dictionary
    public int amount = 1;
}

public enum RewardType
{
    Item,
    Gold,
    XP,
    Custom
}