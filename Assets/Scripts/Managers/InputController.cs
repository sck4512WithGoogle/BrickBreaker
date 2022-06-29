using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class InputController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public event Action<Vector3> OnBeginDragAction;
    public event Action<Vector3> OnDragAction;
    public event Action OnEndDragAction;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        SetInputActive(false);
    }

    public void SetInputActive(bool _IsActive)
    {
        image.raycastTarget = _IsActive;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        OnBeginDragAction.Invoke(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDragAction.Invoke(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnEndDragAction.Invoke();
    }
}
