using UnityEngine;
using UnityEngine.SceneManagement;

public class TownToHomeLotTransition : MonoBehaviour
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
            saveController.SaveGame(new Vector3(5.4f, 0.5f, 86.3f));
            // add save game with new position
            SceneManager.LoadScene(1);  // Home Lot
        }
    }
}
