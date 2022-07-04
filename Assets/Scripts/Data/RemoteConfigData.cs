
namespace MJ.Data
{
    using System.Collections.Generic;
    using UnityEngine;
    using Unity.RemoteConfig;
    using System;
    using MJ.Data;
    public class RemoteConfigData : MonoBehaviour
    {
        static readonly string environmentID = "2c6514d2-bd58-43b3-9c67-87cd93cdce2e";
        struct A
        { }
        struct B
        { }
        public static bool IsLoaded { get; set; }

        public static Dictionary<StoreType, string> StoreReviewURLs => storeReviewURLs;
        private static Dictionary<StoreType, string> storeReviewURLs;


        static RemoteConfigData()
        {
            ConfigManager.SetEnvironmentID(environmentID);
            ConfigManager.FetchCompleted += OnFetchCompleted;
        }

        public static void LoadData()
        {
            if (IsLoaded)
            {
                return;
            }

            ConfigManager.FetchConfigs<A, B>(new A(), new B());
        }

        static void OnFetchCompleted(ConfigResponse _ConfigResponse)
        {
            storeReviewURLs = new Dictionary<StoreType, string>();
            var data = ConfigManager.appConfig.GetJson("StoreReviewSiteData");
            var dictionaryData = JsonUtility.FromJson<DictionaryData<string, string>>(data).MyDictionary;

            foreach (var dicData in dictionaryData)
            {
                storeReviewURLs.Add(StringToStoreTypeEnum(dicData.Key), dicData.Value);
            }


            IsLoaded = true;

            StoreType StringToStoreTypeEnum(string _StoreTypeString)
            {
                var enums = Enum.GetValues(typeof(StoreType));
                foreach (var storeType in enums)
                {
                    if(storeType.ToString().Equals(_StoreTypeString))
                    {
                        return (StoreType)storeType;
                    }
                }

                return StoreType.GooglePlayStore;
            }
        }
    }
}