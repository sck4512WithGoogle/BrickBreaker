using UnityEngine;
using System.Collections;
using UnityEngine.UI;


[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class MessageBoxAction : MonoBehaviour
{
    [SerializeField] private float biggingTime = 1f;
    private RectTransform rectTransform;
    private Vector3 startScale;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        startScale = rectTransform.localScale;
        //images = GetComponentsInChildren<Image>();
    }

    private void OnEnable()
    {
        //rectTransform.localScale *= 0.01f;
        ////ÄÃ·¯ ¾ËÆÄ°ª 0.3À¸·Î ³·Ãã
        //for (int i = 0; i < images.Length; i++)
        //{
        //    var color = images[i].color;
        //    color.a = 0.1f;
        //    images[i].color = color;
        //}

        //StartCoroutine(BiggingEvent(biggingTime, 0.1f));

        StartCoroutine(DoSizeEffectRoutine());
    }


   

    private IEnumerator DoSizeEffectRoutine()
    {
        var originPos = transform.localScale;
        transform.localScale = new Vector3(originPos.x * 0.98f, originPos.y * 0.98f, 1f);
        float time = 0f;
        while (time < 0.1f)
        {
            time += Time.fixedDeltaTime;
            transform.localScale += Vector3.right * Time.fixedDeltaTime * 0.46f + Vector3.up * Time.fixedDeltaTime * 0.46f;
            yield return new WaitForFixedUpdate();
        }


        time = 0f;
        while (time < 0.03f)
        {
            time += Time.fixedDeltaTime;
            transform.localScale += Vector3.left * Time.fixedDeltaTime * 0.7f + Vector3.down * Time.fixedDeltaTime * 0.7f;
            yield return new WaitForFixedUpdate();
        }
        transform.localScale = originPos;
    }
}