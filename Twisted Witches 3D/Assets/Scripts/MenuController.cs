using UnityEngine;

// This handles the backend of the in game menu
public class MenuController : MonoBehaviour
{
    public static MenuController Instance { get; private set; }

    public GameObject menuCanvas;
    public GameObject tabController;
    public GameObject playerStatsController;
    public GameObject dayStatsController;

    private void Awake()
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
        menuCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Open pages
        
        if (Input.GetKeyDown(KeyCode.Tab))  // player tab
        {
            if (!menuCanvas.activeSelf && PauseController.IsGamePaused)
            {
                return;  // wont try to pause if already paused
            }

            menuCanvas.SetActive(!menuCanvas.activeSelf);
            tabController.GetComponent<TabController>().ActivateTab(0);
        }

        if (Input.GetKeyDown(KeyCode.I))  // inventory tab
        {
            if (!menuCanvas.activeSelf && PauseController.IsGamePaused)
            {
                return;
            }

            menuCanvas.SetActive(!menuCanvas.activeSelf);
            tabController.GetComponent<TabController>().ActivateTab(1);
        }

        if (Input.GetKeyDown(KeyCode.Q))  // quests tab
        {
            if (!menuCanvas.activeSelf && PauseController.IsGamePaused)
            {
                return;
            }

            menuCanvas.SetActive(!menuCanvas.activeSelf);
            tabController.GetComponent<TabController>().ActivateTab(2);
        }

        if (Input.GetKeyDown(KeyCode.M))  // map tab
        {
            if (!menuCanvas.activeSelf && PauseController.IsGamePaused)
            {
                return;
            }

            menuCanvas.SetActive(!menuCanvas.activeSelf);
            tabController.GetComponent<TabController>().ActivateTab(3);
        }

        Pause(menuCanvas.activeSelf);
    }

    private void Pause(bool menuActiveSelf)
    {
        PauseController.SetPause(menuActiveSelf);  // sets to if menu is open or closed
        if (playerStatsController != null) playerStatsController.GetComponent<PlayerStatsController>().DisplayPlayerStats(!menuActiveSelf);
        if (dayStatsController != null) dayStatsController.GetComponent<DayStatsController>().DisplayDayStats(!menuActiveSelf);
        //  TODO:
        // - cauldron
    }
}
