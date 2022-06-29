using UnityEngine;
using TMPro;
using MJ.Data;
using UnityEngine.UI;
using MJ.Manager;
using UnityEngine.EventSystems;

public class TwoBoundButtonUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image image;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;



    private void OnEnable()
    {
        DataManager.Instance.OnTwoBoundItemCountChange += OnTwoBoundItemCountChange;
    }

    private void OnDisable()
    {
        DataManager.Instance.OnTwoBoundItemCountChange -= OnTwoBoundItemCountChange;
    }

    private void Start()
    {
        OnTwoBoundItemCountChange(DataManager.Instance.TwoBoundItemCount);
    }

    private void OnTwoBoundItemCountChange(int _Count)
    {
        countText.text = _Count.ToString();

        if (_Count == 0)
        {
            image.raycastTarget = false;
            OptionManager.IsTwoBoundItemOptionOn = false;
            UpdateTwoBoundOptionStatus();
        }
        else
        {
            image.raycastTarget = true;
            UpdateTwoBoundOptionStatus();
        }
    }

      
    private void UpdateTwoBoundOptionStatus()
    {
        image.sprite = OptionManager.IsTwoBoundItemOptionOn ? onSprite : offSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OptionManager.IsTwoBoundItemOptionOn = !OptionManager.IsTwoBoundItemOptionOn;
        UpdateTwoBoundOptionStatus();
    }
}
