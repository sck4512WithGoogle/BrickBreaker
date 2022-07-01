using UnityEngine;
using TMPro;
using MJ.Data;
using UnityEngine.UI;
using MJ.Manager;
using UnityEngine.EventSystems;
using System;
using MJ.Ads;

public class TwoBoundButtonUI : MonoBehaviour, IPointerClickHandler
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
        DataManager.OnTwoBoundItemCountChange += OnTwoBoundItemCountChange;
    }

    private void OnDisable()
    {
        DataManager.OnTwoBoundItemCountChange -= OnTwoBoundItemCountChange;
    }

    private void Start()
    {
        OnTwoBoundItemCountChange(DataManager.TwoBoundItemCount);
    }

    private void OnTwoBoundItemCountChange(int _Count)
    {
        countText.text = _Count.ToString();

        if (_Count == 0)
        {
            OptionManager.IsTwoBoundItemOptionOn = false;



            var popupAction = new PopupAction();
            Action openPopup = () => OpenRewardAdsItemPopup(popupAction.SetIsDoneTrue);
            popupAction.OpenPopup = openPopup;
            playSceneController.AddPopupAction(popupAction);
        }

        UpdateTwoBoundOptionStatus();
    }

    private void OpenRewardAdsItemPopup(Action _OnDie)
    {
        InputController.escInput.Disable();
        rewardAdsBoxUI.gameObject.SetActive(true);
        rewardAdsBoxUI.OnDie += _OnDie;
        rewardAdsBoxUI.SetMessage($"two bound item +{Constants.TwoBoundItemCountMax}");
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
                OptionManager.IsTwoBoundItemOptionOn = true; //있으니 옵션 켜줌
                DataManager.TwoBoundItemCount += Constants.TwoBoundItemCountMax;
            };
            AdsManager.ShowRewardedAd(onFailed, onSuccess);
        });
    }

    private void UpdateTwoBoundOptionStatus()
    {
        image.sprite = OptionManager.IsTwoBoundItemOptionOn ? onSprite : offSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //아이템이 없는 경우
        if (DataManager.TwoBoundItemCount == 0)
        {
            OpenRewardAdsItemPopup(InputController.escInput.Enable);
            return;
        }



        OptionManager.IsTwoBoundItemOptionOn = !OptionManager.IsTwoBoundItemOptionOn;
        UpdateTwoBoundOptionStatus();
    }
}
