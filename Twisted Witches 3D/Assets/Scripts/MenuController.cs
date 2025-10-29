using UnityEngine;

// This handles the backend of the in game menu
public class MenuController : MonoBehaviour
{
    public static MenuController Instance { get; private set; }

    public GameObject menuCanvas;
    public GameObject tabController;

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
            // TODO:
            // - add checking if canvas is active && game paused (--> return)
            menuCanvas.SetActive(!menuCanvas.activeSelf);
            tabController.GetComponent<TabController>().ActivateTab(0);
        }

        if (Input.GetKeyDown(KeyCode.I))  // inventory tab
        {
            menuCanvas.SetActive(!menuCanvas.activeSelf);
            tabController.GetComponent<TabController>().ActivateTab(1);
        }

        if (Input.GetKeyDown(KeyCode.Q))  // quests tab
        {
            menuCanvas.SetActive(!menuCanvas.activeSelf);
            tabController.GetComponent<TabController>().ActivateTab(2);
        }

        if (Input.GetKeyDown(KeyCode.M))  // map tab
        {
            menuCanvas.SetActive(!menuCanvas.activeSelf);
            tabController.GetComponent<TabController>().ActivateTab(3);
        }

        Pause(menuCanvas.activeSelf);
    }

    private void Pause(bool menuActiveSelf)
    {
        // TODO:
        // - implement openeing menu pases game
    }
}
