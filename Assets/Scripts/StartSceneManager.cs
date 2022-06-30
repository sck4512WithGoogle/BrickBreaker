using UnityEngine;
using Firebase.Auth;
using MJ.Data;
using MJ.Ads;
using MJ.Manager;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    private FirebaseAuth firebaseAuth;
    private DataManager dataManager;

    private void Awake()
    {
#if UNITY_EDITOR
        //PlayerPrefs.DeleteAll();
#endif
        Application.targetFrameRate = 60;
        dataManager = DataManager.Instance;


        firebaseAuth = FirebaseAuth.DefaultInstance;

        if(!dataManager.IsGuestLoginDone)
        {
            firebaseAuth.SignInAnonymouslyAsync();
            dataManager.IsGuestLoginDone = true;
        }

        //¿É¼Ç
        OptionManager.Init();

        //±¤°í
        AdsManager.Init();
        AdsManager.ShowBannerAd();
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
        continueButton.interactable = PlayMapDataManager.HasData;
    }

    private void OnESCPerform(InputAction.CallbackContext _CallbackContext)
    {
        Application.Quit();
    }


    public void OnClickStart()
    {
        dataManager.IsContinuePlay = false;
        SceneManager.LoadScene(SceneNames.PlaySceneName);
    }

    public void OnClickContinue()
    {
        dataManager.IsContinuePlay = true; 
        SceneManager.LoadScene(SceneNames.PlaySceneName);
    }

    public void OnClickReview()
    {

    }
}
