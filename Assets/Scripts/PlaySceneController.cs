using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MJ.Ads;
using UnityEngine.SceneManagement;
using MJ.Data;
using MJ.Manager;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlaySceneController : MonoBehaviour
{
    [SerializeField] private GameObject serverLoading;
    [SerializeField] private Collider2D gameOverTriggerCollider;
    [SerializeField] private GameObject resurrectPanel; //부활 사용할 건지 여부 UI패널
    [SerializeField] private GameObject pausePanel; //현재 퍼즈 켜져있는지 여부만 체크

    [Header("아이템 버튼")]
    [SerializeField] private Image[] itemButtonImages;
    [SerializeField] private RectTransform[] itemButtonRects;


    private float currentTimeScale;
    private bool isClick = false;
    private bool hasResurrected = false; //부활 사용했는지


    private void OnEnable()
    {
        InputController.escInput.performed += OnESCPerform;
        InputController.escInput.Enable();
    }

    private void OnDisable()
    {
        InputController.escInput.performed += OnESCPerform;
        InputController.escInput.Disable();
    }

    private void Start()
    {
        StartCoroutine(ButtonUpRoutine());
    }

    IEnumerator ButtonUpRoutine()
    {
        for (int i = 0; i < itemButtonImages.Length; i++)
        {
            itemButtonImages[i].raycastTarget = false;
        }


        Vector2[] startPoses = new Vector2[itemButtonRects.Length];
        float moveLength = 200f;
        for (int i = 0; i < itemButtonRects.Length; i++)
        {
            startPoses[i] = itemButtonRects[i].anchoredPosition;
            itemButtonRects[i].anchoredPosition -= Vector2.up * moveLength;
        }

        float speed = 200f;
        while (moveLength > 0f)
        {
            float moveAmount = Time.deltaTime * speed;
            for (int i = 0; i < itemButtonRects.Length; i++)
            {
                itemButtonRects[i].anchoredPosition += Vector2.up * moveAmount;
            }
            moveLength -= moveAmount;
            yield return null;
        }

        for (int i = 0; i < itemButtonImages.Length; i++)
        {
            itemButtonRects[i].anchoredPosition = startPoses[i];
            itemButtonImages[i].raycastTarget = true;
        }
    }


    private void OnESCPerform(InputAction.CallbackContext _CallBackContext)
    {
        if(pausePanel.activeSelf)
        {
            pausePanel.SetActive(false);
            ResetTimeScale();
        }
        else
        {
            pausePanel.SetActive(true);
            SetTimeScale(0);
        }
    }

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
        Time.timeScale = 1;
        bool isDone = false;
        serverLoading.SetActive(true);   
        AdsManager.ShowInterstitialAd(() => isDone = true, () => isDone = true);   
        yield return new WaitUntil(() => isDone);
        isClick = false;

        serverLoading.SetActive(false);
        //어딜 가도 무조건 타임스케일 1로 해줌
        SceneManager.LoadScene(_SceneName);
    }


    private void OnApplicationPause(bool pause)
    {
        //퍼즈 상태이면서 패널 꺼져있는 경우만
        if(pause && !pausePanel.activeSelf)
        {
            SetTimeScale(0);
            pausePanel.SetActive(true);
        }
    }
}
