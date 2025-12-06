using UnityEngine;

public class Gold : MonoBehaviour
{
    public int goldAmount = 50;

    private void OnTriggerEnter(Collider collision)
    {
        int tempAmount = goldAmount;

        // "Pick up" item
        Destroy(gameObject);

        // Add to player stat
        PlayerStats.Instance.IncreaseGold(tempAmount);
    }
}
