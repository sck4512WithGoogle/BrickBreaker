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
    [SerializeField] private GameObject resurrectPanel; //��Ȱ ����� ���� ���� UI�г�
    private float currentTimeScale;
    private bool isClick = false;
    private bool hasResurrected = false; //��Ȱ ����ߴ���

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

            //��Ȱ ��Ŵ
            Resurrect();
        }
    }

    //���ӿ��� Ʈ���ſ� ����� ��
    public void OnGameOver()
    {
        if(hasResurrected)
        {
            //��Ȱ �߾����� �ٷ� ���ӿ����� �Ѿ
            PlayMapDataManager.DeleteData();
            SceneManager.LoadScene(SceneNames.GameOverSceneName);
        }
        else
        {
            //��Ȱ �� ������ �̰� �����
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
