using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    public static Vector3 playerPosition;
    public static int currScene;
    public static List<InventorySaveData> inventorySaveData;
    public static List<InventorySaveData> hotbarSaveData;
    public static List<ChestSaveData> chestSaveData;
    public static List<QuestProgress> questProgressData;
    public static List<string> handInQuestIDs;

    // PlayerStats
    public static int xpLevel;
    public static int xpPoints;
    public static float currHealth;
    public static float currMagicEnergy;
    public static int gold;
    public static bool canCastSpells;

    // DayStats
    public static int currDay;
    public static int daysRemaining;
    public static int currHour;
    public static int currMinute;
    public static int AMCount;
}
