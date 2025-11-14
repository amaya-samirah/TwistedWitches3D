using System.Collections.Generic;
using UnityEngine;

// This deals with the inofrmation associated with CREATING potions
public class Potion : MonoBehaviour
{
    // IDs (+1 in dictionary)
    // 0 - Health, cook=15f, ingreidents=[apple, candy, donut]
    // 1 - Speed, cook=20f, ingredients=[sunflower, pumpkin, mushroom]
    // 2 - Magic, cook=15f, ingredients=[health potion, mushroom, candy]

    public int ID;
    public string Name;
    public List<Item> ingredients;
    public int IngredientsInCauldronCount = 0;  // used to keep track of whether or not the ingredients in the cauldron match this potion
    public float cookTime = 15f;  // in seconds
}
