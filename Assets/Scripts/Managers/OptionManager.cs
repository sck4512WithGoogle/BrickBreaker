
namespace MJ.Manager
{
    using UnityEngine;
    public static class OptionManager
    {
        public static bool IsSpeedUpItemOptionOn
        {
            get => isSpeedUpItemOptionOn;
            set
            {
                isSpeedUpItemOptionOn = value;
                PlayerPrefs.SetInt(SpeedUpOptionStatusItemKey, ChangeBoolToInt(isSpeedUpItemOptionOn));
            }
        }
        public static bool IsPowerUpItemOptionOn
        {
            get => isPowerUpItemOptionOn;
            set
            {
                isPowerUpItemOptionOn = value;
                PlayerPrefs.SetInt(PowerUpOptionStatusItemKey, ChangeBoolToInt(isPowerUpItemOptionOn));
            }
        }
        public static bool IsTwoBoundItemOptionOn
        {
            get => isTwoBoundItemOptionOn;
            set
            {
                isTwoBoundItemOptionOn = value;
                PlayerPrefs.SetInt(TwoBoundStatusItemKey, ChangeBoolToInt(isTwoBoundItemOptionOn));
            }
        }

        private static bool isSpeedUpItemOptionOn;
        private static bool isPowerUpItemOptionOn;
        private static bool isTwoBoundItemOptionOn;

        private const string SpeedUpOptionStatusItemKey = "SpeedUpOption";
        private const string PowerUpOptionStatusItemKey = "PowerUpOption";
        private const string TwoBoundStatusItemKey = "TwoBoundOption";

        private static int ChangeBoolToInt(bool _Bool)
        {
            return _Bool ? 1 : 0;
        }

        private static bool ChangeIntToBool(int _Int)
        {
            return _Int == 1;
        }


        public static void Init()
        {
            isSpeedUpItemOptionOn = ChangeIntToBool(PlayerPrefs.GetInt(SpeedUpOptionStatusItemKey, 1));
            isPowerUpItemOptionOn = ChangeIntToBool(PlayerPrefs.GetInt(PowerUpOptionStatusItemKey, 1));
            isTwoBoundItemOptionOn = ChangeIntToBool(PlayerPrefs.GetInt(TwoBoundStatusItemKey, 1));
        }
    }
}

