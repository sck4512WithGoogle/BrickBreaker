
using System;

namespace MJ.Ads
{
    public static class AdsManager
    {
        private static bool isTestMode = false;
        public static bool IsTestMode => isTestMode;


        private static GoogleAdmobController googleAdmobController;
        private static UnityAdsController unityAdsController;

        private static bool isInit = false;
        public static bool IsOpeningAdsShow { get; private set; } = false;

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

        public static void LoadOpeningAd()
        {
            googleAdmobController.LoadOpeningAds();
        }
        public static void ShowOpeningAd()
        {
            googleAdmobController.ShowAdIfAvailable(() => IsOpeningAdsShow = true);
        }



        public static void ShowBannerAd()
        {
#if UNITY_EDITOR
            unityAdsController.ShowBannerAd();
            return;
#endif
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
