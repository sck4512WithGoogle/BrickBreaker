
namespace MJ.Manager
{
    using UnityEngine;
    public static class OptionManager
    {
        #region 아이템 옵션
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
        #endregion


        #region 시스템 옵션
        public static bool IsSound => isSound;
        private static bool isSound;
        public static bool IsAiming => isAiming;
        private static bool isAiming;

        public static void SetSoundOption(bool _IsActive)
        {
            isSound = _IsActive;
            PlayerPrefs.SetInt(soundOptionKey, ChangeBoolToInt(isSound));
        }
        public static void SetAimingOption(bool _IsActive)
        {
            isAiming = _IsActive;
            PlayerPrefs.SetInt(aimingOptionKey, ChangeBoolToInt(isAiming));
        }

        private const string soundOptionKey = "SoundOption";
        private const string aimingOptionKey = "AimingOption";
        #endregion



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


            isSound = ChangeIntToBool(PlayerPrefs.GetInt(soundOptionKey, 1));
            isAiming = ChangeIntToBool(PlayerPrefs.GetInt(aimingOptionKey, 1));
        }
    }
}

