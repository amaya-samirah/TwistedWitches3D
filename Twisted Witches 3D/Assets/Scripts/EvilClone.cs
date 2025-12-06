using System;
using System.Collections;
using UnityEngine;

public class EvilClone : MonoBehaviour, IInteractable
{
    public static event Action OnEvilCloneDefeated;
    
    public Transform player;
    public float chaseSpeed = 0.5f;
    public int damage = 1;
    public bool inRange = false;

    private Animator animator;
    private HotbarController hotbarController;
    private bool moveToPlayer = false;
    private bool isAlive = true;  // if player is in hit range, then they can interact
    private float health = 15;

    void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        hotbarController = FindAnyObjectByType<HotbarController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Stop moving if game paused
        if (PauseController.IsGamePaused)
        {
            //Debug.Log($"Game paused (PlayerMovement sees IsGamePaused={PauseController.IsGamePaused})");
            Vector3 stopMovement = new Vector3(0, 0, 0);
            AnimateMovement(stopMovement);
            return;
        }

        if (moveToPlayer) 
        {
            MoveToPlayer();
        }

        float horizontal = gameObject.transform.position.x;
        float vertical = gameObject.transform.position.z;

        Vector3 direction = new Vector3(horizontal,0, vertical);

        AnimateMovement(direction);
    }

    void AnimateMovement(Vector3 direction)
    {
        if (animator != null)
        {
            if (direction.magnitude > 0)  // if moving
            {
                animator.SetBool("isMoving", true);

                animator.SetFloat("horizontal", direction.x);
                animator.SetFloat("vertical", direction.z);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
        }
    }

    void MoveToPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, (chaseSpeed * Time.deltaTime) / 2);
    }

    // When player enters detection range
    private void OnTriggerEnter(Collider collision)
    {
        moveToPlayer = true;
    }

    public bool CanInteract()
    {
        return isAlive && PlayerStats.Instance.GetCanCastSpells();  // if clone's not already defeated & player using magical item
    }

    public void Interact()
    {
        if (!CanInteract() && hotbarController.usingItem != null && !hotbarController.usingItem.Name.Contains("Wand"))
        {
            Debug.Log("Can't interact with evil clone.");
            return;
        }

        Debug.Log("CanInteract() is true: Evil Clone");

        int damage = 3;
        if (health - damage <= 0)
        {
            DefeatEvilClone();
            return;
        }

        StartCoroutine(Attacked(damage));
        Debug.Log($"New Health: {health}");
    }

    IEnumerator Attacked(int damage)
    {
        if (UsedMagicItem(damage))
        {
            yield return new WaitForSeconds(1f);
        }
    }

    // Decreases durability of the magic item used
    private bool UsedMagicItem(int damage)
    {
        if (hotbarController.usingItem != null && hotbarController.usingItem.Name.Contains("Wand"))
        {
            Debug.Log("Evil Clone Hit");

            Wand wand = hotbarController.usingItem.GetComponent<Wand>();

            wand.DecreaseDurability();

            health -= damage;

            PlayerStats.Instance.DecreaseCurrMagicEnergy();

            return true;
        }

        return false;
    }

    private void DefeatEvilClone()
    {
        OnEvilCloneDefeated.Invoke();

        Debug.Log("Evil Clone defeated");

        isAlive = false;
        moveToPlayer = false;
        Destroy(gameObject);
    }
}
