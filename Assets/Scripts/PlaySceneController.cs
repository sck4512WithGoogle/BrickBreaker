using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MJ.Ads;
using UnityEngine.SceneManagement;
using MJ.Data;

public class PlaySceneController : MonoBehaviour
{
    [SerializeField] private GameObject serverLoading;
    [SerializeField] private Collider2D gameOverTriggerCollider;
    private float currentTimeScale;
    private bool isClick = false;

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


    public void OnClickResurrect(GameObject _ResurrectPanel)
    {
        if (isClick)
        {
            return;
        }
        isClick = true;

        StartCoroutine(OnClickResurrectRoutine(_ResurrectPanel));
        IEnumerator OnClickResurrectRoutine(GameObject _ResurrectPanel)
        {
            bool isDone = false;
            serverLoading.SetActive(true);
            AdsManager.ShowRewardedAd(() => isDone = true, () => isDone = true);
            yield return new WaitUntil(() => isDone);
            isClick = false;
            serverLoading.SetActive(false);
            _ResurrectPanel.SetActive(false);

            //ºÎÈ° ½ÃÅ´
            Resurrect();
        }
    }

    public void Resurrect()
    {
        GameManager.Instance.OnResurrect();
        gameOverTriggerCollider.enabled = true;
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
