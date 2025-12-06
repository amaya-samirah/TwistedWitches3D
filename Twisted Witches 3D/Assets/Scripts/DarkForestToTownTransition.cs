using UnityEngine;
using UnityEngine.SceneManagement;

public class DarkForestToTownTransition : MonoBehaviour
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
            saveController.SaveGame(new Vector3(5.45f, 0.5f, 87.333f));
            // add save game with new position
            SceneManager.LoadScene(3);  // Twon
        }
    }
}
