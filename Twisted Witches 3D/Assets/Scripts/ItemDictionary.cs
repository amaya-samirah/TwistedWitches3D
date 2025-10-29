using System.Collections.Generic;
using UnityEngine;

// INDEX
// 1 - Item
// 2 - Health Potion
// 3 - Donut
// 4 - Speed Potion
// 5 - Apple
// 6 - Mushroom
// 7 - Pumpkin
// 8 - Candy
// 9 - Sunflower
// 10 - Magic Energy Potion
// 11 - Wand

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
