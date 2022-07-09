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
    [SerializeField] private RectTransform itemButtonsParentRect;
    //[SerializeField] private RectTransform[] itemButtonRects;

    [Header("GameOver")]
    [SerializeField] private Image fade;
    [SerializeField] private AudioSource gameOverSound;

    
    private float currentTimeScale;
    private bool isClick = false;
    private Queue<PopupAction> popupActions;
    private bool hasInsertAction = false; //���ӸŴ����� ��������Ʈ�� �־����� ����
    private bool hasClickedRewardedAds = false;
    private Action<bool> onPause;

    private void Awake()
    {
        popupActions = new Queue<PopupAction>();

        onPause += _Pause =>
        {
            //���� �����̸鼭 �г� �����ִ� ��츸, ���� �� ��쵵 �� ��
            if (_Pause && !pausePanel.activeSelf && !hasClickedRewardedAds)
            {
                SetTimeScale(0);
                pausePanel.SetActive(true);
            }
        };
    }

    private void OnEnable()
    {
        InputController.escInput.performed += OnESCPerform;
        InputController.escInput.Enable();
    }

    private void OnDisable()
    {
        InputController.escInput.performed -= OnESCPerform;
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


        Vector2 startPos = itemButtonsParentRect.anchoredPosition;
        float moveLength = 200f;
        itemButtonsParentRect.anchoredPosition -= Vector2.up * moveLength;

        float speed = 250f;
        while (moveLength > 0f)
        {
            float moveAmount = Time.deltaTime * speed;
            itemButtonsParentRect.anchoredPosition += Vector2.up * moveAmount;
                       
            moveLength -= moveAmount;
            yield return null;
        }

        itemButtonsParentRect.anchoredPosition = startPos;
        for (int i = 0; i < itemButtonImages.Length; i++)
        {
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

        DoGameOverEvent();
    }

    private void DoGameOverEvent()
    {
        StartCoroutine(DoGameOver());

        IEnumerator DoGameOver()
        {
            onPause = null;
            InputController.escInput.Disable();
            fade.gameObject.SetActive(true);

            if (OptionManager.IsSound)
            {
                gameOverSound.Play();
            }
            
            fade.color = new Color(0, 0, 0, 0);
            while (fade.color.a < 0.5f)
            {
                var color = fade.color;
                color.a += Time.deltaTime;
                fade.color = color;
                yield return null;
            }
            yield return YieldContainer.GetWaitForSeconds(1f);

            PlayMapDataManager.DeleteData();
            SceneManager.LoadScene(SceneNames.GameOverSceneName);
        }
    }

    public void OnClickResurrect()
    {
        if (isClick)
        {
            return;
        }
        hasClickedRewardedAds = true;
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

            hasClickedRewardedAds = false;
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
#if UNITY_EDITOR
            yield return YieldContainer.GetWaitForSeconds(0.1f);
#else
            yield return YieldContainer.WaitForFixedUpdate;
#endif
            Resurrect();
            //�Է� ���� �� �ְ� ����
            InputController.escInput.Enable();
            hasClickedRewardedAds = false;
        }
    }

    //���ӿ��� Ʈ���ſ� ����� ��
    public void OnGameOver()
    {
        if(GameManager.Instance.HasResurrected)
        {
            //��Ȱ �߾����� �ٷ� ���ӿ����� �Ѿ
            DoGameOverEvent();
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
        onPause?.Invoke(pause);
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
            hasClickedRewardedAds = true;
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
            hasClickedRewardedAds = false;
        }
    }
}
