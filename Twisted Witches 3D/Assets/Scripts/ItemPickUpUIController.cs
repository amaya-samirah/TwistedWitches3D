using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickUpUIController : MonoBehaviour
{
    public static ItemPickUpUIController Instance { get; private set; }

    public GameObject popupPrefab;
    public int maxPopups = 5;
    public float popupDuration = 3f;

    private readonly Queue<GameObject> activePopups = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        Debug.Log("Multiple instances of ItemPickUpUIController detected. Destroying the extra one.");
        Destroy(gameObject);
    }

    public void ShowItemPickUp(string itemName, Sprite itemIcon)
    {
        GameObject newPopup = Instantiate(popupPrefab, transform);

        // Get name
        newPopup.GetComponentInChildren<TMP_Text>().text = itemName;

        // Get icon
        Image itemImage = newPopup.transform.GetChild(0)?.GetComponent<Image>();
        if (itemImage) itemImage.sprite = itemIcon;

        activePopups.Enqueue(newPopup);

        // If max pickups reached...
        if (activePopups.Count > maxPopups) Destroy(activePopups.Dequeue());

        // Fade out popups
        StartCoroutine(FadeOutAndDestroy(newPopup));
    }
    
    private IEnumerator FadeOutAndDestroy(GameObject popup)
    {
        yield return new WaitForSeconds(popupDuration);

        if (popup == null) yield break;

        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();
        for (float timePassed = 0; timePassed < 1f; timePassed += Time.deltaTime)
        {
            if (popup == null) yield break;
            canvasGroup.alpha = 1f - timePassed;  // slowly fade out transparancy
            yield return null;
        }

        Destroy(popup);
    }
}
