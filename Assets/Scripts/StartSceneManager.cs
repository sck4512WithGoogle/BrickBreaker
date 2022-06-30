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
    [SerializeField] private Image fadeImage;

    private void Awake()
    {
#if UNITY_EDITOR
        //PlayerPrefs.DeleteAll();
#endif
                      

        AdsManager.ShowBannerAd();
    }


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

    private void Start()
    {
        continueButton.interactable = PlayMapDataManager.HasData;
        StartCoroutine(Fade(true));
    }

    private void OnESCPerform(InputAction.CallbackContext _CallbackContext)
    {
        Application.Quit();
    }


    public void OnClickStart()
    {
        DataManager.IsContinuePlay = false;
        StartCoroutine(Fade(false, () => SceneManager.LoadScene(SceneNames.PlaySceneName)));
    }

    public void OnClickContinue()
    {
        DataManager.IsContinuePlay = true;
        StartCoroutine(Fade(false, () => SceneManager.LoadScene(SceneNames.PlaySceneName)));
    }

    private IEnumerator Fade(bool _IsFadeIn, Action _OnEnd = null)
    {
        float speed = Constants.fadeSpeed;
        fadeImage.gameObject.SetActive(true);
        if(_IsFadeIn)
        {
            fadeImage.color = Color.black;

            while (fadeImage.color.a > 0f)
            {
                var color = fadeImage.color;
                color.a -= Time.deltaTime * speed;
                fadeImage.color = color;
                yield return null;
            }
            fadeImage.gameObject.SetActive(false);
        }
        else
        {
            fadeImage.color = new Color(0, 0, 0, 0);

            while (fadeImage.color.a < 1f)
            {
                var color = fadeImage.color;
                color.a += Time.deltaTime * speed;
                fadeImage.color = color;
                yield return null;
            }
        }
        _OnEnd?.Invoke();
    }

    public void OnClickReview()
    {
        switch (DataManager.CurrentStoreType)
        {
            case StoreType.GooglePlayStore:
                break;
            case StoreType.OneStore:
                break;
            case StoreType.GalaxyStore:
                break;
            case StoreType.AppStore:
                break;
        }
    }
}
