using UnityEngine;
using MJ.Data;
using MJ.Manager;


public class IceBlock : Block
{
    private void OnEnable()
    {
        currentPosY = 1;    
    }

    protected override void Die()
    {
        var iceBlockBreakEffect = PoolManager.GetIceBlockBreakEffect();
        iceBlockBreakEffect.transform.position = transform.position;
        iceBlockBreakEffect.SetActive(true);

        GameManager.Instance.AddBall();
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D _Other)
    {
        if (_Other.gameObject.CompareTag(Tags.BallTag) || _Other.gameObject.CompareTag(Tags.GameOverTriggerTag))
        {
            Die();
        }
    }
}
