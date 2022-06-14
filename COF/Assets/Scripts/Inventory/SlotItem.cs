using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{

    public string id;

    [SerializeField] private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Item item;
    private Image icon;

    public void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        icon = gameObject.GetComponent<Image>();
    }



    public void OnBeginDrag(PointerEventData e)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData e)
    {
        rectTransform.position = e.position;
    }

    public void OnEndDrag(PointerEventData e)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        rectTransform.anchoredPosition = new Vector3(0, 0, 0);
    }

    public void OnPointerClick(PointerEventData e)
    {
        //hmmm, megy ez jobban is 
    }

    public void setItem(Item item) {
        this.item = item;
        icon = gameObject.GetComponent<Image>();
        icon.sprite = item.getIcon();
    }

    public Item getItem() {
        return item;
    }
}
