using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MJ.Manager;
using MJ.Data;
using MJ.Ads;

#if !UNITY_EDITOR
using Firebase.Auth;
#endif

public class IntroSceneController : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
   
    private void Awake()
    {
#if UNITY_EDITOR
        PlayerPrefs.DeleteAll();
#endif

        //데이터
        DataManager.Init();
        //옵션
        OptionManager.Init();
        //광고
        AdsManager.Init();
        //리모트 컨피그 데이터 불러옴
        RemoteConfigData.LoadData();


        AdsManager.LoadOpeningAd();
        GoogleMobileAds.Api.AppStateEventNotifier.AppStateChanged += OnAppStateChanged;


#if !UNITY_EDITOR
        Application.targetFrameRate = 60;

        var firebaseAuth = FirebaseAuth.DefaultInstance;
        firebaseAuth.SignInAnonymouslyAsync();
#endif
    }
     
    private void Start()
    {
        StartCoroutine(IntroRoutine());
    }

    private IEnumerator IntroRoutine()
    {
        fadeImage.color = Color.black;
        float speed = Constants.fadeSpeed;
        fadeImage.gameObject.SetActive(true);



        while (fadeImage.color.a > 0f)
        {
            var color = fadeImage.color;
            color.a -= Time.deltaTime * speed;
            fadeImage.color = color;
            yield return null;
        }

        float waitTime = 0f;
      
        while (!AdsManager.IsOpeningAdsShow)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }

        if (waitTime < 2f)
        {
            yield return YieldContainer.GetWaitForSeconds(2f - waitTime);
        }
        else
        {
            //아니면 이만큼 기다림
            yield return YieldContainer.GetWaitForSeconds(0.5f);
        }

        GoogleMobileAds.Api.AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
        AdsManager.ShowBannerAd();
        //이후 씬이동
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.MainSceneName);
    }

    private void OnAppStateChanged(GoogleMobileAds.Common.AppState _AppStatus)
    {
        if(_AppStatus == GoogleMobileAds.Common.AppState.Foreground)
        {
            AdsManager.ShowOpeningAd();
        }
    }
}
