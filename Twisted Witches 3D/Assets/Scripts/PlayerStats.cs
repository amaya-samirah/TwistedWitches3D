using System;
using UnityEngine;

// Handles all the stats attatched to the player
public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }
    public static event Action OnHealthZero;
    public GameObject player;
    public PlayerStatsController playerStatsController;

    private int xpLevel = 1;  // the level the player has
    private int xpPoints = 0;  // the points the player has towards the next level
    private int pointsPerLevel = 100;  // 100 points per level  ??
    private float currHealth = 10;  // starting
    private float fullHealth = 10;  // will increase as player levels up
    private float currMagicEnergy = 10;  // starting
    private float fullMagicEnergy = 10;
    private int gold = 100;  // starting
    private bool canCastSpells = true;  // as long as player has magic energy

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // When taking damage
    public void DecreaseCurrHealth(float amount = 1f)
    {
        float newHealth = currHealth - amount;

        // If player looses all health
        if (newHealth <= 0 || newHealth == 0)
        {
            OnHealthZero.Invoke();
            currHealth = 0;
            playerStatsController.UpdateStatsUI();
            return;
        }

        currHealth = newHealth;
        playerStatsController.UpdateStatsUI();
    }

    // Increasing health
    public void IncreaseCurrHealth(float amount = 1f)
    {
        float newHealth = currHealth - amount;

        // Check health not full
        if (newHealth == fullHealth)
        {
            return;
        }
        else if (newHealth > fullHealth)  // check health doesn't go over
        {
            currHealth = fullHealth;
            playerStatsController.UpdateStatsUI();
            return;
        }

        currHealth = newHealth;
        playerStatsController.UpdateStatsUI();
    }

    // When using a magical item
    public void DecreaseCurrMagicEnergy(float amount = 1f)
    {
        float newEngergy = currMagicEnergy - amount;

        // Player doesn't have enough energy
        if (newEngergy < 0)
        {
            // TODO:
            // - add implementation when not enough energy
            return;
        }

        // Player has no more energy
        if (newEngergy == 0)
        {
            canCastSpells = false;
        }

        playerStatsController.UpdateStatsUI();
    }

    // Increase energy
    public void IncreaseCurrMagicenergy(float amount = 1)
    {
        float newEngergy = currMagicEnergy + amount;

        // Check energy not full
        if (currMagicEnergy == fullMagicEnergy) return;
        else if (newEngergy > fullMagicEnergy)
        {
            currMagicEnergy = newEngergy;
            playerStatsController.UpdateStatsUI();
            return;
        }

        // Increase...
        currMagicEnergy = newEngergy;
        playerStatsController.UpdateStatsUI();

        // Allow player to use magic again
        if (currMagicEnergy > 0)
        {
            canCastSpells = true;
        }
    }

    public void IncreaseGold(int amount)
    {
        SoundEffectManager.PlaySFX("Gold");
        gold += amount;
        playerStatsController.UpdateGoldPanel();
    }

    public void DecreaseGold(int amount)
    {
        int newGold = gold -= amount;
        // Check that gold won't be negative
        if (newGold < 0)
        {
            return;
        }
        gold = newGold;
        playerStatsController.UpdateGoldPanel();
    }

    public void IncreaseXPPoints(int amount)
    {
        // If reached threashold (100) --> raise xp level and reset points to 0
        if ((xpPoints += amount) >= pointsPerLevel)
        {
            IncreaseXPLevel();
        }

        // Increase points
        xpPoints += amount;
    }

    private void IncreaseXPLevel()
    {
        xpLevel++;
        playerStatsController.UpdatePlayerPage();
    }

    // To load data when opening scene
    public void LoadState(int xpLevel, int xpPoints, int currHealth, int currMagicEnergy, int gold, bool canCastSpells)
    {
        this.xpLevel = xpLevel;
        this.xpPoints = xpPoints;
        this.currHealth = currHealth;
        this.currMagicEnergy = currMagicEnergy;
        this.gold = gold;
        this.canCastSpells = canCastSpells;
    }

    // Getters
    public int GetXPLevel() { return xpLevel; }
    public int GetXPPoints() { return xpPoints; }
    public float GetCurrHealth() { return currHealth; }
    public float GetCurrMagicEnergy() { return currMagicEnergy; }
    public int GetGold() { return gold; }
    public bool GetCanCastSpells() { return canCastSpells; }
}
