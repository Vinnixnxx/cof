using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] KeyCode openInv = KeyCode.I;
    [SerializeField] GameObject inv;
    [SerializeField] GameObject hud;
    [SerializeField] Transform slots;
    [SerializeField]Transform slotItemTemp;
    [SerializeField] GameObject[] itemsObjects = new GameObject[1];
<<<<<<< Updated upstream
   
=======
    [SerializeField] Image crosshairDefault;
    [SerializeField] Image crosshairUse;
>>>>>>> Stashed changes

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
            if (t.GetChildCount() == 0) {
                return t;
            }
        }

        return null;
    }

<<<<<<< Updated upstream
=======
 

>>>>>>> Stashed changes
    private void Awake()
    {
        int i = 0;
        foreach (GameObject g in itemsObjects) {
            if (g.GetComponent<Item>().getID() != i) {
                Debug.LogError("Item id ERROR");
                
            }
            i++;
        }
    }

    public GameObject getItemObjectById(int id) {
        return itemsObjects[id];
    }

<<<<<<< Updated upstream
=======
    public void setUseCrosshair() {
        crosshairDefault.enabled = false;
        crosshairUse.enabled = true;
    }

    public void setDefaultCrosshair()
    {
        crosshairDefault.enabled = true;
        crosshairUse.enabled = false;
    }

>>>>>>> Stashed changes
}
