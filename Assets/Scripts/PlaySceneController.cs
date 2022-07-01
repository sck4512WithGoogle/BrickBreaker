using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MJ.Ads;
using UnityEngine.SceneManagement;
using MJ.Data;
using MJ.Manager;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class PopupAction
{
    public Action OpenPopup;

    public void SetIsDoneTrue()
    {
        isDone = true;
    }
    public bool IsDone => isDone;
    private bool isDone;
}

public class PlaySceneController : MonoBehaviour
{
    [SerializeField] private GameObject serverLoading;
    [SerializeField] private Collider2D gameOverTriggerCollider;
    [SerializeField] private GameObject resurrectPanel; //부활 사용할 건지 여부 UI패널
    [SerializeField] private GameObject pausePanel; //현재 퍼즈 켜져있는지 여부만 체크
    [SerializeField] private MessageBoxUI messageBoxUI;


    [Header("아이템 버튼")]
    [SerializeField] private Image[] itemButtonImages;
    [SerializeField] private RectTransform[] itemButtonRects;

    
    private float currentTimeScale;
    private bool isClick = false;
    private bool hasResurrected = false; //부활 사용했는지
    private Queue<PopupAction> popupActions;
    private bool hasInsertAction = false; //게임매니저에 델리게이트에 넣었는지 여부



    private void Awake()
    {
        popupActions = new Queue<PopupAction>();
    }

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
        GameManager.Instance.OnStartShootBall += () =>
        {
            for (int i = 0; i < itemButtonImages.Length; i++)
            {
                itemButtonImages[i].raycastTarget = false;
            }
        };

        GameManager.Instance.OnBlockCreateDone += () =>
        {
            for (int i = 0; i < itemButtonImages.Length; i++)
            {
                itemButtonImages[i].raycastTarget = true;
            }
        };

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
        //입력 못 받게
        InputController.escInput.Disable();
        //로딩 켜줌
        serverLoading.SetActive(true);
   
        Action onFailed = () =>
        {
            isClick = false;
            serverLoading.SetActive(false);

            //실패하면 메시지 띄워줌
            messageBoxUI.gameObject.SetActive(true);
            messageBoxUI.SetButtonTextOK();
            messageBoxUI.SetMessage("Failed to load ads.");
        };
        Action onSuccess = () =>
        {
            isClick = false;
            serverLoading.SetActive(false);
            resurrectPanel.SetActive(false);

            StartCoroutine(InvokeResurrectionEvent());
        };
        AdsManager.ShowRewardedAd(onFailed, onSuccess);

        IEnumerator InvokeResurrectionEvent()
        {
            //잠깐 기다렸다가 부활 시킴
            yield return YieldContainer.WaitForFixedUpdate;
            Resurrect();
            //입력 받을 수 있게 해줌
            InputController.escInput.Enable();
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
            //입력 못 받게 해줌
            InputController.escInput.Disable();
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



    public void AddPopupAction(PopupAction _PopupAction)
    {
        popupActions.Enqueue(_PopupAction);
        if(hasInsertAction)
        {
            return;
        }
        else
        {
            GameManager.Instance.OnBlockCreateDone += AddPopupAction;
            hasInsertAction = true;
        }
    }

    private void AddPopupAction()
    {
        StartCoroutine(AddPopupActionRoutine());
        IEnumerator AddPopupActionRoutine()
        {
            //시작할 때 못 누르게 함
            for (int i = 0; i < itemButtonImages.Length; i++)
            {
                itemButtonImages[i].raycastTarget = false;
            }
            InputController.escInput.Disable();


            while (popupActions.Count > 0)
            {
                var popupAction = popupActions.Dequeue();
                popupAction.OpenPopup();
                yield return new WaitUntil(() => popupAction.IsDone);
                //약간 텀을 줌
                yield return YieldContainer.GetWaitForSecondsRealtime(0.1f);
            }


            //누를 수 있게 해줌
            for (int i = 0; i < itemButtonImages.Length; i++)
            {
                itemButtonImages[i].raycastTarget = true;
            }
            //다 끝나고 빼줌
            GameManager.Instance.OnBlockCreateDone -= AddPopupAction;
            InputController.escInput.Enable();
            hasInsertAction = false;
        }
    }
}
