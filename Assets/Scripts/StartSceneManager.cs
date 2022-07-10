using UnityEngine;
using Firebase.Auth;
using MJ.Data;
using MJ.Ads;
using MJ.Manager;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Graphic fadeImage;
    [SerializeField] private Graphic darkImage;
    [SerializeField] private GameObject logo;
    [SerializeField] private GameObject[] optionAndTutorialButtons;
    [SerializeField] private Button[] mainButtons;

    [Header("PopUps")]
    [SerializeField] private GameObject optionPopup;
    [SerializeField] private GameObject tutorialPopup;
    [SerializeField] private GameObject startMessagePopup;

    private void OnEnable()
    {
        InputController.escInput.performed += OnESCPerform;
        InputController.escInput.Enable();
    }

    private void OnDisable()
    {
        InputController.escInput.performed -= OnESCPerform;
        InputController.escInput.Disable();
    }

    private IEnumerator Start()
    {
        continueButton.interactable = PlayMapDataManager.HasData;
        yield return YieldContainer.WaitForFixedUpdate;
        StartCoroutine(Fade(true, fadeImage, null, 1.3f));
    }

   
    private void OnESCPerform(InputAction.CallbackContext _CallbackContext)
    {
        if(optionPopup.activeSelf)
        {
            optionPopup.SetActive(false);
            return;
        }

        if(tutorialPopup.activeSelf)
        {
            tutorialPopup.SetActive(false);
            return;
        }

#if !UNITY_EDITOR
        Application.Quit();
#endif
    }

  
    public void OnClickStartMessageOk()
    {
        //입력 못 받게 하려고
        for (int i = 0; i < mainButtons.Length; i++)
        {
            mainButtons[i].image.enabled = false;
        }
        startMessagePopup.SetActive(false);

        PlayMapDataManager.DeleteData();

        CoroutineExecuter.ExcuteAfterWaitTime(() =>
        {
            //이것도 꺼줌
            logo.SetActive(false);
            for (int i = 0; i < optionAndTutorialButtons.Length; i++)
            {
                optionAndTutorialButtons[i].SetActive(false);
            }


            DataManager.IsContinuePlay = false;
            StartCoroutine(Fade(true, darkImage, () => SceneManager.LoadScene(SceneNames.PlaySceneName), 2f));
        }, 0.1f);
    }

    public void OnClickStart()
    {
        //데이터 갖고있으면 이거 켜줌
        if(PlayMapDataManager.HasData)
        {
            startMessagePopup.SetActive(true);
            return;
        }

        //입력 못 받게 하려고
        for (int i = 0; i < mainButtons.Length; i++)
        {
            mainButtons[i].image.enabled = false;
        }


        //이것도 꺼줌
        logo.SetActive(false);
        for (int i = 0; i < optionAndTutorialButtons.Length; i++)
        {
            optionAndTutorialButtons[i].SetActive(false);
        }


        DataManager.IsContinuePlay = false;
        StartCoroutine(Fade(true, darkImage, () => SceneManager.LoadScene(SceneNames.PlaySceneName), 2f));
    }


    public void OnClickContinue()
    {
        //입력 못 받게 하려고
        for (int i = 0; i < mainButtons.Length; i++)
        {
            mainButtons[i].image.enabled = false;
        }

        //이것도 꺼줌
        logo.SetActive(false);
        for (int i = 0; i < optionAndTutorialButtons.Length; i++)
        {
            optionAndTutorialButtons[i].SetActive(false);
        }

        DataManager.IsContinuePlay = true;
        StartCoroutine(Fade(true, darkImage, () => SceneManager.LoadScene(SceneNames.PlaySceneName), 2f));
    }

    private IEnumerator Fade(bool _IsFadeIn, Graphic _Graphic, Action _OnEnd = null, float _Speed = 1.1f)
    {
        _Graphic.gameObject.SetActive(true);
        if(_IsFadeIn)
        {
            while (_Graphic.color.a > 0f)
            {
                var color = _Graphic.color;
                color.a -= Time.deltaTime * _Speed;
                _Graphic.color = color;
                yield return null;
            }
            _Graphic.gameObject.SetActive(false);
        }
        else
        {
            while (_Graphic.color.a < 1f)
            {
                var color = _Graphic.color;
                color.a += Time.deltaTime * _Speed;
                _Graphic.color = color;
                yield return null;
            }
        }
        _OnEnd?.Invoke();
    }

    public void OnClickReview()
    {
        Application.OpenURL(RemoteConfigData.StoreReviewURLs[DataManager.CurrentStoreType]);
    }
}
