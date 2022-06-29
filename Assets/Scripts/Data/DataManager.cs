
namespace MJ.Data
{
    using UnityEngine;
    using System;

    public enum StoreType
    {
        GooglePlayStore,OneStore, GalaxyStore, AppStore
    }
    public sealed class DataManager : MonoBehaviour
    {
        public static DataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<DataManager>();
                    if (instance == null)
                    {
                        instance = new GameObject("DataManager").AddComponent<DataManager>();
                    }
                }
                return instance;
            }
        }
        private static DataManager instance;

        [SerializeField] private StoreType storeType = StoreType.GooglePlayStore;
        public StoreType CurrentStoreType => storeType;
        public bool IsGuestLoginDone { get; set; } = false;


        public event Action<int> OnSpeedUpItemCountChange;
        public event Action<int> OnPowerUpItemCountChange;
        public event Action<int> OnTwoBoundItemCountChange;

        public int SpeedUpItemCount
        {
            get => speedUpItemCount;
            set
            {
                speedUpItemCount = value;
                PlayerPrefs.SetInt(SpeedUpItemKey, speedUpItemCount);
                OnSpeedUpItemCountChange.Invoke(speedUpItemCount);
            }
        }
        public int PowerUpItemCount
        {
            get => powerUpItemCount;
            set
            {
                powerUpItemCount = value;
                PlayerPrefs.SetInt(PowerUpItemKey, powerUpItemCount);
                OnPowerUpItemCountChange.Invoke(powerUpItemCount);
            }
        }
        public int TwoBoundItemCount
        {
            get => twoBoundItemCount;
            set
            {
                twoBoundItemCount = value;
                PlayerPrefs.SetInt(TwoBoundItemKey, twoBoundItemCount);
                OnTwoBoundItemCountChange.Invoke(twoBoundItemCount);
            }
        }

        private int speedUpItemCount;
        private int powerUpItemCount;
        private int twoBoundItemCount;

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

        public bool IsContinuePlay { get; set; } = false;

        public void Awake()
        {
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            speedUpItemCount = PlayerPrefs.GetInt(SpeedUpItemKey, Constants.SpeedUpItemCountMax);
            powerUpItemCount = PlayerPrefs.GetInt(PowerUpItemKey, Constants.PowerUpItemCountMax);
            twoBoundItemCount = PlayerPrefs.GetInt(TwoBoundItemKey, Constants.TwoBoundItemCountMax);
        }
    }

}
