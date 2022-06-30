using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MJ.Manager;
using MJ.Data;
using MJ.Ads;
using Firebase.Auth;
using System.Collections.Generic;

public class IntroSceneController : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
   
    private void Awake()
    {
#if UNITY_EDITOR
        PlayerPrefs.DeleteAll();
#endif
        Application.targetFrameRate = 60;

        //������
        DataManager.Init();

        //�ɼ�
        OptionManager.Init();

        //����
        AdsManager.Init();

#if !UNITY_EDITOR
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

        //���� ���̵�
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.MainSceneName);
    }

}
