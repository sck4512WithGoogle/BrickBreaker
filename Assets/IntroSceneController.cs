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

        yield return YieldContainer.GetWaitForSeconds(2f);


        AdsManager.ShowBannerAd();
        //이후 씬이동
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.MainSceneName);
    }

}
