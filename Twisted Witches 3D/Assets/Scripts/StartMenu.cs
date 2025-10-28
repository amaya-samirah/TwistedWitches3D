using UnityEngine;
using UnityEngine.SceneManagement;

// SCENE LIST:
// 0 - Start Menu
// 1 - Home Lot
// 2 - Home (interior)
// 3 - Town
// 4 - Dark Forest

// The main functionality for the Start Menu UI
public class StartMenu : MonoBehaviour
{
    // Called by Play button
    public void PlayButton()
    {
        SceneManager.LoadScene(1);
    }

    // Called by New Game button
    public void NewGameButton()
    {
        // TODO:
        // - create new game (save controller)
        SceneManager.LoadScene(1);
    }

    // Called by Quit button
    public void QuitButton()
    {
        Application.Quit();
    }
}
