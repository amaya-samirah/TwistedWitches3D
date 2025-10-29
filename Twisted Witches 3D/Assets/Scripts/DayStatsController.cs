using System.Collections;
using TMPro;
using UnityEngine;

// Handles the day and time in the game
public class DayStatsController : MonoBehaviour
{
    public GameObject dayStatsPanel;
    public TMP_Text dayText;
    public TMP_Text timeText;
    public TMP_Text AMText;

    private float timeInterval = 10f;  // after this # is when the time will change
    private bool inInterval = false;  // if waiting still waiting for timeInterval to finish

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Register this scene's UI controller with singleton DayStats
        if (DayStats.Instance != null)
        {
            DayStats.Instance.dayStatsController = this;
        }
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (!inInterval)
        {
            StartCoroutine(WaitForTimeInterval());
        }
    }

    private IEnumerator WaitForTimeInterval()
    {
        inInterval = true;
        yield return new WaitForSeconds(timeInterval);
        DayStats.Instance.IncreaseTime();
        inInterval = false;
    }

    public void UpdateUI()
    {
        UpdateDayUI();
        UpdateTimeUI();
    }

    public void UpdateDayUI()
    {
        switch (DayStats.Instance.GetCurrDay())
        {
            case 0:
                dayText.text = "Sunday";
                break;
            case 1:
                dayText.text = "Monday";
                break;
            case 2:
                dayText.text = "Tuesday";
                break;
            case 3:
                dayText.text = "Wednesday";
                break;
            case 4:
                dayText.text = "Thursday";
                break;
            case 5:
                dayText.text = "Friday";
                break;
            case 6:
                dayText.text = "Saturday";
                break;
            default:
                dayText.text = "Sunday";
                break;
        }
    }

    public void UpdateTimeUI()
    {
        string minute;
        if (DayStats.Instance.GetCurrMinute() <= 9)
        {
            minute = "0" + DayStats.Instance.GetCurrMinute().ToString();
        }
        else
        {
            minute = DayStats.Instance.GetCurrMinute().ToString();
        }

        timeText.text = DayStats.Instance.GetCurrHour().ToString() + ":" + minute;

        AMText.text = DayStats.Instance.GetAMTime();
    }

    public void DisplayDayStats(bool show)
    {
        dayStatsPanel.SetActive(show);
    }
}
