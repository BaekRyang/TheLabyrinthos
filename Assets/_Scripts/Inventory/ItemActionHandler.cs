using TypeDefs;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemActionHandler : MonoBehaviour, IPointerExitHandler
{
    private Item focusedItem;

    private GameObject useObject;
    private GameObject wearObject;
    private GameObject discardObject;
    private GameObject discardAllObject;
    private IPointerExitHandler _pointerExitHandlerImplementation;

    private void Start()
    {
        useObject  = transform.Find("Use").gameObject;
        wearObject = transform.Find("Wear").gameObject;
        discardObject = transform.Find("Discard").gameObject;
        discardAllObject = transform.Find("DiscardAll").gameObject;
        
        gameObject.SetActive(false);
    }

    public void ButtonAction(string action)
    {

        if (action == "Wear")
        {
            InventoryManager.Instance.Wear(ref focusedItem);
            
            OnPointerExit(null);
            return;
        }
        
        if (action == "Use")
        {
            if (focusedItem is Disposable disposable)
                disposable.dele_itemEffect?.Invoke();

            InventoryManager.Instance.RemoveItem(focusedItem);
            InventoryManager.Instance.openedInventory.UpdateInventory();
            OnPointerExit(null);
            return;
        }
        
        if (action == "Discard")
        {
            InventoryManager.Instance.RemoveItem(focusedItem);
            InventoryManager.Instance.openedInventory.UpdateInventory();
            OnPointerExit(null);
            return;
        }

        if (action == "DiscardAll")
        {
            InventoryManager.Instance.RemoveItem(focusedItem, 99);
            InventoryManager.Instance.openedInventory.UpdateInventory();
            OnPointerExit(null);
        }

    }

    public void LoadItem(ref Item item, Vector3 position)
    {
        focusedItem        = item;
        transform.position = position;
        gameObject.SetActive(true);

        useObject.SetActive(focusedItem.b_useable);

        if (focusedItem.IT_type == ItemType.Weapon)
        {
            wearObject.SetActive(true);
            discardAllObject.SetActive(false);
        }
        else
        {
            discardAllObject.SetActive(true);
            wearObject.SetActive(false);
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        focusedItem = null;
        gameObject.SetActive(false);
    }
}
