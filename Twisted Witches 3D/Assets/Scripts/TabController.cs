using UnityEngine;
using UnityEngine.UI;

// TABS:
// 0 - Player
// 1 - Inventory
// 2 - Quests
// 3 - Map
// 4 - Settings
public class TabController : MonoBehaviour
{
    public Image[] tabImages;
    public GameObject[] pages;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ActivateTab(0);  // player page opens 1st
    }

    public void ActivateTab(int tab)
    {
        SoundEffectManager.PlaySFX("Button");  // play button click sound effect
        
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);

            Color newColor;
            tabImages[i].color = ColorUtility.TryParseHtmlString("d0c1b3", out newColor) ? tabImages[i].color = newColor : tabImages[i].color = Color.grey;
        }

        pages[tab].SetActive(true);
        tabImages[tab].color = Color.white;  // will be transparent
    }
}
