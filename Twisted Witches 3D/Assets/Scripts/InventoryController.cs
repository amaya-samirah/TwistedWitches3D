using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;
    private HotbarController hotbarController;

    public static InventoryController Instance { get; private set; }

    public GameObject itemDictionaryObject;
    public GameObject hotbarControllerObject;
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemPrefabs;

    // int = itemID, int = count amount
    Dictionary<int, int> itemsCountCache = new();  // updates when item amounts change to check for quests
    public event Action OnInventoryChanged;  // notifies quest system

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
        if (itemDictionaryObject != null) itemDictionary = itemDictionaryObject.GetComponent<ItemDictionary>();
        // add hotbar script to gamecontroller
        hotbarController = hotbarControllerObject.GetComponent<HotbarController>();

        // Build item counts
        RebuildItemCounts();
    }

    public void RebuildItemCounts()
    {
        itemsCountCache.Clear();

        // Check hotbar
        foreach (Transform slotTransform in hotbarController.hotbarPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();

                if (item != null)
                {
                    // Adding this item's quantity to any existing one inside cache
                    itemsCountCache[item.ID] = itemsCountCache.GetValueOrDefault(item.ID, 0) + item.quantity;
                }
            }
        }

        // Check inventory
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();

                if (item != null)
                {
                    // Adding this item's quantity to any existing one inside cache
                    itemsCountCache[item.ID] = itemsCountCache.GetValueOrDefault(item.ID, 0) + item.quantity;
                }
            }
        }

        // Only want to invoke if this was subscribed to and set to an event
        OnInventoryChanged?.Invoke();  // tells anything subscribed to this event inventory was changed
    }

    public Dictionary<int, int> GetItemCounts() => itemsCountCache;

    public bool AddItem(GameObject itemPrefab)
    {
        Item itemToAdd = itemPrefab.GetComponent<Item>();
        if (itemToAdd == null) return false;

        // Check hotbar
        // Look for slot with same item
        foreach (Transform slotTransform in hotbarController.hotbarPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem != null)
            {
                Item slotItem = slot.currentItem.GetComponent<Item>();
                if (slotItem != null && slotItem.ID == itemToAdd.ID)
                {
                    // Stack
                    slotItem.AddToStack();

                    RebuildItemCounts();

                    return true;
                }
            }
        }

        // Check inventory
        foreach (Transform slotTransform in inventoryPanel.transform)  // for every slot inside inventory panel
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem != null)
            {
                Item slotItem = slot.currentItem.GetComponent<Item>();
                if (slotItem != null && slotItem.ID == itemToAdd.ID)
                {
                    // Stack
                    slotItem.AddToStack();
                    slotItem.transform.localScale = new Vector3(0.5f, 0.5f, 1);

                    RebuildItemCounts();

                    return true;
                }
            }
        }

        // Add items to hotbar 1st if available slot
        foreach (Transform slotTransform in hotbarController.hotbarPanel.transform)  // for every slot inside inventory panel
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slotTransform);
                newItem.transform.localScale = new Vector3(0.5f, 0.5f, 1);  // make item look smaller in slot
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                slot.currentItem = newItem;
                Debug.Log("Added to hotbar.");

                RebuildItemCounts();
                return true;
            }
        }

        // Look for empty slot in inventory
        foreach (Transform slotTransform in inventoryPanel.transform)  // for every slot inside inventory panel
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slotTransform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                slot.currentItem = newItem;

                RebuildItemCounts();
                return true;
            }
        }

        Debug.Log("Inventory is full.");
        return false;  // not added
    }

    // To load saved inventory
    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();
        foreach (Transform slotTransform in inventoryPanel.transform)  // for every slot in inventory panel
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                // GetSiblingIndexgets indecx of game object in conjunction to other game objects on same level (i.e. where they sit on child hierarchy)
                invData.Add(new InventorySaveData
                {
                    itemID = item.ID,
                    slotIndex = slotTransform.GetSiblingIndex(),
                    quantity = item.quantity
                });
            }
        }

        return invData;
    }

    // To save inventory
    public void SetInventoryItems(List<InventorySaveData> inventorySaveData)
    {
        // Clear out inventory panel
        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Create new slots
        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);  // sets inventory penl to parent of slot prefab
        }

        // Populate slots with saved items
        foreach (InventorySaveData data in inventorySaveData)
        {
            if (data.slotIndex < slotCount)  // can actually fit in inventory
            {
                Slot slot = inventoryPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);

                if (itemPrefab != null)
                {
                    GameObject item = Instantiate(itemPrefab, slot.transform);  // creating new object of item prefab from dictionary and putting in in slot
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    Item itemComponent = item.GetComponent<Item>();
                    if (itemComponent != null && data.quantity > 1)  // actually have a stack of multiple items
                    {
                        itemComponent.quantity = data.quantity;
                        itemComponent.UpdateQuantityDisplay();
                    }

                    slot.currentItem = item;
                }
            }
        }

        RebuildItemCounts(); ;
    }

    // To remove item from inventory
    public void RemoveItems(int itemID, int amount)
    {
        // Check hotbar
        foreach (Transform slotTransform in hotbarController.hotbarPanel.transform)
        {
            if (amount <= 0) break;

            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot?.currentItem?.GetComponent<Item>() is Item item && item.ID == itemID)  // if item isn't null grab it and check if ID matches
            {
                int removed = Mathf.Min(amount, item.quantity);
                item.RemoveFromStack(removed);
                amount -= removed;

                // Destory if all items removed
                if (item.quantity == 0)
                {
                    Destroy(slot.currentItem);
                    slot.currentItem = null;
                }
            }
        }
        
        // Check inventory
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            if (amount <= 0) break;

            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot?.currentItem?.GetComponent<Item>() is Item item && item.ID == itemID)  // if item isn't null grab it and check if ID matches
            {
                int removed = Mathf.Min(amount, item.quantity);
                item.RemoveFromStack(removed);
                amount -= removed;

                // Destory if all items removed
                if (item.quantity == 0)
                {
                    Destroy(slot.currentItem);
                    slot.currentItem = null;
                }
            }
        }

        RebuildItemCounts();
    }
}
