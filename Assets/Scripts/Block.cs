using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class Block : Movable
{
    [SerializeField] protected SpriteRenderer spriteRenderer;

    protected static int suicideDefinition = 3;
    protected int currentPosY;
    public int CurrentPosY => currentPosY;
    public bool IsScaleChangeDone { get; private set; }

    public sealed override void MoveToBottom()
    {
        base.MoveToBottom();
        currentPosY++;
    }

    public void SetPosY(int _PosY)
    {
        currentPosY = _PosY;
    }

    public void Suicide()
    {
        if(currentPosY <= suicideDefinition)
        {
            return;
        }

        Die();
    }

    protected virtual void Die()
    {}

    public void OnCreateBlock()
    {
        IsScaleChangeDone = false;
        StartCoroutine(OnCreateBlockRoutine());
    }

    private IEnumerator OnCreateBlockRoutine()
    {
        //어차피 정육면체임
        var startScale = myTransform.localScale;
        myTransform.localScale = Vector3.zero;

        float speed = 33f;
        float movedLength = 0f;
        while (movedLength < startScale.x)
        {
            movedLength += Time.deltaTime * speed;
            myTransform.localScale = Vector3.one * movedLength;
            yield return null;
        }
        myTransform.localScale = startScale;

        //크기 바꾸는 거 다 끝남
        IsScaleChangeDone = true;
    }
}
