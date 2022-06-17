using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : Interactable
{
    enum ItemType { food, ammo };

   

    [SerializeField] private int id;
    [SerializeField] private string itemName;
    [SerializeField][TextArea] private string itemDesc;
    [SerializeField] private Sprite icon;
    [SerializeField] private ItemType itemType;


        public override void OnFocus()
<<<<<<< Updated upstream
    {
        UpdateCross();
        if (crosshairDefault == null || crosshairUse == null) return;

        crosshairDefault.transform.gameObject.GetComponent<Image>().enabled = false;
            crosshairUse.transform.gameObject.GetComponent<Image>().enabled = true;
            
=======
        {

        GameObject.Find("InventoryManager").GetComponent<Inventory>().setUseCrosshair();

>>>>>>> Stashed changes
    }

    public override void OnInteract()
    {

        GameObject.Find("InventoryManager").GetComponent<Inventory>().addSlotItem(this);
        Destroy(gameObject);
        
    }

    public override void OnLoseFocus()
    {
<<<<<<< Updated upstream
        UpdateCross();
        if (crosshairDefault == null || crosshairUse == null) return;
        crosshairDefault.transform.gameObject.GetComponent<Image>().enabled = true;
        crosshairUse.transform.gameObject.GetComponent<Image>().enabled = false;
    }



    public void UpdateCross() {

        crosshairDefault = GameObject.Find("DefaultCrosshair").GetComponent<Image>();
        crosshairUse = GameObject.Find("UseCrosshair").GetComponent<Image>();
    }

    public int getID() {
        return id;
    }

    public Sprite getIcon()
    {
        return icon;
    }

    public string getItemName() {
        return itemName;
    }

    public string getItemDesc()
    {
        return itemDesc;
=======
        GameObject.Find("InventoryManager").GetComponent<Inventory>().setDefaultCrosshair();
    }



  

    public int getID() {
        return id;
    }

    public Sprite getIcon()
    {
        return icon;
>>>>>>> Stashed changes
    }

    public string getItemName() {
        return itemName;
    }

    public string getItemDesc()
    {
        return itemDesc;
    }

    private void OnDestroy()
    {
        GameObject.Find("InventoryManager").GetComponent<Inventory>().setDefaultCrosshair();
    }

}
