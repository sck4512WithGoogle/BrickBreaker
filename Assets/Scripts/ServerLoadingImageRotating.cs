using System.Collections;
using UnityEngine;
using MJ.Manager;


public class ServerLoadingImageRotating : MonoBehaviour
{
    private float rotatingSpeed = 330f;
    private Transform myTransform;

    private void Awake()
    {
        myTransform = GetComponent<Transform>();
    }

    private void OnEnable()
    {
        myTransform.rotation = Quaternion.identity;
        StartCoroutine(RotatingRoutine());
    }

    private void OnDisable()
    {
        StopCoroutine(RotatingRoutine());
    }

    //시계방향으로 계속 회전
    private IEnumerator RotatingRoutine()
    {
        while (true)
        {
            myTransform.Rotate(Vector3.forward, -0.02f * rotatingSpeed);
            yield return YieldContainer.GetWaitForSecondsRealtime(0.02f);
        }
    }
}
