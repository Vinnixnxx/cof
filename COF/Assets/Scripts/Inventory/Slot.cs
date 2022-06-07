using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData e)
    {
        if (transform.childCount <= 0)
        {
            if (e.pointerDrag != null)
            {
                Debug.Log("Drop");
                e.pointerDrag.GetComponent<RectTransform>().SetParent(transform);
                e.pointerDrag.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
            }
        }
        
            e.pointerDrag.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        
        
        
    }
}
