using UnityEngine;

public class RewardsController : MonoBehaviour
{
    // Rewards will pop directily into inventory

    public static RewardsController Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void GiveQuestReward(Quest quest)
    {
        if (quest?.questRewards == null)  // no quest rewards for quest
        {
            return;
        }

        foreach (var reward in quest.questRewards)
        {
            switch (reward.type)
            {
                case RewardType.Item:
                    GiveItemReward(reward.rewardID, reward.amount);
                    break;
                case RewardType.Gold:
                    GiveGoldReward(reward.amount);
                    break;
                case RewardType.XP:
                    GiveXPReward(reward.amount);
                    break;
                case RewardType.Custom:
                    break;
                default:
                    break;
            }
        }
    }

    public void GiveItemReward(int itemID, int amount)
    {
        var itemPrefab = FindAnyObjectByType<ItemDictionary>()?.GetItemPrefab(itemID);

        if (itemPrefab == null)
        {
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            if (!InventoryController.Instance.AddItem(itemPrefab))  // item wasn't able to be added
            {
                GameObject dropItem = Instantiate(itemPrefab, transform.position + Vector3.down, Quaternion.identity);
                dropItem.GetComponent<BounceEffect>().StartBounce();
            }
            else
            {
                // Show popup
                itemPrefab.GetComponent<Item>().ShowPopup();
            }
        }
    }

    public void GiveGoldReward(int amount)
    {
        if (amount < 0)  // i.e. making a purchase
        {
            PlayerStats.Instance.DecreaseGold(-amount);  // DecreaseGold() takes in a postive value (so don't do gold - - amount)
        } else
        {
           PlayerStats.Instance.IncreaseGold(amount);
        }
    }

    public void GiveXPReward(int amount)
    {
        PlayerStats.Instance.IncreaseXPPoints(amount);
    }
}
