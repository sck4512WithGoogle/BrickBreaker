using UnityEngine;
using System.Collections;
using MJ.Manager;

[DisallowMultipleComponent]
public class MessageBoxAction : MonoBehaviour
{
    [SerializeField] private float biggingTime = 1f;
    private Vector3 startScale;
    private Transform myTransform;

    private void Awake()
    {
        myTransform = transform;
        startScale = myTransform.localScale;
    }

    private void OnEnable()
    {
        StartCoroutine(DoSizeEffectRoutine());
    }
      

    private IEnumerator DoSizeEffectRoutine()
    {
        myTransform.localScale = startScale;


        var originPos = myTransform.localScale;
        myTransform.localScale = new Vector3(originPos.x * 0.98f, originPos.y * 0.98f, 1f);
        float time = 0f;
        while (time < 0.1f)
        {
            time += Time.fixedDeltaTime;
            myTransform.localScale += Vector3.right * Time.fixedDeltaTime * 0.46f + Vector3.up * Time.fixedDeltaTime * 0.46f;
            yield return YieldContainer.GetWaitForSecondsRealtime(0.02f);
        }


        time = 0f;
        while (time < 0.03f)
        {
            time += Time.fixedDeltaTime;
            myTransform.localScale += Vector3.left * Time.fixedDeltaTime * 0.7f + Vector3.down * Time.fixedDeltaTime * 0.7f;
            yield return YieldContainer.GetWaitForSecondsRealtime(0.02f);
        }
        myTransform.localScale = originPos;
    }
}