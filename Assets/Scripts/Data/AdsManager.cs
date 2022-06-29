
using System;

namespace MJ.Ads
{
    public static class AdsManager
    {
        private static bool isTestMode = true;
        public static bool IsTestMode => isTestMode;


        private static GoogleAdmobController googleAdmobController;
        private static UnityAdsController unityAdsController;

        private static bool isInit = false;

        public static void Init()
        {
            if(isInit)
            {
                return;
            }


#if UNITY_EDITOR
            isTestMode = true;
#endif

            googleAdmobController = new GoogleAdmobController();
            googleAdmobController.Init();

            unityAdsController = new UnityAdsController();
            unityAdsController.Init();

            isInit = true;
        }

        public static void ShowBannerAd()
        {
            googleAdmobController.ShowBanner(unityAdsController.ShowBannerAd);
        }

        public static void ShowInterstitialAd(Action _OnFailed, Action _OnClosed)
        {
#if UNITY_EDITOR
            unityAdsController.ShowInterstitialAd(_OnFailed, _OnClosed);
            return;
#endif
            googleAdmobController.ShowInterstitialAd(() => unityAdsController.ShowInterstitialAd(_OnFailed, _OnClosed), _OnClosed);
        }

        public static void ShowRewardedAd(Action _OnFailed, Action _OnClosed)
        {
#if UNITY_EDITOR
            unityAdsController.ShowRewardedAd(_OnFailed, _OnClosed);
            return;
#endif
            googleAdmobController.ShowRewardedAd(() => unityAdsController.ShowRewardedAd(_OnFailed, _OnClosed), _OnClosed);
        }
    }
}
