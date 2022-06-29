using UnityEngine;

[DisallowMultipleComponent]
public class Block : Movable
{
    [SerializeField] protected SpriteRenderer spriteRenderer;

    protected static int suicideDefinition = 4;
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

    public void Suicide()
    {
        if(currentPosY <= suicideDefinition)
        {
            return;
        }

        Die();
    }

    protected virtual void Die()
    {

    }
}
