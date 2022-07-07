using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;


public class BallShootInputListner : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public event Action<Vector3> OnBeginDragAction;
    public event Action<Vector3> OnDragAction;
    public event Action OnEndDragAction;
    private Image image;
    private int touchCount = 0;

    private void Awake()
    {
        image = GetComponent<Image>();
        SetInputActive(false);
    }

    void OnEnable()
    {
        touchCount = 0;
    }

    public void SetInputActive(bool _IsActive)
    {
        image.raycastTarget = _IsActive;
    }

  
    public void OnBeginDrag(PointerEventData eventData)
    {
        ++touchCount;
        if (touchCount == 1)
        {
            OnBeginDragAction.Invoke(eventData.position);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (touchCount == 1)
        {
            OnDragAction.Invoke(eventData.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        --touchCount;
        if (touchCount == 0)
        {
            OnEndDragAction.Invoke();
        }
        
        if(touchCount < 0)
        {
            touchCount = 0;
        }
    }
}
