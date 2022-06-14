using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDrop : MonoBehaviour, IDropHandler
{
    private Transform playerHand;
    private Inventory invM;

    private void Start()
    {
        invM = GameObject.Find("InventoryManager").GetComponent<Inventory>();
        playerHand = GameObject.Find("Hand").transform;    
    }

    public void OnDrop(PointerEventData e)
    {
        if (e.pointerDrag.TryGetComponent<SlotItem>(out SlotItem slotItem)) {
            Instantiate(invM.getItemObjectById(slotItem.getItem().getID()).transform, 
                playerHand.position, 
                playerHand.rotation, 
                GameObject.Find("Items").transform
                );
            Debug.Log("Droped");
            Destroy(e.pointerDrag);
        }
    }

 
}
