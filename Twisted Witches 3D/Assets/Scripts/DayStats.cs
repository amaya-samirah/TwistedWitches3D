using UnityEngine;

public class DayStats : MonoBehaviour
{
    // DAYS:
    // 0 - Sunday
    // 1 - Monday
    // 2 - Tuesday
    // 3 - Wednesday
    // 4 - Thursday
    // 5 - Friday
    // 6 - Saturday

    public static DayStats Instance { get; private set; }

    public DayStatsController dayStatsController;

    private int currDay = 0;
    private int daysRemaining = 1;  // totalDays - currDay
    private int currHour = 8;
    private int currMinute = 0;
    private int AMCount = 0;  // to tell if it's the next day
    private string[] AMText = { "AM", "PM" };  // count[0] = am, count[1] = pm

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Change day
    public void NextDay()
    {
        if (daysRemaining != 0)
        {
            currDay++;
            daysRemaining--;
        }

        dayStatsController.UpdateDayUI();
    }

    public void IncreaseTime()
    {
        // Hour = [00,12], Minute = [0,50] (10 min intervals)
        switch (currMinute)
        {
            case 50:  // __:50
                currMinute = 0;
                switch (currHour)
                {
                    case 12:  // 12:__
                        currHour = 1;
                        AMCount++;
                        break;
                    default:
                        currHour++;
                        break;
                }
                break;
            default:
                currMinute += 10;
                break;
        }

        if (AMCount >= 2)
        {
            AMCount = 0;
            NextDay();
        }

        dayStatsController.UpdateTimeUI();

        // Increase player magic energy
        if (currMinute == 30)
        {
            PlayerStats.Instance.IncreaseCurrMagicEnergy(1);
        }
    }

    // To load when opening scene
    public void LoadState(int currDay, int daysRemaining, int currHour, int currMinute, int AMCount)
    {
        this.currDay = currDay;
        this.daysRemaining = daysRemaining;
        this.currHour = currHour;
        this.currMinute = currMinute;
        this.AMCount = AMCount;
    }

    // Getters
    public int GetCurrDay() { return currDay; }
    public int GetDaysRemaining() { return daysRemaining; }
    public int GetCurrHour() { return currHour; }
    public int GetCurrMinute() { return currMinute; }
    public int GetAMCount() { return AMCount; }
    public string GetAMTime() { return AMText[AMCount]; }
}
