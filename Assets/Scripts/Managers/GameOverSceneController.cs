using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using MJ.Manager;
using MJ.Data;
using UnityEngine.InputSystem;

public class GameOverSceneController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI curScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;

    private void Start()
    {
        //if(Application.systemLanguage == SystemLanguage.Korean)
        //{
        //    curScoreText.text = "�������� : ";
        //    bestScoreText.text = "�ְ��� : ";
        //}
        //else
        //{
            curScoreText.text = "SCORE : ";
            bestScoreText.text = "BEST SCORE : ";
        //}

        curScoreText.text += ScoreManager.CurScore.ToString();
        bestScoreText.text += ScoreManager.BestScore.ToString();
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

    private void OnESCPerform(InputAction.CallbackContext _CallbackContext)
    {
        SceneManager.LoadScene(SceneNames.MainSceneName);
    }

    public void OnClickHome()
    {
        SceneManager.LoadScene(SceneNames.MainSceneName);
    }

    public void OnClickStart()
    {
        DataManager.IsContinuePlay = false;
        SceneManager.LoadScene(SceneNames.PlaySceneName);
    }
}
