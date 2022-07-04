

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
            gameID = "4819405"; //�׳� �ȵ���̵�� ����
#elif UNITY_ANDROID
        gameID = "4819405";
#elif UNITY_IPHONE
        gameID = "4819404";
#endif
            try
            {
                Debug.Log("����Ƽ �ʱ�ȭ1");
                Advertisement.Initialize(gameID, AdsManager.IsTestMode, this);
                Debug.Log("����Ƽ �ʱ�ȭ2");
                isUnityAdsOk = true;
                Debug.Log("����Ƽ �ʱ�ȭ3");
            }
            catch (System.Exception)
            {
                isUnityAdsOk = false;
            }
            isUnityAdsOk = false;
        }


        public void OnInitializationComplete()
        {
            //Debug.Log("�ʱ�ȭ ��");
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            //Debug.Log("�ʱ�ȭ ����");
        }
    }
}