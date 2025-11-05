using UnityEngine;
using UnityEngine.InputSystem;

// TODO:
// - fix chest icons not animating in Dark Forest
public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null;  // track closest interactable to player
    private InventoryController inventoryController;
    public GameObject[] interactionIcons;  // so can have multiple interactables in one scene
    public Animator animator;
    private string objectTag;  // was causing problem where couldn't interact because dialogue was already active

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryController = FindAnyObjectByType<InventoryController>();
        foreach (GameObject interactionIcon in interactionIcons)
        {
            interactionIcon.SetActive(false);
            if (interactionIcon != null)
            {
                animator = interactionIcon.GetComponent<Animator>();
            }
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        // TODO:
        // - add a different if statement if tag is NPC to try and bypass the !CanInteract()
        // Check to make sure there's an interactable

        if (interactableInRange == null) return;

        if (interactableInRange != null && !interactableInRange.CanInteract())  // THIS probably causing issue where can't press through dialogue
        {
            Debug.Log("CanInteract() is false.");
            return;
        }
        else
        {
            Debug.Log("CanInteract() is true.");
            if (context.performed)
            {
                try
                {
                    interactableInRange.Interact();
                }
                catch (MissingReferenceException)
                {
                    // If the target was destroyed between the check and the call, swallow safely
                    Debug.LogWarning("Interaction target destroyed before Interact() could be called.");
                }
                
                foreach (GameObject interactionIcon in interactionIcons)
                {
                    interactionIcon.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Mob"))
        {
            Debug.Log("In range of mob");
            if (collision.TryGetComponent(out IInteractable mobInteractable) && mobInteractable.CanInteract())  // check if has IInteractable script on object
            {
                interactableInRange = mobInteractable;
                collision.gameObject.GetComponent<Slime>().inRange = true;
            }
        }
        else if (collision.CompareTag("EvilClone"))
        {
            Debug.Log("In range of evil clone");
            if (collision.TryGetComponent(out IInteractable mobInteractable) && mobInteractable.CanInteract())  // check if has IInteractable script on object
            {
                interactableInRange = mobInteractable;
                collision.gameObject.GetComponent<EvilClone>().inRange = true;
            }
        }
        else if (collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();  // get item script

            // Check that did get script
            if (item != null)
            {
                // Add to inventory
                bool itemAdded = inventoryController.AddItem(collision.gameObject);

                if (itemAdded)
                {
                    // Show pickup UI
                    item.ShowPopup();

                    Destroy(collision.gameObject);  // looks like picking up object
                }
            }
        }
        else
        {
            if (collision.CompareTag("NPC"))
            {
                objectTag = "NPC";
                QuestController.Instance.CheckTalkNPCObjective(collision.GetComponent<NPC>());
            }
            if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())  // check if has IInteractable script on object
            {
                interactableInRange = interactable;
                foreach (GameObject interactionIcon in interactionIcons)
                {
                    if (interactionIcon == null) return;
                    if (interactionIcon.GetComponentInParent<IInteractable>() == interactable)
                    {
                        interactionIcon.SetActive(true);
                    }
                    AnimateIcon();

                }
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)  // if the nearest is the one already interacted with
        {
            if (collision.CompareTag("Mob"))
            {
                collision.gameObject.GetComponent<Slime>().inRange = false;
            }

            if (collision.CompareTag("EvilClone"))
            {
                collision.gameObject.GetComponent<EvilClone>().inRange = false;
            }
            interactableInRange = null;
            foreach (GameObject interactionIcon in interactionIcons)
            {
                if (interactionIcon == null) return;
                if (interactionIcon.GetComponentInParent<IInteractable>() == interactable)
                {
                    interactionIcon.SetActive(false);
                }
                StopAnimateIcon();
            }
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interactableInRange?.Interact();
        }
    }

    void AnimateIcon()
    {
        if (animator != null)
        {
            animator.SetBool("moving", true);
        }
    }

    void StopAnimateIcon()
    {
        if (animator != null)
        {
            animator.SetBool("moving", false);
        }
    }
}
