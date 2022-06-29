

namespace MJ.Ads
{
    using UnityEngine;
    using UnityEngine.Advertisements;

    public class UnityAdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
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
                Advertisement.Initialize(gameID, AdsManager.IsTestMode, this);
                isUnityAdsOk = true;
            }
            catch (System.Exception)
            {
                isUnityAdsOk = false;
            }
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