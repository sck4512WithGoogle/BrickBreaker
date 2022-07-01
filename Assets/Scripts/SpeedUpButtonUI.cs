using UnityEngine;
using TMPro;
using MJ.Data;
using MJ.Manager;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MJ.Ads;
using System;

public class SpeedUpButtonUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image image;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private MessageBoxUI rewardAdsBoxUI;
    [SerializeField] private MessageBoxUI messageBoxUI;
    [SerializeField] private GameObject serverLoading;
    [SerializeField] private PlaySceneController playSceneController;
    
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

        if (_Count == 0)
        {
            OptionManager.IsSpeedUpItemOptionOn = false;

            var popupAction = new PopupAction();
            Action openPopup = () => OpenRewardAdsItemPopup(popupAction.SetIsDoneTrue);
            popupAction.OpenPopup = openPopup;
            playSceneController.AddPopupAction(popupAction);
        }
        UpdateSpeedUpOptionStatus();
    }


    private void OpenRewardAdsItemPopup(Action _OnDie = null)
    {
        InputController.escInput.Disable();
        rewardAdsBoxUI.gameObject.SetActive(true);
        rewardAdsBoxUI.OnDie += _OnDie;

        rewardAdsBoxUI.SetMessage($"speed up item +{Constants.SpeedUpItemCountMax}");
        rewardAdsBoxUI.OkButton.onClick.AddListener(() =>
        {
            serverLoading.SetActive(true);
            Action onFailed = () =>
            {
                //실패시 꺼줌
                serverLoading.SetActive(false);
                messageBoxUI.gameObject.SetActive(true);
                messageBoxUI.SetMessage("failed to load ads", 73f);
                messageBoxUI.SetButtonTextOK();
            };
            Action onSuccess = () =>
            {
                serverLoading.SetActive(false);
                rewardAdsBoxUI.gameObject.SetActive(false);
                OptionManager.IsSpeedUpItemOptionOn = true;  //있으니 옵션 켜줌
                DataManager.SpeedUpItemCount += Constants.SpeedUpItemCountMax;
            };
            AdsManager.ShowRewardedAd(onFailed, onSuccess);
        });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //아이템이 없는 경우
        if (DataManager.SpeedUpItemCount == 0)
        {
            OpenRewardAdsItemPopup(InputController.escInput.Enable);
            return;
        }



        OptionManager.IsSpeedUpItemOptionOn = !OptionManager.IsSpeedUpItemOptionOn;
        UpdateSpeedUpOptionStatus();
    }
}
