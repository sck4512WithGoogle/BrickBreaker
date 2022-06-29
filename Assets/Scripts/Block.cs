using UnityEngine;

[DisallowMultipleComponent]
public class Block : Movable
{
    [SerializeField] protected SpriteRenderer spriteRenderer;

    protected int currentPosY;
    public int CurrentPosY => currentPosY;

    public override void MoveToBottom()
    {
        base.MoveToBottom();
        currentPosY++;
    }

    public void SetPosY(int _PosY)
    {
        currentPosY = _PosY;
    }
}
