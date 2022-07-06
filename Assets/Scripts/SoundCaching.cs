using UnityEngine;
using MJ.Manager;
public sealed class SoundCaching : MonoBehaviour
{
    [SerializeField] private AudioClip blockTouchSound;
    [SerializeField] private AudioClip blockDestroySound;
    [SerializeField] private AudioClip magicBlockDestroySound;
    [SerializeField] private AudioClip addBallSound;


    private void Awake()
    {
        GameSoundManager.Init(blockTouchSound, blockDestroySound, magicBlockDestroySound, addBallSound);
    }
}
