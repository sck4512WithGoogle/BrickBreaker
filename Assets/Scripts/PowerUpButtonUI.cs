using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MJ.Data;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MJ.Manager;
using System;
using MJ.Ads;

public class PowerUpButtonUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image image;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private MessageBoxUI rewardAdsBoxUI;
    [SerializeField] private MessageBoxUI messageBoxUI;
    [SerializeField] private GameObject serverLoading;
    [SerializeField] PlaySceneController playSceneController;

    private void OnEnable()
    {
        DataManager.OnPowerUpItemCountChange += OnPowerUpItemCountChange;
    }

    private void OnDisable()
    {
        DataManager.OnPowerUpItemCountChange -= OnPowerUpItemCountChange;
    }


    private void Start()
    {
        OnPowerUpItemCountChange(DataManager.PowerUpItemCount);
    }
    private void OnPowerUpItemCountChange(int _Count)
    {
        countText.text = _Count.ToString();


        if (_Count == 0)
        {
            OptionManager.IsPowerUpItemOptionOn = false;


            var popupAction = new PopupAction();
            Action openPopup = () => OpenRewardAdsItemPopup(popupAction.SetIsDoneTrue);
            popupAction.OpenPopup = openPopup;
            playSceneController.AddPopupAction(popupAction);
        }

        UpdatePowerUpOptionStatus();
    }

    private void OpenRewardAdsItemPopup(Action _OnDie)
    {
        InputController.escInput.Disable();
        rewardAdsBoxUI.gameObject.SetActive(true);
        rewardAdsBoxUI.OnDie += _OnDie;

        rewardAdsBoxUI.SetMessage($"power up item +{Constants.PowerUpItemCountMax}");
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
                OptionManager.IsPowerUpItemOptionOn = true; //있으니 옵션 켜줌
                DataManager.PowerUpItemCount += Constants.PowerUpItemCountMax;
            };
            AdsManager.ShowRewardedAd(onFailed, onSuccess);
        });
    }

    private void UpdatePowerUpOptionStatus()
    {
        image.sprite = OptionManager.IsPowerUpItemOptionOn ? onSprite : offSprite;
    }
 
    public void OnPointerClick(PointerEventData eventData)
    {
        //아이템이 없는 경우
        if (DataManager.PowerUpItemCount == 0)
        {
            OpenRewardAdsItemPopup(InputController.escInput.Enable);
            return;
        }



        OptionManager.IsPowerUpItemOptionOn = !OptionManager.IsPowerUpItemOptionOn;
        UpdatePowerUpOptionStatus();
    }
}
