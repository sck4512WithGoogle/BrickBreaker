using UnityEngine;

namespace MJ.Manager
{
    public static class SoundManager
    {
        private static AudioSource[] blockSounds;
        private static AudioSource[] greenOrbSounds;

        public static void Init(AudioSource[] _BlockSounds, AudioSource[] _GreenOrbSounds)
        {
            blockSounds = _BlockSounds;
            greenOrbSounds = _GreenOrbSounds;
        }

        public static void PlayBlockSound()
        {
            foreach (var sound in blockSounds)
            {
                if(!sound.isPlaying)
                {
                    sound.Play();
                    break;
                }
            }
        }

        public static void PlayGreenOrbSound()
        {
            foreach (var sound in greenOrbSounds)
            {
                if (!sound.isPlaying)
                {
                    sound.Play();
                    break;
                }
            }
        }
    }

}
