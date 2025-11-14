using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// This will deal with whether or not the player won or lost the game

public class GameStateController : MonoBehaviour
{
    public GameObject gameOverScreen;
    public GameObject gameWinScreen;

    public static event Action OnReset;  // player restarts the game

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerStats.OnHealthZero += GameOverScreen;
        EvilClone.OnEvilCloneDefeated += GameWinScreen;

        gameOverScreen.SetActive(false);
        gameWinScreen.SetActive(false);
    }

    public void ReturnToMenu()
    {
        // Load to Start Menu
        // DO NOT save progress but start over
        SaveController.Instance.NewGame();
        
        SceneManager.LoadScene(0);
    }

    void GameOverScreen()
    {
        if (gameOverScreen != null) gameOverScreen.SetActive(true);
        PauseController.SetPause(true);
    }

    void GameWinScreen()
    {
        if (gameWinScreen != null) gameWinScreen.SetActive(true);
        PauseController.SetPause(true);
    }
}
