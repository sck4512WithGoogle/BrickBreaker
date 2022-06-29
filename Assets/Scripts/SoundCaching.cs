using UnityEngine;
using MJ.Manager;
public sealed class SoundCaching : MonoBehaviour
{
    [SerializeField] private AudioSource[] blockSounds;
    [SerializeField] private AudioSource[] greenOrbSounds;

    private void Awake()
    {
        SoundManager.Init(blockSounds, greenOrbSounds);
    }
}
