using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public int currScene;
    public List<InventorySaveData> inventorySaveData;
    public List<InventorySaveData> hotbarSaveData;
    public List<ChestSaveData> chestSaveData;
    public List<QuestProgress> questProgressData;
    public List<string> handInQuestIDs;

    // PlayerStats
    public int xpLevel;
    public int xpPoints;
    public float currHealth;
    public float currMagicEnergy;
    public int gold;
    public bool canCastSpells;

    // DayStats
    public int currDay;
    public int daysRemaining;
    public int currHour;
    public int currMinute;
    public int AMCount;
}

[System.Serializable]
public class ChestSaveData
{
    public string chestID;
    public bool IsOpened;
}
