using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarController : MonoBehaviour
{
    public GameObject hotbarPanel;
    public GameObject slotPrefab;
    public GameObject itemDictionaryObject;
    public int slotCount = 9;  // 1-9 on keyboard
    public Item usingItem { get; private set; }

    private ItemDictionary itemDictionary;

    private Key[] hotbarKeys;

    void Awake()
    {
        itemDictionary = itemDictionaryObject.GetComponent<ItemDictionary>();

        hotbarKeys = new Key[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            // If slot less than 9 store keye of number of slot + 1
            hotbarKeys[i] = i < 9 ? (Key)((int)Key.Digit1 + i) : Key.Digit0;  // slot 0 is first slot on hotbar so need to set to digit 1
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check for key presses
        for (int i = 0; i < slotCount; i++)
        {
            if (Keyboard.current[hotbarKeys[i]].wasPressedThisFrame)
            {
                // Use item
                UseItemInSlot(i);
                Debug.Log("Keyboard pressed.");
            }
        }
    }

    void UseItemInSlot(int index)
    {
        Slot slot = hotbarPanel.transform.GetChild(index).GetComponent<Slot>();  // get child matching index being pressed

        if (slot.currentItem != null)
        {
            Item item = slot.currentItem.GetComponent<Item>();
            if (item.hasDuration)
            {
                StartCoroutine(DurationRemove(slot, item));
                return;
            }
            item.UseItem();
            usingItem = item;
        }
        else
        {
            usingItem = null;
            Debug.Log("slot.currentItem is null");
        }
    }

    IEnumerator DurationRemove(Slot slot, Item item)
    {
        item.UseItem();
        Debug.Log("Waiting for duration.");
        if (item.quantity == 1)
        {
            RemoveItem(slot, item);
        }
        else
        {
            item.RemoveFromStack();
        }
        yield return new WaitForSeconds(item.duration);
    }

    private void RemoveItem(Slot slot, Item item)
    {
        Debug.Log($"Removing item: {slot.currentItem.name}.");
        if (slot.currentItem != null && item != null)
        {
            if (item.quantity == 1)
            {
                Destroy(slot.currentItem);
            }
            else
            {
                item.RemoveFromStack();
            }
        }
    }

    // Load saved hotbar
    public List<InventorySaveData> GetHotBarItems()
    {
        List<InventorySaveData> hotbarData = new List<InventorySaveData>();
        foreach (Transform slotTransform in hotbarPanel.transform)  // for every slot in inventory panel
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                // GetSiblingIndexgets index of game object in conjunction to other game objects on same level (i.e. where they sit on child hierarchy)
                hotbarData.Add(new InventorySaveData { itemID = item.ID, slotIndex = slotTransform.GetSiblingIndex() });
            }
        }

        return hotbarData;
    }
    
    // Save hotbar
    public void SetHotbarItems(List<InventorySaveData> inventorySaveData)
    {
        // Clear out inventory panel
        foreach (Transform child in hotbarPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Create new slots
        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, hotbarPanel.transform);  // sets hotbar panel to parent of slot prefab
        }

        // Populate slots with saved items
        foreach (InventorySaveData data in inventorySaveData)
        {
            if (data.slotIndex < slotCount)  // can actually fit in hotbar
            {
                Slot slot = hotbarPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);

                if (itemPrefab != null)
                {
                    Vector3 smallerScale = new Vector3(0.5f, 0.5f, 1);
                    GameObject item = Instantiate(itemPrefab, slot.transform);  // creating new object of item prefab from dictionary and putting in in slot
                    item.transform.localScale = smallerScale;
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    slot.currentItem = item;
                }
            }
        }
    }
}
