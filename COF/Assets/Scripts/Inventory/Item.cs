using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : Interactable
{
    enum ItemType { food, ammo };

    Image crosshairDefault;
    Image crosshairUse;

    [SerializeField] private int id;
    [SerializeField] private string name;
    [SerializeField] private Sprite icon;
    [SerializeField] private ItemType itemType;
    Transform myModel;

        public override void OnFocus()
    {
        
      
            crosshairDefault.transform.gameObject.GetComponent<Image>().enabled = false;
            crosshairUse.transform.gameObject.GetComponent<Image>().enabled = true;
            
    }

    public override void OnInteract()
    {

        GameObject.Find("InventoryManager").GetComponent<Inventory>().addSlotItem(this);
        Destroy(gameObject);
        
    }

    public override void OnLoseFocus()
    {
        crosshairDefault.transform.gameObject.GetComponent<Image>().enabled = true;
        crosshairUse.transform.gameObject.GetComponent<Image>().enabled = false;
    }

    private void Start()
    {
        crosshairDefault = GameObject.Find("DefaultCrosshair").GetComponent<Image>();
        crosshairUse = GameObject.Find("UseCrosshair").GetComponent<Image>();
        myModel = transform;
    }
}
