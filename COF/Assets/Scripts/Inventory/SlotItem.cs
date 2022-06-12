using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{

    public string id;

    [SerializeField] private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Item item;

    public void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        
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
        if (e.button == PointerEventData.InputButton.Right) 
        {
            print("Pápá item");
            Destroy(gameObject);
        }
    }

    public void setItem(Item item) {
        this.item = item;
    }
}
