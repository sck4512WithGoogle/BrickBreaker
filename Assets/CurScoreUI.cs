using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MJ.Manager;

public class CurScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI curScoreContentsText;
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake()
    {
        if(Application.systemLanguage != SystemLanguage.Korean)
        {
            curScoreContentsText.text = "Score : ";
        }
    }

    private void OnEnable()
    {
        ScoreManager.OnCurScoreChange += OnCurScoreChange;
    }

    private void OnDisable()
    {
        ScoreManager.OnCurScoreChange -= OnCurScoreChange;
    }


    private void Start()
    {
        OnCurScoreChange(ScoreManager.CurScore);
    }

    private void OnCurScoreChange(int _Score)
    {
        scoreText.text = _Score.ToString();
    }
}
