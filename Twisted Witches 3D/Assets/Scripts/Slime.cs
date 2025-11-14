using System.Collections;
using UnityEngine;

public class Slime : MonoBehaviour, IInteractable
{
    public Transform player;
    public float chaseSpeed = 0.5f;
    public GameObject itemDrop;
    public GameObject hotbarObject;
    public float damage = 0.5f;  // damage slime deals to player
    public bool inRange = false;

    private Animator animator;
    private HotbarController hotbarController;
    private bool moveToPlayer = false;
    private bool isAlive = true;  // if player is in 'hit' range then they can interact
    private int health = 10;

    void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        hotbarController = hotbarObject.GetComponent<HotbarController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Stop moving if game paused
        if (PauseController.IsGamePaused)
        {
            Vector3 stopMovement = new Vector3(0, 0, 0);
            AnimateMovement(stopMovement);
            return;
        }

        // If player is in the mob's detection range (i.e. when mob "notices" player)
        if (moveToPlayer)
        {
            MoveToPlayer();
        }


        float horizontal = gameObject.transform.position.x;
        float vertical = gameObject.transform.position.y;

        Vector3 direction = new Vector3(horizontal, 0, vertical);

        AnimateMovement(direction);
    }

    public bool CanInteract()
    {
        return isAlive && PlayerStats.Instance.GetCanCastSpells();  // if slime is not already defeatead, if player has magic energy, and if player is using a magical item
    }
    public void Interact()
    {
        if (!CanInteract() && hotbarController.usingItem != null && !hotbarController.usingItem.Name.Contains("Wand"))
        {
            Debug.Log("Can't interact with slime.");
            return;
        }

        int damage = 3;
        if (health - damage <= 0)
        {
            DefeatSlime();
            return;
        }

        StartCoroutine(Attacked(damage));
        Debug.Log($"New Health: {health}");
    }

    void MoveToPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, (chaseSpeed * Time.deltaTime)/2);
    }

    void AnimateMovement(Vector3 direction)
    {
        if (animator != null)
        {
            if (direction.magnitude > 0)
            {
                animator.SetBool("isMoving", true);

                // decide which animation
                animator.SetFloat("horizontal", direction.x);
                animator.SetFloat("vertical", direction.z);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        moveToPlayer = true;
    }

    IEnumerator Attacked(int damage)
    {
        if (animator != null && UsedMagicItem(damage))  // checks not null and if the player has a magic item to use
        {
            animator.SetBool("isHit", true);
            yield return new WaitForSeconds(1f);
            animator.SetBool("isHit", false);
        }
    }

    private bool UsedMagicItem(int damage)
    {
        if (hotbarController.usingItem != null && hotbarController.usingItem.Name.Contains("Wand"))
        {
            Debug.Log("Slime hit");

            Wand wand = hotbarController.usingItem.GetComponent<Wand>();

            wand.DecreaseDurability();
            
            health -= damage;

            PlayerStats.Instance.DecreaseCurrMagicEnergy();

            return true;
        }

        return false;
    }

    private void DefeatSlime()
    {
        Debug.Log("Slime defeated.");
        isAlive = false;
        moveToPlayer = false;
        Destroy(gameObject);

        int probability = Random.Range(1, 4);  // 33% chance of dropping an item
        if (probability == 1)
        {
            GameObject droppedItem = Instantiate(itemDrop, transform.position + Vector3.back, Quaternion.identity);  // item appears 1 down from chest position
            droppedItem.GetComponent<BounceEffect>().StartBounce();
        }
    }
}
