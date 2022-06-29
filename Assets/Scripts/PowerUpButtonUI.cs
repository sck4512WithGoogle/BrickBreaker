using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MJ.Data;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MJ.Manager;

public class PowerUpButtonUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image image;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;


    private void OnEnable()
    {
        DataManager.Instance.OnPowerUpItemCountChange += OnPowerUpItemCountChange;
    }

    private void OnDisable()
    {
        DataManager.Instance.OnPowerUpItemCountChange -= OnPowerUpItemCountChange;
    }


    private void Start()
    {
        OnPowerUpItemCountChange(DataManager.Instance.PowerUpItemCount);
    }
    private void OnPowerUpItemCountChange(int _Count)
    {
        countText.text = _Count.ToString();


        if (_Count == 0)
        {
            image.raycastTarget = false;
            OptionManager.IsPowerUpItemOptionOn = false;
            UpdatePowerUpOptionStatus();
        }
        else
        {
            image.raycastTarget = true;
            UpdatePowerUpOptionStatus();
        }
    }

    private void UpdatePowerUpOptionStatus()
    {
        image.sprite = OptionManager.IsPowerUpItemOptionOn ? onSprite : offSprite;
    }
 
    public void OnPointerClick(PointerEventData eventData)
    {
        OptionManager.IsPowerUpItemOptionOn = !OptionManager.IsPowerUpItemOptionOn;
        UpdatePowerUpOptionStatus();
    }
}
