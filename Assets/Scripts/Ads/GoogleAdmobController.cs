using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MJ.Ads
{
    public class GoogleAdmobController
    {
        private AppOpenAd appOpeningAd;
        private BannerView bannerView;
        private InterstitialAd interstitialAd;
        private RewardedAd rewardedAd;

        private Action onInterstitialFailed;
        private Action onInterstitialClosed;

        private Action onRewardedFailed;
        private Action onRewardedClosed;


        private DateTime openingAdsLoadedTime;
        private bool isOpeningAdsShowing = false;
        private bool isOpeningAdAvailable => appOpeningAd != null && (DateTime.UtcNow - openingAdsLoadedTime).TotalHours < 4;


        private bool isBannerShow = false; //배너 광고 현재 나왔는지 여부 체크
        private bool isBannerLoaded = false;


        private bool isInterstitialAdsLoaded = false;
        private bool isRewardAdsLoaded = false;

        private string[] rewardAdsIds;
        private int curRewardAdsIndex;


        public void Init()
        {
            rewardAdsIds = new string[4];
#if UNITY_ANDROID
            rewardAdsIds[0] = "ca-app-pub-1270408828484515/6568824267";
            rewardAdsIds[1] = "ca-app-pub-1270408828484515/7318815067";
            rewardAdsIds[2] = "ca-app-pub-1270408828484515/8073477625";
            rewardAdsIds[3] = "ca-app-pub-1270408828484515/5064170907";
#elif UNITY_IOS
            rewardAdsIds[0] = "ca-app-pub-1270408828484515/9709992925";
            rewardAdsIds[1] = "ca-app-pub-1270408828484515/2789361351";
            rewardAdsIds[2] = "ca-app-pub-1270408828484515/7466972963";
            rewardAdsIds[3] = "ca-app-pub-1270408828484515/6864337731";
#endif
            curRewardAdsIndex = UnityEngine.Random.Range(0, rewardAdsIds.Length);


            MobileAds.SetiOSAppPauseOnBackground(true);
            RequestConfiguration requestConfiguration = null;
            if (AdsManager.IsTestMode)
            {
                List<String> deviceIds = new List<String>() { AdRequest.TestDeviceSimulator };

                // Add some test device IDs (replace with your own device IDs).
#if UNITY_IOS
        deviceIds.Add("96e23e80653bb28980d3f40beb58915c");
#elif UNITY_ANDROID
                deviceIds.Add("75EF8D155528C04DACBBA6F36F433035");
                deviceIds.Add("2E3B5F652C849BA944FF091E84B6DD2B");
#endif

                // Configure TagForChildDirectedTreatment and test device IDs.
                requestConfiguration =
                new RequestConfiguration.Builder()
                .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
                .SetTestDeviceIds(deviceIds).build();
            }
            else
            {
                // Configure TagForChildDirectedTreatment and test device IDs.
                requestConfiguration =
                    new RequestConfiguration.Builder()
                    .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
                    .build();
            }

            MobileAds.SetRequestConfiguration(requestConfiguration);

            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(HandleInitCompleteAction);


            RequestInterstitialAD();
            RequestRewardedAD();
        }

        private void HandleInitCompleteAction(InitializationStatus initstatus)
        {
            // Callbacks from GoogleMobileAds are not guaranteed to be called on
            // main thread.
            // In this example we use MobileAdsEventExecutor to schedule these calls on
            // the next Update() loop.
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                //RequestBannerAd();
            });
        }


        #region 오프닝 광고
        public void LoadOpeningAds()
        {
            string adUnitId = string.Empty;
            if (AdsManager.IsTestMode)
            {
#if UNITY_ANDROID
                adUnitId = "ca-app-pub-3940256099942544/3419835294";
#elif UNITY_IOS
    adUnitId = "ca-app-pub-3940256099942544/5662855259";
#else
    adUnitId = "unexpected_platform";
#endif
            }
            else
            {
#if UNITY_ANDROID
                adUnitId = "ca-app-pub-1270408828484515/7600281430";
#elif UNITY_IPHONE
        adUnitId = "ca-app-pub-1270408828484515/1669583319";
#else
        adUnitId = "unexpected_platform";
#endif
            }

            AdRequest adRequest = new AdRequest.Builder().Build();

            // Load an app open ad for portrait orientation
            AppOpenAd.LoadAd(adUnitId, ScreenOrientation.Portrait, adRequest, ((appOpenAd, error) =>
            {
                if (error != null)
                {
                    // Handle the error.
                    Debug.LogFormat("Failed to load the ad. (reason: {0})", error.LoadAdError.GetMessage());
                    return;
                }

                // App open ad is loaded.
                appOpeningAd = appOpenAd;
                openingAdsLoadedTime = DateTime.UtcNow;


                appOpeningAd.OnAdDidDismissFullScreenContent += HandleAdDidDismissFullScreenContent;
                appOpeningAd.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresentFullScreenContent;
                appOpeningAd.OnAdDidPresentFullScreenContent += HandleAdDidPresentFullScreenContent;
                appOpeningAd.OnAdDidRecordImpression += HandleAdDidRecordImpression;
                appOpeningAd.OnPaidEvent += HandlePaidEvent;
            }));
        }
        public void ShowAdIfAvailable(Action _OnEnd = null)
        {
            CoroutineExecuter.Excute(ShowOpeningAd());
            IEnumerator ShowOpeningAd()
            {
                yield return new WaitWhile(() => !isOpeningAdAvailable || isOpeningAdsShowing);
                appOpeningAd.Show();
                _OnEnd?.Invoke();
            }
        }

        private void HandleAdDidDismissFullScreenContent(object sender, EventArgs args)
        {
            Debug.Log("Closed app open ad");
            // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
            appOpeningAd = null;
            isOpeningAdsShowing = false;
            LoadOpeningAds();
        }

        private void HandleAdFailedToPresentFullScreenContent(object sender, AdErrorEventArgs args)
        {
            Debug.LogFormat("Failed to present the ad (reason: {0})", args.AdError.GetMessage());
            // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
            appOpeningAd = null;
            LoadOpeningAds();
        }

        private void HandleAdDidPresentFullScreenContent(object sender, EventArgs args)
        {
            Debug.Log("Displayed app open ad");
            isOpeningAdsShowing = true;
        }

        private void HandleAdDidRecordImpression(object sender, EventArgs args)
        {
            Debug.Log("Recorded ad impression");
        }

        private void HandlePaidEvent(object sender, AdValueEventArgs args)
        {
            Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
                    args.AdValue.CurrencyCode, args.AdValue.Value);
        }
        #endregion















        private void RequestBanner()
        {
            string adUnitId = string.Empty;
            if(AdsManager.IsTestMode)
            {
#if UNITY_ANDROID
                adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IOS
        adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        adUnitId = "unexpected_platform";
#endif

            }
            else
            {
#if UNITY_ANDROID
                adUnitId = "ca-app-pub-1270408828484515/2036614515";
#elif UNITY_IPHONE
        adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        adUnitId = "unexpected_platform";
#endif
            }


            // Create a 320x50 banner at the top of the screen.
            bannerView = new BannerView(adUnitId, AdSize.SmartBanner,AdPosition.Bottom);
            bannerView.OnAdLoaded += (a, b) => isBannerLoaded = true;
            
            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            // Load the banner with the request.
            bannerView.LoadAd(request);
        }

        public void ShowBanner(Action _OnLoadFailed)
        {
            isBannerLoaded = false;
            RequestBanner();
            CoroutineExecuter.Excute(ShowBannerRoutine(_OnLoadFailed));

            IEnumerator ShowBannerRoutine(Action _OnFailed)
            {
                float timer = 3f;
                while (timer > 0f)
                {
                    timer -= Time.deltaTime;
                    if(isBannerLoaded)
                    {
                        bannerView.Show();
                        isBannerShow = true;
                        yield break;
                    }

                    yield return null;
                }
                _OnFailed?.Invoke();
            }
        }


        private void RequestInterstitialAD()
        {
            isInterstitialAdsLoaded = false;

            string adUnitId = string.Empty;
            if (AdsManager.IsTestMode)
            {
#if UNITY_EDITOR
                adUnitId = "unused";
#elif UNITY_ANDROID
        adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE || UNITY_iOS
        adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        adUnitId = "unexpected_platform";
#endif
            }
            else
            {
#if UNITY_EDITOR
                adUnitId = "unused";
#elif UNITY_ANDROID
        adUnitId = "ca-app-pub-1270408828484515/8410451177";
#elif UNITY_IPHONE || UNITY_iOS
        adUnitId = "ca-app-pub-1270408828484515/9003459104";
#else
        adUnitId = "unexpected_platform";
#endif
            }


            if (interstitialAd != null)
            {
                interstitialAd.Destroy();
            }
           
            interstitialAd = new InterstitialAd(adUnitId);
            interstitialAd.OnAdLoaded += (a, b) => isInterstitialAdsLoaded = true;
            //interstitialAd.OnAdFailedToLoad += (a, b) => { isInterstitialLoadFailed = true; };
            //interstitialAd.OnAdFailedToShow += (a, b) => { _OnFailed?.Invoke(); };
            interstitialAd.OnAdClosed += (a, b) => onInterstitialClosed?.Invoke();
            AdRequest request = new AdRequest.Builder().AddKeyword("unity-admob-sample").Build();
            interstitialAd.LoadAd(request);
        }
        public void ShowInterstitialAd(Action _OnFailed, Action _OnAdClosed)
        { 
            CoroutineExecuter.Excute(ShowInterstitialRoutine(_OnFailed, _OnAdClosed));

            IEnumerator ShowInterstitialRoutine(Action _OnFailed, Action _OnAdClosed)
            {
                if (!isInterstitialAdsLoaded)
                {
                    RequestInterstitialAD();
                }

                onInterstitialFailed = _OnFailed;
                onInterstitialClosed = _OnAdClosed;

                float timer = 1.5f;
                while (timer > 0f)
                {
                    timer -= Time.deltaTime;
                    
                
                    if (interstitialAd.IsLoaded())
                    {
                        interstitialAd.Show();
                        RequestInterstitialAD();
                        yield break;
                    }
                    yield return null;
                }
                RequestInterstitialAD();
                onInterstitialFailed?.Invoke();
            }

        }














        public void RequestRewardedAD()
        {
            isRewardAdsLoaded = false;
            string adUnitId = string.Empty;
            if (AdsManager.IsTestMode)
            {
#if UNITY_EDITOR
                adUnitId = "unused";
#elif UNITY_ANDROID
                adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
                adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
                adUnitId = "unexpected_platform";
#endif
            }
            else
            {
#if UNITY_EDITOR
                adUnitId = "unused";
#elif UNITY_ANDROID
                adUnitId = GetRewardAdsID();
#elif UNITY_IPHONE
                adUnitId = GetRewardAdsID();
#else
                adUnitId = "unexpected_platform";
#endif
            }



            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
            }

            rewardedAd = new RewardedAd(adUnitId);
            rewardedAd.OnAdClosed += (a, b) => onRewardedClosed?.Invoke();
            rewardedAd.OnAdLoaded += (a, b) => isRewardAdsLoaded = true;
            AdRequest request = new AdRequest.Builder().AddKeyword("unity-admob-sample").Build();
            rewardedAd.LoadAd(request);
        }
        public void ShowRewardedAd(Action _OnFailed, Action _OnAdClosed)
        {
            CoroutineExecuter.Excute(ShowRewardedRoutine(_OnFailed, _OnAdClosed));

            IEnumerator ShowRewardedRoutine(Action _OnFailed, Action _OnAdClosed)
            {
                if (!isRewardAdsLoaded)
                {
                    RequestRewardedAD();
                }

                onRewardedFailed = _OnFailed;
                onRewardedClosed = _OnAdClosed;

                //isInterstitialLoadFailed = false;
                //RequestInterstitialAD();

                float timer = 1.5f;
                while (timer > 0f)
                {
                    timer -= Time.deltaTime;
                
                    if (rewardedAd.IsLoaded())
                    {
                        rewardedAd.Show();
                        RequestRewardedAD();
                        yield break;
                    }
                    yield return null;
                }
                onRewardedFailed?.Invoke();
                RequestRewardedAD();
            }
        }

        private string GetRewardAdsID()
        {
            int nextRewardIndex = UnityEngine.Random.Range(0, rewardAdsIds.Length);
            while (nextRewardIndex == curRewardAdsIndex)
            {
                nextRewardIndex = UnityEngine.Random.Range(0, rewardAdsIds.Length);
            }
            curRewardAdsIndex = nextRewardIndex;
            return rewardAdsIds[curRewardAdsIndex];
        }
    }

}