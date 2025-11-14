using System.Collections.Generic;
using UnityEngine;

// Keeps track of potion information

public class PotionDictionary : MonoBehaviour
{
    // IDs (+1 in dictionary)
    // 0 - Health, cook=15f, ingreidents=[apple, candy, donut]
    // 1 - Speed, cook=20f, ingredients=[sunflower, pumpkin, mushroom]
    // 2 - Magic, cook=15f, ingredients=[health potion, mushroom, candy]

    public List<Item> potionPrefabs;
    private Dictionary<int, GameObject> potionDictionary;

    void Awake()
    {
        potionDictionary = new Dictionary<int, GameObject>();

        for (int i = 0; i < potionPrefabs.Count; i++)
        {
            if (potionPrefabs[i] != null)
            {
                potionPrefabs[i].ID = i + 1;
            }
        }

        foreach (Item potion in potionPrefabs)
        {
            potionDictionary[potion.ID] = potion.gameObject;
        }
    }

    public GameObject GetPotionPrefab(int potionID)
    {
        potionDictionary.TryGetValue(potionID, out GameObject prefab);

        if (prefab == null)
        {
            Debug.LogWarning($"Potion with ID: {potionID} was not found in Potion Dictionary");
        }

        return prefab;
    }
}
