using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class Block : Movable
{
    [SerializeField] protected SpriteRenderer spriteRenderer;

    protected static int suicideDefinition = 4;
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
        StartCoroutine(OnCreateBlockRoutine());
    }

    private IEnumerator OnCreateBlockRoutine()
    {
        //�����̰� �ִ� ��
        IsScaleChangeDone = false;

        //������ ������ü��
        var startScale = myTransform.localScale;
        myTransform.localScale = Vector3.zero;

        float speed = 13f;
        float movedLength = 0f;
        while (movedLength < startScale.x)
        {
            movedLength += Time.deltaTime * speed;
            myTransform.localScale = Vector3.one * movedLength;
            yield return null;
        }
        myTransform.localScale = startScale;

        //ũ�� �ٲٴ� �� �� ����
        IsScaleChangeDone = true;


        MoveToBottom();
    }
}
