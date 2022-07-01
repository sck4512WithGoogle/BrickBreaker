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
    [SerializeField] private GameObject resurrectPanel; //��Ȱ ����� ���� ���� UI�г�
    [SerializeField] private GameObject pausePanel; //���� ���� �����ִ��� ���θ� üũ
    [SerializeField] private MessageBoxUI messageBoxUI;


    [Header("������ ��ư")]
    [SerializeField] private Image[] itemButtonImages;
    [SerializeField] private RectTransform[] itemButtonRects;

    
    private float currentTimeScale;
    private bool isClick = false;
    private bool hasResurrected = false; //��Ȱ ����ߴ���
    private Queue<PopupAction> popupActions;
    private bool hasInsertAction = false; //���ӸŴ����� ��������Ʈ�� �־����� ����



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
        //�Է� �� �ް�
        InputController.escInput.Disable();
        //�ε� ����
        serverLoading.SetActive(true);
   
        Action onFailed = () =>
        {
            isClick = false;
            serverLoading.SetActive(false);

            //�����ϸ� �޽��� �����
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
            //��� ��ٷȴٰ� ��Ȱ ��Ŵ
            yield return YieldContainer.WaitForFixedUpdate;
            Resurrect();
            //�Է� ���� �� �ְ� ����
            InputController.escInput.Enable();
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
            //�Է� �� �ް� ����
            InputController.escInput.Disable();
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
        Time.timeScale = 1;
        bool isDone = false;
        serverLoading.SetActive(true);   
        AdsManager.ShowInterstitialAd(() => isDone = true, () => isDone = true);   
        yield return new WaitUntil(() => isDone);
        isClick = false;

        serverLoading.SetActive(false);
        //��� ���� ������ Ÿ�ӽ����� 1�� ����
        SceneManager.LoadScene(_SceneName);
    }


    private void OnApplicationPause(bool pause)
    {
        //���� �����̸鼭 �г� �����ִ� ��츸
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
            //������ �� �� ������ ��
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
                //�ణ ���� ��
                yield return YieldContainer.GetWaitForSecondsRealtime(0.1f);
            }


            //���� �� �ְ� ����
            for (int i = 0; i < itemButtonImages.Length; i++)
            {
                itemButtonImages[i].raycastTarget = true;
            }
            //�� ������ ����
            GameManager.Instance.OnBlockCreateDone -= AddPopupAction;
            InputController.escInput.Enable();
            hasInsertAction = false;
        }
    }
}
