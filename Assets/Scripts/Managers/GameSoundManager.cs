
namespace MJ.Manager
{
    using UnityEngine;

    public static class GameSoundManager
    {
        private static AudioClip blockTouchSound;
        private static AudioClip blockDestroySound;
        private static AudioClip magicBlockDestroySound;
        private static AudioClip addBallSound;


        public static void Init(AudioClip _BlockTouchSound, AudioClip _BlockDestroySound, AudioClip _MagicBlockDestroySound, AudioClip _AddBallSound)
        {
            blockTouchSound = _BlockTouchSound;
            blockDestroySound = _BlockDestroySound;
            magicBlockDestroySound = _MagicBlockDestroySound;
            addBallSound = _AddBallSound;
        }


        public static void PlayBlockTouchSound(float _Volume)
        {
            PlaySound(blockTouchSound, _Volume);
        }


        public static void PlayBlockDestroySound(float _Volume)
        {
            PlaySound(blockDestroySound, _Volume);
        }

        public static void PlayMagicBlockDestroySound(float _Volume)
        {
            PlaySound(magicBlockDestroySound, _Volume);
        }

        public static void PlayAddBallSound(float _Volume)
        {
            PlaySound(addBallSound, _Volume);
        }


        private static void PlaySound(AudioClip _Sound, float _Volume = 1f)
        {
            //사운드 켜져있는거 아니면 소리 안 냄
            if(!OptionManager.IsSound)
            {
                return;
            }

            var speaker = PoolManager.GetSpeaker();
            speaker.clip = _Sound;
            speaker.volume = _Volume;
            speaker.Play();
        }
    }

}
