using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using MJ.Manager;
using MJ.Data;
using UnityEngine.InputSystem;
using System.Collections;
using MJ.Ads;

public class GameOverSceneController : MonoBehaviour
{
    [SerializeField] private GameObject serverLoading;
    [SerializeField] private TextMeshProUGUI curScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    private bool isClick = false;
    
    private void Start()
    {
        curScoreText.text = "SCORE : ";
        bestScoreText.text = "BEST SCORE : ";


        curScoreText.text += ScoreManager.CurScore.ToString();
        bestScoreText.text += ScoreManager.BestScore.ToString();
    }
     

    public void OnClickHome()
    {
        if(isClick)
        {
            return;
        }
        isClick = true;

        StartCoroutine(ShowAdsAndLoadScene(SceneNames.MainSceneName));
    }

    public void OnClickStart()
    {
        if (isClick)
        {
            return;
        }
        isClick = true;

        DataManager.IsContinuePlay = false;
        StartCoroutine(ShowAdsAndLoadScene(SceneNames.PlaySceneName));
    }

    IEnumerator ShowAdsAndLoadScene(string _SceneName)
    {
        serverLoading.SetActive(true);
        //약간 기다림
        yield return YieldContainer.WaitForFixedUpdate;

        bool isDone = false;
        AdsManager.ShowInterstitialAd(() => isDone = true, () => isDone = true);
        yield return new WaitUntil(() => isDone);
        serverLoading.SetActive(false);
        SceneManager.LoadScene(_SceneName);
    }
}
