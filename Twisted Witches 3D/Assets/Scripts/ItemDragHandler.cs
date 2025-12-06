using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    Transform originalParent;  // place item originally came from
    CanvasGroup canvasGroup;

    public float minDropDistance = 1f;
    public float maxDropDistance = 2f;  // far enough away as not to accidentally pick back up
    public GameObject itemDictionaryObject;
    
    private InventoryController inventoryController;
    private Item3DDictionary itemDictionary;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        inventoryController = InventoryController.Instance;
        itemDictionary = FindAnyObjectByType<Item3DDictionary>();
        //itemDictionary = itemDictionaryObject.GetComponent<Item3DDictionary>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;  // save original parent
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;  // turns semi transparent during drag
    }

    public void OnDrag(PointerEventData eventData)  // eventData gives information on pointer (from mouse)
    {
        // Want item to follow mouse while moving
        transform.position = eventData.position;  // will make follow mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;  // allows to click on again
        canvasGroup.alpha = 1;  // no longer transparent

        // Move from one slot to another
        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>();  // slot where item dropped by mouse ('?' means this is nullable)

        // Fix bug of items not swapping
        // Previously was 1st raycasting onto item that was below but trying to grab slot straight from item but didn't have slot on item (that's the parent)
        if (dropSlot == null)
        {
            GameObject dropItem = eventData.pointerEnter;
            if (dropItem != null)
            {
                dropSlot = dropItem.GetComponentInParent<Slot>();
                // Now have dropSlot underneath item trying to grab
            }
        }

        Slot originalSlot = originalParent.GetComponent<Slot>();

        if (dropSlot == originalSlot)
        {
            transform.SetParent(originalParent);
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            return;
        }

        if (dropSlot != null)  // ended drag on stop of slot
        {
            if (dropSlot.currentItem != null)
            {
                // Check for stack possibility
                Item draggedItem = GetComponent<Item>();
                Item targetItem = dropSlot.currentItem.GetComponent<Item>();

                if (draggedItem.ID == targetItem.ID)
                {
                    targetItem.AddToStack(draggedItem.quantity);
                    originalSlot.currentItem = null;
                    Destroy(gameObject);
                }
                else
                {
                    // If drag item to a full slot --> reposition dragging item
                    dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                    originalSlot.currentItem = dropSlot.currentItem;
                    dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    // Move item into drop slot
                    transform.SetParent(dropSlot.transform);
                    dropSlot.currentItem = gameObject;
                    GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                }

            }
            else  // no current item in drop slot
            {
                originalSlot.currentItem = null;

                // Move item into drop slot
                transform.SetParent(dropSlot.transform);
                dropSlot.currentItem = gameObject;

                GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }

        }
        else  // if no slot under where dragged to
        {
            Debug.Log("No slot found");
            // Drop item outside of inventory
            if (!IsWithinInventory(eventData.position))
            {
                // Drop item
                DropItem(originalSlot);
            }
            else // No slot where dropping
            {
                //gameObject.transform.localScale = new Vector3(1, 1, 1);
                transform.SetParent(originalParent);  // send back to where it was
                GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
        }
    }

    bool IsWithinInventory(Vector2 mousePosition)
    {
        // Within InventoryPage
        // get shape of inventory panel
        RectTransform inventoryRect = originalParent.parent.GetComponent<RectTransform>();  // parent of any slot is the Menu page

        return RectTransformUtility.RectangleContainsScreenPoint(inventoryRect, mousePosition);  // checking if mouse inside this transform
    }

    void DropItem(Slot originalSlot)
    {
        // Drop items 1 by 1 from stack
        GameObject item3D = itemDictionary.GetItemPrefab(gameObject.GetComponent<Item>().ID);
        Item item = item3D.GetComponent<Item>();
        int quantity = item.quantity;

        if (quantity > 1)
        {
            item.RemoveFromStack();

            transform.SetParent(originalParent);
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            quantity = 1;
        }
        else
        {
            originalSlot.currentItem = null;  // empty slot
        }

        // Find player
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform == null)
        {
            Debug.LogError("Missing 'Player' tag.");
            return;
        }
        // Random drop position near player
        Vector3 dropOffset = Random.insideUnitCircle.normalized * Random.Range(minDropDistance, maxDropDistance);  // drops randomly in cirle around player
        Vector3 dropPosition = playerTransform.position + new Vector3(-Random.Range(minDropDistance, maxDropDistance), -0.5f, 0);  // player.transform is actually a Vector3

        // Instantiate drop item
        GameObject dropItem = Instantiate(gameObject, dropPosition, Quaternion.identity);  // Quaternion.identity means will have normal rotation
        Item droppedItem = dropItem.GetComponent<Item>();
        droppedItem.quantity = 1;  // only drop 1 item at a time
        dropItem.GetComponent<BounceEffect>().StartBounce();

        // Destory the UI one
        if (quantity <= 1 && originalSlot.currentItem == null)
        {
            Destroy(gameObject);
        }

        InventoryController.Instance.RebuildItemCounts();  // update item count

    }

    // If right click on stack it'll split and move to another slot
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Split stack
            SplitStack();
        }
    }

    private void SplitStack()
    {
        Item item = GetComponent<Item>();

        if (item == null || item.quantity <= 1) return;
        
        int splitAmount = item.quantity / 2;
        if (splitAmount <= 0) return;

        item.RemoveFromStack(splitAmount);
        GameObject newItem = item.CloneItem(splitAmount);

        // Find next empty inventory slot
        if (inventoryController == null || newItem == null) return;

        foreach (Transform slotTransform in inventoryController.inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.currentItem == null)  // actually have a slot and it's empty
            {
                slot.currentItem = newItem;
                newItem.transform.SetParent(slotTransform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                return;
            }
        }

        // No empty slot --> return to stack
        item.AddToStack(splitAmount);
        Destroy(newItem);
        
    }
}
