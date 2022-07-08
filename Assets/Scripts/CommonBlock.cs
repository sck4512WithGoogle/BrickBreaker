using UnityEngine;
using MJ.Data;
using System;
using MJ.Manager;
using UnityEngine.UI;
using System.Collections;

public class CommonBlock : Block
{
    [Header("NumberRender")]
    [SerializeField] private Transform pos_1;
    [SerializeField] private Transform[] poses_2;
    [SerializeField] private Transform[] poses_3;
    [SerializeField] private Transform[] poses_4;

    [Header("SpriteRenderer")]
    [SerializeField] private SpriteRenderer secondBlockSpriteRenderer;
    [SerializeField] private SpriteRenderer effectSpriteRenderer;
    private static readonly int damageCountMax = 7; //0, 1, 2, 3, 4, 5, 6, 7 종류


    public event Action OnDisableAction;
    public int Count => count;
    private int count;

    public int CurrentDamageCount => currentDamageCount;
    private int currentDamageCount; //현재 데미지 받은 횟수

    private int disableScore;
    private GameObject[] numbers;
    
    protected override void Awake()
    {
        base.Awake();
        numbers = new GameObject[0];
    }


    private void OnEnable()
    {
        var color = secondBlockSpriteRenderer.color;
        color.a = 1;
        secondBlockSpriteRenderer.color = color;


        effectSpriteRenderer.color = new Color(1, 1, 1, 0);
    }

    public void SetDamageCount(int _DamageCount)
    {
        currentDamageCount = _DamageCount;
        UpdateSecondBlockSpriteRenderer();
    }
    private void UpdateSecondBlockSpriteRenderer()
    {
        var color = secondBlockSpriteRenderer.color;
        color.a = Mathf.Clamp01(1f - (currentDamageCount / (float)damageCountMax));
        secondBlockSpriteRenderer.color = color;
    }
    private void TakeDamage()
    {
        count -= DataManager.BallDamage;
        currentDamageCount += DataManager.BallDamage;
        UpdateSecondBlockSpriteRenderer();


        if (count > 0)
        {
            StopCoroutine("TakeDamageEffect");
            StartCoroutine("TakeDamageEffect");
            GameSoundManager.PlayBlockTouchSound(0.6f);
            //ScoreManager.AddScore(1);
            RenderNumber(count);
        }
        else
        {
            Die();
        }
    }

    IEnumerator TakeDamageEffect()
    {
        effectSpriteRenderer.color = new Color(1, 1, 1, 0);
        float speed = 3f;
        while (effectSpriteRenderer.color.a < 0.2f)
        {
            var color = effectSpriteRenderer.color;
            color.a += Time.deltaTime * speed;
            effectSpriteRenderer.color = color;
            yield return YieldContainer.WaitForFixedUpdate;
        }

        while (effectSpriteRenderer.color.a > 0f)
        {
            var color = effectSpriteRenderer.color;
            color.a -= Time.deltaTime * speed;
            effectSpriteRenderer.color = color;
            yield return YieldContainer.WaitForFixedUpdate;
        }
        effectSpriteRenderer.color = new Color(1, 1, 1, 0);
    }


    protected override void Die()
    {
        var blockBreakEffect = PoolManager.GetBlockBreakEffect();
        blockBreakEffect.transform.position = myTransform.position;
        blockBreakEffect.SetActive(true);

        for (int i = 0; i < numbers.Length; i++)
        {
            numbers[i].SetActive(false);
        }
        //numbersParent.DetachChildren();
        numbers = new GameObject[0];

        //소리냄
        GameSoundManager.PlayBlockDestroySound(0.6f);


        ScoreManager.AddScore(disableScore);
        gameObject.SetActive(false);
    }

    private void RenderNumber(int _Number)
    {
        string temp = _Number.ToString();

        for (int i = 0; i < numbers.Length; i++)
        {
            numbers[i].SetActive(false);
            numbers[i].transform.SetParent(null);
        }
  
        numbers = new GameObject[temp.Length];
        Transform[] poses = new Transform[numbers.Length];
        switch (numbers.Length)
        {
            case 1:
                poses[0] = pos_1;
                break;
            case 2:
                poses[0] = poses_2[0];
                poses[1] = poses_2[1];
                break;
            case 3:
                poses[0] = poses_3[0];
                poses[1] = poses_3[1];
                poses[2] = poses_3[2];
                break;
            case 4:
                poses[0] = poses_4[0];
                poses[1] = poses_4[1];
                poses[2] = poses_4[2];
                poses[3] = poses_4[3];
                break;
        }

        for (int i = 0; i < numbers.Length; i++)
        {
            var num = PoolManager.GetNumber((int)temp[i] - 48);
            num.SetActive(true);
            numbers[i] = num;
            numbers[i].transform.SetParent(poses[i]);
            numbers[i].transform.localScale = Vector3.one;
            numbers[i].transform.localPosition = Vector3.zero;
        }
    }

    public void SetNumber(int _Number)
    {
        count = _Number;
        RenderNumber(_Number);
        currentPosY = 1; 

        if(_Number <= damageCountMax)
        {
            //이렇게 해야함 (알파값 맞출려고)
            currentDamageCount = damageCountMax - _Number + 1;
        }
        else
        {
            currentDamageCount = 0;
        }


        disableScore = 1;
        if(20 < count)
        {
            disableScore++;
        }

        if (80 < count)
        {
            disableScore++;
        }

        if (200 < count)
        {
            disableScore++;
        }

        if (300 < count)
        {
            disableScore++;
        }
    }


    private void OnCollisionEnter2D(Collision2D _Other)
    {
        if(_Other.gameObject.CompareTag(Tags.BallTag))
        {
            TakeDamage();
        }
    }    
}
