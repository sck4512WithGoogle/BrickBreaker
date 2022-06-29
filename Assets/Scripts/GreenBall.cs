using System.Collections;
using UnityEngine;
using System;

public class GreenBall : MonoBehaviour
{
    private Transform myTransform;

    private void Awake()
    {
        myTransform = transform;
    }


    public void MoveToTargetAndActiveOff(Vector3 _TargetPos, Action _OnEnd)
    {
        Action onEnd = () =>
        {
            _OnEnd.Invoke();
            gameObject.SetActive(false);
        };
        var speed = (myTransform.position - _TargetPos).magnitude / 12f;
        StartCoroutine(MoveToTargetRoutine(_TargetPos, speed, onEnd));
    }

    public void MoveToTarget(Vector3 _TargetPos, float _Speed = 2.5f)
    {
        StartCoroutine(MoveToTargetRoutine(_TargetPos, _Speed));
    }

    IEnumerator MoveToTargetRoutine(Vector3 _TargetPos, float _Speed, Action _OnEnd = null)
    {
        while ((myTransform.position - _TargetPos).magnitude > 0.01f)
        {
            myTransform.position = Vector3.MoveTowards(myTransform.position, _TargetPos, _Speed);
            yield return null;
        }
        myTransform.position = _TargetPos;
        _OnEnd?.Invoke();
    }
}
