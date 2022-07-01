using UnityEngine;
using MJ.Manager;

[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
public class CanvasCaching : MonoBehaviour
{
    private void OnEnable()
    {
        UICanvasCachinger.SetCanvas(GetComponent<Canvas>());        
    }

    private void OnDisable()
    {
        UICanvasCachinger.SetCanvas(null);
    }
}
