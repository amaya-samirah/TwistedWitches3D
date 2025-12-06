using UnityEngine;
using UnityEngine.SceneManagement;

public class TownToDarkForestTransition : MonoBehaviour
{
    private SaveController saveController;

    void Awake()
    {
        saveController = FindAnyObjectByType<SaveController>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Save new position
            saveController.SaveGame(new Vector3(-7.642f, 0.5f, 87.499f));
            // add save game with new position
            SceneManager.LoadScene(4);  // Dark Forest
        }
    }
}
