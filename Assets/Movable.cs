using MJ.Data;
using System.Collections;
using UnityEngine;

public class Movable : MonoBehaviour
{
    public bool IsMoving { get; set; } = false;
    private Transform myTransform;
    protected virtual void Awake()
    {
        myTransform = transform;
    }

    public virtual void MoveToBottom()
    {
        IsMoving = true;
        StartCoroutine(MoveToBottomRoutine());
    }

    private IEnumerator MoveToBottomRoutine()
    {
        //yield return MJ.Manager.YieldContainer.GetWaitSeconds(0.2f);
        Vector3 targetPos = myTransform.position + new Vector3(0, -Constants.BlockColumnSize, 0);

        var firstTargetPos = targetPos + Vector3.up * -3f;
        while ((myTransform.position - firstTargetPos).magnitude > 0.1f)
        {
            myTransform.position = Vector3.Lerp(myTransform.position, firstTargetPos, 0.2f);
            yield return null;
        }
        myTransform.position = firstTargetPos;


        while ((myTransform.position - targetPos).magnitude > 0.01f)
        {
            myTransform.position = Vector3.Lerp(myTransform.position, targetPos, 0.2f);
            yield return null;
        }
        myTransform.position = targetPos;

        IsMoving = false;
    }
}
