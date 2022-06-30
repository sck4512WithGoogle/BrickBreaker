
namespace MJ.Data
{
    using UnityEngine;
    using System;

    public enum StoreType
    {
        GooglePlayStore,OneStore, GalaxyStore, AppStore
    }
    public static class DataManager
    {
        private static StoreType storeType = StoreType.GooglePlayStore;
        public static StoreType CurrentStoreType => storeType;
     

        public static event Action<int> OnSpeedUpItemCountChange;
        public static event Action<int> OnPowerUpItemCountChange;
        public static event Action<int> OnTwoBoundItemCountChange;

        public static int SpeedUpItemCount
        {
            get => speedUpItemCount;
            set
            {
                speedUpItemCount = value;
                PlayerPrefs.SetInt(SpeedUpItemKey, speedUpItemCount);
                OnSpeedUpItemCountChange.Invoke(speedUpItemCount);
            }
        }
        public static int PowerUpItemCount
        {
            get => powerUpItemCount;
            set
            {
                powerUpItemCount = value;
                PlayerPrefs.SetInt(PowerUpItemKey, powerUpItemCount);
                OnPowerUpItemCountChange.Invoke(powerUpItemCount);
            }
        }
        public static int TwoBoundItemCount
        {
            get => twoBoundItemCount;
            set
            {
                twoBoundItemCount = value;
                PlayerPrefs.SetInt(TwoBoundItemKey, twoBoundItemCount);
                OnTwoBoundItemCountChange.Invoke(twoBoundItemCount);
            }
        }

        private static int speedUpItemCount;
        private static int powerUpItemCount;
        private static int twoBoundItemCount;

        private const string SpeedUpItemKey = "SpeedUp";
        private const string PowerUpItemKey = "PowerUp";
        private const string TwoBoundItemKey = "TwoBound";

        public static int BallDamage
        {
            get => ballDamage;
            set
            {
                ballDamage = value;
                if (ballDamage < 1)
                    ballDamage = 1;
            }
        }
        public static int ballDamage = 1;

        public static bool IsContinuePlay { get; set; } = false;

        public static void Init()
        {
#if UNITY_EDITOR
            storeType = StoreType.GooglePlayStore;
#endif

            speedUpItemCount = PlayerPrefs.GetInt(SpeedUpItemKey, Constants.SpeedUpItemCountMax);
            powerUpItemCount = PlayerPrefs.GetInt(PowerUpItemKey, Constants.PowerUpItemCountMax);
            twoBoundItemCount = PlayerPrefs.GetInt(TwoBoundItemKey, Constants.TwoBoundItemCountMax);
        }
    }

}
