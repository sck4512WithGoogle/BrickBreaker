using UnityEngine;
using MJ.Data;
using MJ.Manager;
using System.Collections;


public class MagicBlock : Block
{
    [SerializeField] private SpriteRenderer secondBlockSpriteRenderer;
    
    private readonly static float timePeriod = 1.5f;
    private static float currentTime = 0f;
    private static Coroutine changeTimePeriodRoutine;
    public static void ChangeTimePeriod()
    {
        changeTimePeriodRoutine = CoroutineExecuter.Excute(ChangeTimePeriodRoutine());
    }
    public static void StopChangeTimePeriod()
    {
        CoroutineExecuter.MyStopCoroutine(changeTimePeriodRoutine);
    }
    private static IEnumerator ChangeTimePeriodRoutine()
    {
        float max = timePeriod * 2f;
        while (true)
        {
            currentTime = 0f;
            while (currentTime < max)
            {
                currentTime += Time.deltaTime;
                yield return null;
            }
        }
    }



    private void OnEnable()
    {
        currentPosY = 1;    
    }

    private void Update()
    {
        if(currentTime > 0f)
        {
            //0 ~ 1.5사이일 때
            if (currentTime <= timePeriod)
            {
                SetSecondBlockAlpha(currentTime / timePeriod);
            }
            else //1.5 ~ 3사이일 때
            {
                SetSecondBlockAlpha(2f - (currentTime / timePeriod));
            }
        }
        else
        {
            SetSecondBlockAlpha(0);
        }
    }

    private void SetSecondBlockAlpha(float _Alpha)
    {
        var color = secondBlockSpriteRenderer.color;
        color.a = _Alpha;
        secondBlockSpriteRenderer.color = color;
    }

    protected override void Die()
    {
        var iceBlockBreakEffect = PoolManager.GetIceBlockBreakEffect();
        iceBlockBreakEffect.transform.position = transform.position;
        iceBlockBreakEffect.SetActive(true);

        GameSoundManager.PlayMagicBlockDestroySound(0.6f);

        var numberText = PoolManager.GetNumberText();
        numberText.gameObject.SetActive(true);
        numberText.transform.position = myTransform.position;
        numberText.SetNumber(1);
        numberText.MoveUp(() => 
        {
            //위로가서
            GameSoundManager.PlayAddBallSound(0.5f);
            GameManager.Instance.AddBall();
        });


        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D _Other)
    {
        if (_Other.gameObject.CompareTag(Tags.BallTag) || _Other.gameObject.CompareTag(Tags.GameOverTriggerTag))
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D _Other)
    {
        if(_Other.CompareTag(Tags.GameOverTriggerTag))
        {
            Die();
        }
    }
}
