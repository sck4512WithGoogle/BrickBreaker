using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MJ.Manager;

public class BestScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private void OnEnable()
    {
        ScoreManager.OnBestScoreChange += OnBestScoreChange;
    }

    private void OnDisable()
    {
        ScoreManager.OnBestScoreChange -= OnBestScoreChange;
    }

    private void Start()
    {
        OnBestScoreChange(ScoreManager.BestScore);
    }

    private void OnBestScoreChange(int _Score)
    {
        scoreText.text = _Score.ToString();
    }
}
