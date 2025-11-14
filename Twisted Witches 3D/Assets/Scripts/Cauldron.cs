using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : MonoBehaviour, IInteractable
{
    public GameObject cauldronPanel;
    public GameObject[] slots;
    public GameObject potionDictionary;
    public bool inUse { get; private set; }
    private List<Item> IngredientsInCauldron;
    private int slotCount = 3;
    private Animator animator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        IngredientsInCauldron = new List<Item>();
    }

    public bool CanInteract()
    {
        return !inUse;
    }

    public void Interact()
    {
        ShowCookPage();
    }

    public void ShowCookPage()
    {
        cauldronPanel.SetActive(true);
    }

    public void HideCookPage()  // going to set to an 'X' button
    {
        cauldronPanel.SetActive(false);
    }

    public void CheckSlotsFull()  // on check click in cauldron panel
    {
        int count = 0;
        for (int i = 0; i < slotCount; i++)
        {
            Slot slot = slots[i].GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                IngredientsInCauldron.Add(slot.currentItem.GetComponent<Item>());
                count++;
            }
        }

        if (count == 3)
        {
            int potionID = CheckForCorrectIngredients();
            if (potionID < 0)  // doesn't have all ingredients
            {
                // TODO:
                // - do something...
                Debug.Log("No potions can be made with these ingredients.");
            }
            else
            {
                HideCookPage();
                DestoryIngredientsInCauldron();
                Using(potionID);
            }
        }
        else
        {
            Debug.Log("Empty slots detected.");
            return;
        }
    }

    private void DestoryIngredientsInCauldron()
    {
        for (int i = 0; i < slotCount; i++)
        {
            Slot slot = slots[i].GetComponent<Slot>();

            Destroy(slot.transform.GetChild(0).gameObject);
            slot.currentItem = null;

        }
    }

    public int CheckForCorrectIngredients()
    {
        List<Item> potions = potionDictionary.GetComponent<PotionDictionary>().potionPrefabs;

        foreach (Item item in potions)
        {
            Potion potion = item.GetComponent<Potion>();

            // Return ingredients in cauldron count to 0
            potion.IngredientsInCauldronCount = 0;

            foreach (Item ingredient in IngredientsInCauldron)
            {
                for (int i = 0; i < potion.ingredients.Count; i++)
                {
                    if (potion.ingredients[i].Name == ingredient.Name)
                    {
                        potion.IngredientsInCauldronCount++;
                    }
                }
            }

            if (potion.IngredientsInCauldronCount == potion.ingredients.Count)
            {
                Debug.Log($"Making {potion.Name} potion.");
                return potion.ID;
            }
        }

        return -1;  // means ingredients don't match potion
    }

    public void Using(int potionID)
    {
        StartCoroutine(BrewPotion(potionDictionary.GetComponent<PotionDictionary>().potionPrefabs[potionID]));
    }

    private IEnumerator BrewPotion(Item potion)
    {
        inUse = true;
        animator.SetBool("inUse", true);
        yield return new WaitForSeconds(potion.GetComponent<Potion>().cookTime);
        inUse = false;
        animator.SetBool("inUse", false);

        // Drop item
        SoundEffectManager.PlaySFX("PotionMade");
        GameObject droppedItem = Instantiate(potion.gameObject, transform.position + Vector3.down, Quaternion.identity);
        droppedItem.GetComponent<BounceEffect>().StartBounce();
    }
}
