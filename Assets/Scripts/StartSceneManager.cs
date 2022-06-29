using UnityEngine;
using Firebase.Auth;
using MJ.Data;
using MJ.Ads;
using MJ.Manager;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    private void Start()
    {
        continueButton.interactable = PlayMapDataManager.HasData;
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
