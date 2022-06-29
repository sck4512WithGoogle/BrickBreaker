using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using MJ.Manager;
using MJ.Data;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI curScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;

    private void Start()
    {
        if(Application.systemLanguage == SystemLanguage.Korean)
        {
            curScoreText.text = "현재점수 : ";
            bestScoreText.text = "최고기록 : ";
        }
        else
        {
            curScoreText.text = "Score : ";
            bestScoreText.text = "BestScore : ";
        }

        curScoreText.text += ScoreManager.CurScore.ToString();
        bestScoreText.text += ScoreManager.BestScore.ToString();
    }

    public void OnClickHome()
    {
        SceneManager.LoadScene(SceneNames.MainSceneName);
    }

    public void OnClickStart()
    {
        DataManager.Instance.IsContinuePlay = false;
        SceneManager.LoadScene(SceneNames.PlaySceneName);
    }
}
