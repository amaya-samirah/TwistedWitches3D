using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public bool IsOpened { get; private set; }
    public string ChestID { get; private set; }
    public GameObject itemPrefab;  // item chest drops
    public int itemCount = 1;
    public GameObject openTopObject;  // when opened switch to open chest object

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChestID ??= GlobalHelper.GenerateUniqueID(gameObject); //UniqueID --> generating is something handy to use in other scripts for saving state
    }

    public bool CanInteract()
    {
        Debug.Log($"Chest is open is: {!IsOpened}");
        return !IsOpened;  // if opened chest is empty
    }

    public void Interact()
    {
        if (!CanInteract()) return;

        // Open chest
        OpenChest();
    }

    private void OpenChest()
    {
        // Set opened
        SetOpened(true);
        SoundEffectManager.PlaySFX("Chest");

        // Drop item
        if (itemPrefab)
        {
            GameObject droppedItem = Instantiate(itemPrefab, transform.position + new Vector3(0, 0.15f, -0.5f), Quaternion.identity);
            droppedItem.GetComponent<BounceEffect>().StartBounce();
        }
    }
    
    public void SetOpened(bool opened)
    {
        if (IsOpened = opened)  // sets IsOpened then checks value
        {
            GameObject closedTop = transform.GetChild(1).gameObject;

            if (closedTop != null && openTopObject != null)
            {
                var openTop = Instantiate(openTopObject, closedTop.transform.parent);

                Destroy(closedTop);
            }
        }
    }
}
