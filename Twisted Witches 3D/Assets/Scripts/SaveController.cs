using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveController : MonoBehaviour
{
    public static SaveController Instance { get; private set; }
    public GameObject inventoryObject;
    public GameObject hotbarObject;
    public GameObject playerStatsObject;
    public GameObject dayStatsObject;
    public List<GameObject> chestObjects;

    private string saveLocation;
    private InventoryController inventoryController;
    private HotbarController hotbarController;
    private PlayerStatsController playerStatsController;
    private DayStatsController dayStatsController;
    private Chest[] chests;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeComponents();

        LoadGame();
    }

    private void InitializeComponents()
    {
        // Define saveLocation
        saveLocation = Path.Combine(Application.persistentDataPath, "savedData.JSON");
        // inventoryController = FindAnyObjectByType<InventoryController>();
        // hotbarController = FindAnyObjectByType<HotbarController>();
        // chests = FindObjectsOfType<Chest>();
        // playerStatsController = FindAnyObjectByType<PlayerStatsController>();
        // dayStatsController = FindAnyObjectByType<DayStatsController>();

        inventoryController = inventoryObject.GetComponent<InventoryController>();
        hotbarController = hotbarObject.GetComponent<HotbarController>();
        playerStatsController = playerStatsObject.GetComponent<PlayerStatsController>();
        dayStatsController = dayStatsObject.GetComponent<DayStatsController>();

        // Get chests
        chests = new Chest[chestObjects.Count];
        for (int i = 0; i < chestObjects.Count; i++)
        {
            if (chestObjects[i] != null)
            {
                chests[i] = chestObjects[i].GetComponent<Chest>();
            }
        }
    }

    public void NewGame()
    {
        // Delete save file
        if (File.Exists(saveLocation)) File.Delete(saveLocation);
    }

    // Save game - set position
    public void SaveGame(Vector3 playerPos)
    {
        SaveData saveData = new SaveData
        {
            // Initalize what saveData needs to be
            playerPosition = playerPos,
            currScene = SceneManager.GetActiveScene().buildIndex,  // the current scene player is on
            inventorySaveData = inventoryController.GetInventoryItems(),
            hotbarSaveData = hotbarController.GetHotBarItems(),
            chestSaveData = GetChestState(),  // Get chest save
            questProgressData = QuestController.Instance.activeQuests,
            handInQuestIDs = QuestController.Instance.handInQuestIDs,
            xpLevel = PlayerStats.Instance.GetXPLevel(),
            xpPoints = PlayerStats.Instance.GetXPPoints(),
            currHealth = PlayerStats.Instance.GetCurrHealth(),
            currMagicEnergy = PlayerStats.Instance.GetCurrMagicEnergy(),
            gold = PlayerStats.Instance.GetGold(),
            canCastSpells = PlayerStats.Instance.GetCanCastSpells(),
            currDay = DayStats.Instance.GetCurrDay(),
            daysRemaining = DayStats.Instance.GetDaysRemaining(),
            currHour = DayStats.Instance.GetCurrHour(),
            currMinute = DayStats.Instance.GetCurrMinute(),
            AMCount = DayStats.Instance.GetAMCount()
        };

        GameData.playerPosition = saveData.playerPosition;
        GameData.currScene = saveData.currScene;
        GameData.inventorySaveData = saveData.inventorySaveData;
        GameData.hotbarSaveData = saveData.hotbarSaveData;
        GameData.chestSaveData = saveData.chestSaveData;
        GameData.questProgressData = saveData.questProgressData;
        GameData.handInQuestIDs = saveData.handInQuestIDs;
        GameData.xpLevel = saveData.xpLevel;
        GameData.xpPoints = saveData.xpPoints;
        GameData.currHealth = saveData.currHealth;
        GameData.currMagicEnergy = saveData.currMagicEnergy;
        GameData.gold = saveData.gold;
        GameData.canCastSpells = saveData.canCastSpells;
        GameData.currDay = saveData.currDay;
        GameData.daysRemaining = saveData.daysRemaining;
        GameData.currHour = saveData.currHour;
        GameData.currMinute = saveData.currMinute;
        GameData.AMCount = saveData.AMCount;

        // Wrtie to text file
        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
    }

    // Save game - regular
    public void SaveGame()  // public so button can call this
    {
        SaveData saveData = new SaveData
        {
            // Initalize what saveData needs to be
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
            currScene = SceneManager.GetActiveScene().buildIndex,  // the current scene player is on
            inventorySaveData = inventoryController.GetInventoryItems(),
            hotbarSaveData = hotbarController.GetHotBarItems(),
            chestSaveData = GetChestState(),  // Get chest save
            questProgressData = QuestController.Instance.activeQuests,
            handInQuestIDs = QuestController.Instance.handInQuestIDs,
            xpLevel = PlayerStats.Instance.GetXPLevel(),
            xpPoints = PlayerStats.Instance.GetXPPoints(),
            currHealth = PlayerStats.Instance.GetCurrHealth(),
            currMagicEnergy = PlayerStats.Instance.GetCurrMagicEnergy(),
            gold = PlayerStats.Instance.GetGold(),
            canCastSpells = PlayerStats.Instance.GetCanCastSpells(),
            currDay = DayStats.Instance.GetCurrDay(),
            daysRemaining = DayStats.Instance.GetDaysRemaining(),
            currHour = DayStats.Instance.GetCurrHour(),
            currMinute = DayStats.Instance.GetCurrMinute(),
            AMCount = DayStats.Instance.GetAMCount()
        };

        GameData.playerPosition = saveData.playerPosition;
        GameData.currScene = saveData.currScene;
        GameData.inventorySaveData = saveData.inventorySaveData;
        GameData.hotbarSaveData = saveData.hotbarSaveData;
        GameData.chestSaveData = saveData.chestSaveData;
        GameData.questProgressData = saveData.questProgressData;
        GameData.handInQuestIDs = saveData.handInQuestIDs;
        GameData.xpLevel = saveData.xpLevel;
        GameData.xpPoints = saveData.xpPoints;
        GameData.currHealth = saveData.currHealth;
        GameData.currMagicEnergy = saveData.currMagicEnergy;
        GameData.gold = saveData.gold;
        GameData.canCastSpells = saveData.canCastSpells;
        GameData.currDay = saveData.currDay;
        GameData.daysRemaining = saveData.daysRemaining;
        GameData.currHour = saveData.currHour;
        GameData.currMinute = saveData.currMinute;
        GameData.AMCount = saveData.AMCount;

        // Wrtie to text file
        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
    }

    private List<ChestSaveData> GetChestState()
    {
        List<ChestSaveData> chestStates = new List<ChestSaveData>();

        foreach (Chest chest in chests)
        {
            ChestSaveData chestSaveData = new ChestSaveData
            {
                chestID = chest.ChestID,
                IsOpened = chest.IsOpened
            };

            chestStates.Add(chestSaveData);
        }

        return chestStates;
    }

    // Load game
    public void LoadGame()
    {
        if (File.Exists(saveLocation))  // check for save file
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));  // read this file in this save location
            GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPosition;

            inventoryController.SetInventoryItems(saveData.inventorySaveData);
            hotbarController.SetHotbarItems(saveData.hotbarSaveData);
            inventoryController.RebuildItemCounts();

            // Load chest state
            LoadChestStates(saveData.chestSaveData);

            QuestController.Instance.LoadQuestProgresss(saveData.questProgressData);
            QuestController.Instance.handInQuestIDs = saveData.handInQuestIDs;

            // Set values before updating UI
            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.LoadState(saveData.xpLevel, saveData.xpPoints, saveData.currHealth, saveData.currMagicEnergy, saveData.gold, saveData.canCastSpells);
            }

            if (DayStats.Instance != null)
            {
                DayStats.Instance.LoadState(saveData.currDay, saveData.daysRemaining, saveData.currHour, saveData.currMinute, saveData.AMCount);
            }

            playerStatsController.UpdatePages();

            dayStatsController.UpdateUI();

        }
        else  // if don't have save file in location
        {
            SaveGame();  // save game and create new file
                         // whatever defeault values are when 1st load up will be initial save point
            inventoryController.SetInventoryItems(new List<InventorySaveData>());
            hotbarController.SetHotbarItems(new List<InventorySaveData>());
        }
    }

    private void LoadChestStates(List<ChestSaveData> chestStates)
    {
        foreach (Chest chest in chests)
        {
            ChestSaveData chestSaveData = chestStates.FirstOrDefault(c => c.chestID == chest.ChestID);  // match on chest id and grab save data

            if (chestSaveData != null)
            {
                chest.SetOpened(chestSaveData.IsOpened);
            }
        }
    }

    // Saves game before closing
    public void SaveAndQuit()
    {
        SaveGame();
        Application.Quit();
    }
    
    // Closing game without saving
    public void QuitGame()
    {
        Application.Quit();
    }
}
