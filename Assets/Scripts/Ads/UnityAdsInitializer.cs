

namespace MJ.Ads
{
    using UnityEngine;
    using UnityEngine.Advertisements;

    public class UnityAdsInitializer : IUnityAdsInitializationListener
    {
        private string gameID;
        public static bool IsUnityAdsOk => isUnityAdsOk;
        private static bool isUnityAdsOk = false;

        public void InititalizeAds()
        {
#if UNITY_EDITOR
            gameID = "4819405"; //그냥 안드로이드로 해줌
#elif UNITY_ANDROID
        gameID = "4819405";
#elif UNITY_IPHONE
        gameID = "4819404";
#endif
            try
            {
                Debug.Log("유니티 초기화1");
                Advertisement.Initialize(gameID, AdsManager.IsTestMode, this);
                Debug.Log("유니티 초기화2");
                isUnityAdsOk = true;
                Debug.Log("유니티 초기화3");
            }
            catch (System.Exception)
            {
                isUnityAdsOk = false;
            }
            isUnityAdsOk = false;
        }


        public void OnInitializationComplete()
        {
            //Debug.Log("초기화 끝");
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            //Debug.Log("초기화 실패");
        }
    }
}