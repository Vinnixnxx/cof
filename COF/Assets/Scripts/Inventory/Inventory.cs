using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] KeyCode openInv = KeyCode.I;
    [SerializeField] GameObject inv;
    [SerializeField] GameObject hud;
    [SerializeField] Transform slots;
    [SerializeField]Transform slotItemTemp;

   

    private void Update()
    {
        if (Input.GetKeyDown(openInv)) {
            inv.SetActive(!inv.activeSelf);
            hud.SetActive(!inv.activeSelf);
        }
        if (inv.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void addSlotItem(Item item) {
        if (getFirstEmptySlot()==null) return;
        Transform t = Instantiate(slotItemTemp, getFirstEmptySlot().position, getFirstEmptySlot().rotation, getFirstEmptySlot());
        t.GetComponent<SlotItem>().setItem(item);
    }

    Transform getFirstEmptySlot() {

        foreach (Transform t in slots) {
            if (t.childCount == 0) {
                return t;
            }
        }

        return null;
    }

}
