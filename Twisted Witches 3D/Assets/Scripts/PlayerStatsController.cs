using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsController : MonoBehaviour
{
    public TMP_Text xpLevelText;
    public TMP_Text goldAmountText;
    public GameObject healthBarObject;
    public GameObject magicBarObject;
    public GameObject goldPanel;
    public Sprite[] healthBars; // 0 = empty,...,10 = full
    public Sprite[] magicBars;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdatePlayerPage();
        UpdateStatsUI();
        UpdateGoldPanel();
    }
    
    public void UpdateStatsUI()
    {
        // Health bar
        ChangeStatBar(healthBarObject, healthBars, PlayerStats.Instance.GetCurrHealth());

        // Magic bar
        ChangeStatBar(magicBarObject, magicBars, PlayerStats.Instance.GetCurrMagicEnergy());
    }

    private void ChangeStatBar(GameObject bar, Sprite[] bars, float amount)
    {
        // Change sprite when stat doesn't have decimal (i.e will change sprite when health = 9 not 9.5)
        int tempAmount = (int)amount;
        if (amount % tempAmount != 0)
        {
            return;
        }

        if (tempAmount > 10)
        {
            tempAmount = 10;
        }
        else if (tempAmount < 0)
        {
            tempAmount = 0;
        }
        
        // Change UI
        bar.GetComponent<Image>().sprite = bars[tempAmount];
    }

    public void UpdateGoldPanel()
    {
        goldAmountText.text = PlayerStats.Instance.GetGold().ToString();
    }

    public void UpdatePlayerPage()
    {
        xpLevelText.text = PlayerStats.Instance.GetXPLevel().ToString();
    }

    // Hide or show player stats
    public void DisplayPlayerStats(bool show)
    {
        healthBarObject.SetActive(show);
        magicBarObject.SetActive(show);
        goldPanel.SetActive(show);
    }
    
    public void UpdatePages()
    {
        UpdatePlayerPage();
        UpdateStatsUI();
        UpdateGoldPanel();
    }
}
