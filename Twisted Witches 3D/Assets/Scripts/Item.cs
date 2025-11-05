using TMPro;
using UnityEngine;
using UnityEngine.UI;

// IDs (+1 in dictionary)
// 0 - Item
// 1 - Health Potion
// 2 - Donut
// 3 - Speed Potion
// 4 - Apple
// 5 - Mushroom
// 6 - Pumpkin
// 7 - Candy
// 8 - Sunflower
// 9 - Magic Energy Potion
// 10 - Wand

public class Item : MonoBehaviour
{
    public int ID;
    public string Name;
    public int quantity;
    public bool removeAfter;
    public bool hasDuration;
    public float duration = 0;
    public GameObject player;  // use to access player movement script without use

    private TMP_Text quantityText;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        if (player != null) playerMovement = player.GetComponent<PlayerMovement>();
        quantityText = GetComponent<TMP_Text>();
        UpdateQuantityDisplay();
    }

    // Updates the display if the item is stacked
    public void UpdateQuantityDisplay()
    {
        if (quantityText != null)
        {
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
        }
    }

    public void AddToStack(int ammount = 1)
    {
        quantity += ammount;
        UpdateQuantityDisplay();
    }

    public int RemoveFromStack(int amount = 1)
    {
        int removed = Mathf.Min(amount, quantity);  // so can't remove more than original amount

        quantity -= removed;
        UpdateQuantityDisplay();
        return removed;
    }

    public void RemoveItem(int amount = 1)
    {
        if (quantity == 1)
        {
            Destroy(gameObject);
        }
        else
        {
            RemoveFromStack(amount);
        }
    }

    // When splitting item stack
    public GameObject CloneItem(int newQuantity)
    {
        GameObject clone = Instantiate(gameObject);

        Item cloneItem = clone.GetComponent<Item>();
        cloneItem.quantity = newQuantity;
        cloneItem.UpdateQuantityDisplay();

        return clone;
    }

    // Using an item...
    public virtual void UseItem()
    {
        switch (ID)
        {
            case 2:  // health potion
                PlayerStats.Instance.IncreaseCurrHealth(10);
                RemoveItem();
                break;
            case 4:  // speed potion
                if (playerMovement != null)
                {
                    playerMovement.SpeedPotion();
                   RemoveItem(); 
                }
                break;
            case 10:  // magic energy potion
                PlayerStats.Instance.DecreaseCurrMagicEnergy(10);
                break;
            case 11:  // wand
                break;
            default:  // food items
                SoundEffectManager.PlaySFX("Eat");
                PlayerStats.Instance.IncreaseCurrHealth(10);
                RemoveItem();
                break;
        }
    }

    // When item gets picked up
    public virtual void ShowPopup()  // may have some items have different pickup later
    {
        Sprite itemIcon = GetComponent<Image>().sprite;

        if (ItemPickUpUIController.Instance != null)
        {
            ItemPickUpUIController.Instance.ShowItemPickUp(Name, itemIcon);
        }
    }
}
