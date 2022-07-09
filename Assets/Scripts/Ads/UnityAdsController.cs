
namespace MJ.Ads
{
    using UnityEngine.Advertisements;
    using System;
    using System.Collections;
    using UnityEngine;

    public class UnityAdsController : IUnityAdsLoadListener, IUnityAdsShowListener
    {
        private string interstitialAdsID = string.Empty;
        private string rewardedAdsID = string.Empty;
        private string bannerAdsID = string.Empty;

        private bool isLoaded = false;
        
        private Action onInterstitialAdClosed;
        private Action onRewardedAdClosed;

        public void Init()
        {
#if UNITY_EDITOR
            interstitialAdsID = "Interstitial_Android";
            rewardedAdsID = "Rewarded_Android";
            bannerAdsID = "Banner_Android";
#elif UNITY_ANDROID
        interstitialAdsID = "Interstitial_Android";
        rewardedAdsID = "Rewarded_Android";
        bannerAdsID = "Banner_Android";
#elif UNITY_IPHONE
        interstitialAdsID = "Interstitial_iOS";
        rewardedAdsID = "Rewarded_iOS";
        bannerAdsID = "Banner_iOS";
#endif


            UnityAdsInitializer unityAdsInitializer = new UnityAdsInitializer();
            unityAdsInitializer.InititalizeAds();
        }


        public void OnUnityAdsAdLoaded(string placementId)
        {
            isLoaded = true;
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {

        }


        public void ShowBannerAd()
        {
            CoroutineExecuter.Excute(ShowBannerAdRoutine());
            IEnumerator ShowBannerAdRoutine()
            {
                if(!UnityAdsInitializer.IsUnityAdsOk)
                {
                    yield break;
                }

                yield return new WaitUntil(() => Advertisement.isInitialized && Advertisement.isSupported);
                Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
                Advertisement.Banner.Load(bannerAdsID);
                yield return new WaitUntil(() => Advertisement.Banner.isLoaded);
                Advertisement.Banner.Show(bannerAdsID);
            }
        }

        public void ShowInterstitialAd(Action _OnFailed, Action _OnClose)
        {
            onInterstitialAdClosed = _OnClose;

            CoroutineExecuter.Excute(ShowInterstitialAdRoutine(_OnFailed));

            IEnumerator ShowInterstitialAdRoutine(Action _OnFailed)
            {
                if (!UnityAdsInitializer.IsUnityAdsOk)
                {
                    _OnFailed?.Invoke();
                    yield break;
                }
   

                //제일 먼저 유니티 광고 자체 초기화 체크
                if (!Advertisement.isInitialized)
                {
                    yield return new WaitUntil(() => Advertisement.isInitialized && Advertisement.isSupported);
                }
                //광고 불러옴
                isLoaded = false;
                Advertisement.Load(interstitialAdsID, this);

                float waitTime = 1.5f;
                while (waitTime > 0f)
                {
                    waitTime -= Time.deltaTime;
                    if (isLoaded)
                    {
                        Advertisement.Show(interstitialAdsID, this);
                        yield break;
                    }
                    yield return null;
                }
                //만약 실행 안 됐으면 그냥 이거 실행해줌
                _OnFailed?.Invoke();
            }
        }

        public void ShowRewardedAd(Action _OnFailed, Action _OnClose)
        {
            onRewardedAdClosed = _OnClose;

            CoroutineExecuter.Excute(ShowRewardedAdRoutine(_OnFailed));
            IEnumerator ShowRewardedAdRoutine(Action _OnFailed)
            {
#if UNITY_EDITOR //테스트용
                //yield return new WaitForSeconds(2f);
                //_OnFailed.Invoke();
                //yield break;
#endif

                if (!UnityAdsInitializer.IsUnityAdsOk)
                {
                    _OnFailed?.Invoke();
                    yield break;
                }



                //제일 먼저 유니티 광고 자체 초기화 체크
                if (!Advertisement.isInitialized)
                {
                    yield return new WaitUntil(() => Advertisement.isInitialized && Advertisement.isSupported);
                }
                //광고 불러옴
                isLoaded = false;
                Advertisement.Load(rewardedAdsID, this);

                float waitTime = 1f;
                while (waitTime > 0f)
                {
                    waitTime -= Time.deltaTime;
                    if(isLoaded)
                    {
                        Advertisement.Show(rewardedAdsID, this);
                        yield break;
                    }
                    yield return null;
                }
                //만약 실행 안 됐으면 그냥 이거 실행해줌
                _OnFailed?.Invoke();
            }
        }



        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {

        }

        public void OnUnityAdsShowStart(string placementId)
        {

        }

        public void OnUnityAdsShowClick(string placementId)
        {

        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            onInterstitialAdClosed?.Invoke();
            onRewardedAdClosed?.Invoke();


            onInterstitialAdClosed = null;
            onRewardedAdClosed = null;
        }
    }
}