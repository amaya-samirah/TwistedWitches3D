using UnityEngine;

public class Wand : Item
{
    // public int ID;
    // public string Name;
    public int damageAmount = 2;

    // How many hits it has until it breaks
    public int durability = 100;  // set to 4 for the wand quest...full durability = 100

    public void DecreaseDurability(int amt = 1)
    {
        int newDurability = durability -= amt;
        Debug.Log($"{Name} Durability: {durability}");

        // Durability has reached 0...
        if (newDurability <= 0)
        {
            Destroy(transform.GetComponentInParent<Slot>().currentItem);
            Destroy(this);
            return;
        }

        durability = newDurability;
    }
    
    public void SetDurabilityFull()
    {
        durability = 100;
    }
}
