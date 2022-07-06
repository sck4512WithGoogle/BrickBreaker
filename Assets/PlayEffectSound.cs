using UnityEngine;
using UnityEngine.EventSystems;
using MJ.Manager;

public class PlayEffectSound : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private AudioSource speaker;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(!OptionManager.IsSound)
        {
            return;
        }

        speaker.Play();
    }
}
