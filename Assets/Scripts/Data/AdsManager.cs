
namespace MJ.Ads
{
    public static class AdsManager
    {
        private static bool isTestMode = true;
        public static bool IsTestMode => isTestMode;


        private static GoogleAdmobController googleAdmobController;

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


            isInit = true;
        }

        public static void ShowBannerAd()
        {
            googleAdmobController.ShowBanner(null);
        }

        public static void ShowInterstitialAd()
        {
            googleAdmobController.ShowInterstitialAd(null, null);
        }

        public static void ShowRewardedAd()
        {
            googleAdmobController.ShowRewardedAd(null, null);
        }
    }
}
