using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class NumberText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private Transform myTransform;
    private Color startColor;

    private void Awake()
    {
        startColor = text.color;
        myTransform = transform;
    }

    private void OnEnable()
    {
        text.color = startColor;
    }


    public void SetNumber(int _Number)
    {
        text.text = $"+{_Number}";
    }

    public void MoveUp(Action _OnEnd)
    {
        StartCoroutine(MoveUpRoutine(_OnEnd));
    }

    private IEnumerator MoveUpRoutine(Action _OnEnd)
    {
        var waitForFixed = new WaitForFixedUpdate();
        float moveLength = 1.2f;
        float length = 0f;
        while (length < moveLength)
        {
            length += Time.fixedDeltaTime;
            myTransform.position += Vector3.up * Time.fixedDeltaTime * 4f;

            var color = text.color;
            color.a -= Time.fixedDeltaTime * 0.6f;
            text.color = color;
            yield return waitForFixed;
        }

        text.color = new Color(1f, 1f, 1f, 0f);
        _OnEnd.Invoke();
    }
}
