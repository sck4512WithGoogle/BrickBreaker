using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MJ.Ads;
using UnityEngine.SceneManagement;
using MJ.Data;
using MJ.Manager;

public class PlaySceneController : MonoBehaviour
{
    [SerializeField] private GameObject serverLoading;
    [SerializeField] private Collider2D gameOverTriggerCollider;
    [SerializeField] private GameObject resurrectPanel; //부활 사용할 건지 여부 UI패널
    private float currentTimeScale;
    private bool isClick = false;
    private bool hasResurrected = false; //부활 사용했는지

    public void SetTimeScale(float _TimeScale)
    {
        currentTimeScale = Time.timeScale;
        Time.timeScale = _TimeScale;
    }

    public void ResetTimeScale()
    {
        Time.timeScale = currentTimeScale;
    }

       
    public void LoadMainScene()
    {
        if (isClick)
        {
            return;
        }
        isClick = true;

        StartCoroutine(ShowAdsAndLoadScene(SceneNames.MainSceneName));
    }

    public void LoadGameOverScene()
    {
        if (isClick)
        {
            return;
        }
        isClick = true;

        StartCoroutine(ShowAdsAndLoadScene(SceneNames.GameOverSceneName));
    }


    public void OnClickResurrect()
    {
        if (isClick)
        {
            return;
        }
        isClick = true;

        StartCoroutine(OnClickResurrectRoutine());
        IEnumerator OnClickResurrectRoutine()
        {
            bool isDone = false;
            serverLoading.SetActive(true);
            AdsManager.ShowRewardedAd(() => isDone = true, () => isDone = true);
            yield return new WaitUntil(() => isDone);
            isClick = false;
            serverLoading.SetActive(false);
            resurrectPanel.SetActive(false);

            //부활 시킴
            Resurrect();
        }
    }

    //게임오버 트리거에 닿았을 때
    public void OnGameOver()
    {
        if(hasResurrected)
        {
            //부활 했었으면 바로 게임오버로 넘어감
            PlayMapDataManager.DeleteData();
            SceneManager.LoadScene(SceneNames.GameOverSceneName);
        }
        else
        {
            //부활 안 했으면 이거 띄워줌
            resurrectPanel.SetActive(true);
        }
    }

    private void Resurrect()
    {
        GameManager.Instance.OnResurrect();
        gameOverTriggerCollider.enabled = true;
        hasResurrected = true;
    }


    private IEnumerator ShowAdsAndLoadScene(string _SceneName)
    {
        bool isDone = false;
        serverLoading.SetActive(true);
        AdsManager.ShowInterstitialAd(() => isDone = true, () => isDone = true);
        yield return new WaitUntil(() => isDone);
        isClick = false;
        serverLoading.SetActive(false);
        SceneManager.LoadScene(_SceneName);
    }
}
