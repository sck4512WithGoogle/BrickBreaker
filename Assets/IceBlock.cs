using UnityEngine;
using MJ.Data;
using MJ.Manager;
using System.Collections;

public class IceBlock : Block
{
    private void OnEnable()
    {
        currentPosY = 1;    
    }

    public void Suicide()
    {
        StartCoroutine(SuicideRoutine());
        IEnumerator SuicideRoutine()
        {
            while (spriteRenderer.color.a > 0f)
            {
                var color = spriteRenderer.color;
                color.a -= Time.deltaTime * 3.3f;
                spriteRenderer.color = color;
                yield return null;
            }
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D _Other)
    {
        if (_Other.gameObject.CompareTag(Tags.BallTag) || _Other.gameObject.CompareTag(Tags.GameOverTriggerTag))
        {
            var iceBlockBreakEffect = PoolManager.GetIceBlockBreakEffect();
            iceBlockBreakEffect.transform.position = transform.position;
            iceBlockBreakEffect.SetActive(true);

            GameManager.Instance.AddBall();
            gameObject.SetActive(false);
        }
    }
}
