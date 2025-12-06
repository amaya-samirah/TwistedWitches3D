using System.Collections.Generic;
using UnityEngine;

// INDEX
// 0 - Item
// 1 - Health Potion
// 2 - Apple
// 3 - Donut
// 4 - Candy
// 5 - Mushroom
// 6 - Pumpkin
// 7 - Sunflower
// 8 - Purple Wand
// 9 - Magic Energy Potion
// 10 - Speed Potion

public class ItemDictionary : MonoBehaviour
{
    public List<Item> itemPrefabs;

    // int = ID, GameObject = item prefab
    private Dictionary<int, GameObject> itemDictionary;

    void Awake()
    {
        // Populate dictionary

        itemDictionary = new Dictionary<int, GameObject>();

        for (int i = 0; i < itemPrefabs.Count; i++)
        {
            if (itemPrefabs[i] != null)
            {
                itemPrefabs[i].ID = i + 1;
            }
        }

        foreach (Item item in itemPrefabs)
        {
            itemDictionary[item.ID] = item.gameObject;
        }
    }

    public GameObject GetItemPrefab(int itemID)
    {
        // Check exists
        itemDictionary.TryGetValue(itemID, out GameObject prefab);
        if (prefab == null)
        {
            Debug.LogWarning($"Item with ID {itemID} was not found in dictionary.");
        }
        return prefab;
    }
}
