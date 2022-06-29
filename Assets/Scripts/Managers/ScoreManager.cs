
namespace MJ.Manager
{
    using UnityEngine;
    using System;
    public static class ScoreManager
    {
        public static event Action<int> OnBestScoreChange;
        public static event Action<int> OnCurScoreChange;

        public static int BestScore => bestScore;
        private static int bestScore;
        public static int CurScore => curScore;
        private static int curScore;

        private static readonly string bestScoreSaveKey = "BestScore";

        public static void Init()
        {
            bestScore = PlayerPrefs.GetInt(bestScoreSaveKey, 0);
            curScore = 0;
        }


        public static void AddScore(int _Add)
        {
            curScore += _Add;
            OnCurScoreChange.Invoke(curScore);
            if (bestScore < curScore)
            {
                bestScore = curScore;
                PlayerPrefs.SetInt(bestScoreSaveKey, bestScore);
                OnBestScoreChange.Invoke(bestScore);
            }
        }
    }

}
