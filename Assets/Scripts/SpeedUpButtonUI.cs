using UnityEngine;
using TMPro;
using MJ.Data;
using MJ.Manager;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpeedUpButtonUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image image;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;
    
    private void OnEnable()
    {
        DataManager.OnSpeedUpItemCountChange += OnSpeedUpItemCountChange;
    }

    private void OnDisable()
    {
        DataManager.OnSpeedUpItemCountChange -= OnSpeedUpItemCountChange;
    }

    private void Start()
    {
        OnSpeedUpItemCountChange(DataManager.SpeedUpItemCount);  
    }

    private void UpdateSpeedUpOptionStatus()
    {
        image.sprite = OptionManager.IsSpeedUpItemOptionOn ? onSprite : offSprite;
    }

    private void OnSpeedUpItemCountChange(int _Count)
    {
        countText.text = _Count.ToString();

        if(_Count == 0)
        {
            image.raycastTarget = false;
            OptionManager.IsSpeedUpItemOptionOn = false;
            UpdateSpeedUpOptionStatus();
        }
        else
        {
            image.raycastTarget = true;
            UpdateSpeedUpOptionStatus();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OptionManager.IsSpeedUpItemOptionOn = !OptionManager.IsSpeedUpItemOptionOn;
        UpdateSpeedUpOptionStatus();
    }
}
