using UnityEngine;
using MJ.Data;
using System;
using MJ.Manager;
using UnityEngine.UI;


public class CommonBlock : Block
{
    [SerializeField] private SpriteRenderer secondBlockSpriteRenderer;
    [SerializeField] private Transform numbersParent;
    //[SerializeField] private Sprite takeDamageSprite;
    private static readonly float horizontalSize_4 = -94.3f;
    private static readonly float horizontalSize_3 = -95.5f;
    private static readonly float horizontalSize_2 = -96.8f;
    private static readonly int damageCountMax = 7; //0, 1, 2, 3, 4, 5, 6, 7 종류


    public event Action OnDisableAction;
    public int Count => count;
    private int count;

    public int CurrentDamageCount => currentDamageCount;
    private int currentDamageCount; //현재 데미지 받은 횟수

    private int disableScore;
    private GameObject[] numbers;
    private HorizontalLayoutGroup numberHorizontal;
    protected override void Awake()
    {
        base.Awake();
        numbers = new GameObject[0];
        numberHorizontal = numbersParent.GetComponent<HorizontalLayoutGroup>();
    }


    private void OnEnable()
    {
        var color = secondBlockSpriteRenderer.color;
        color.a = 1;
        secondBlockSpriteRenderer.color = color;
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



        SoundManager.PlayBlockSound();


        if (count > 0)
        {
            //ScoreManager.AddScore(1);
            RenderNumber(count);
        }
        else
        {
            Die();
        }
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
        numbersParent.DetachChildren();
        numbers = new GameObject[0];

        ScoreManager.AddScore(disableScore);
        gameObject.SetActive(false);
    }

    private void RenderNumber(int _Number)
    {
        //numberText.text = leftTouchCount.ToString();
        string temp = _Number.ToString();

        for (int i = 0; i < numbers.Length; i++)
        {
            numbers[i].SetActive(false);
        }
        numbersParent.DetachChildren();
        numbers = new GameObject[temp.Length];

        switch (numbers.Length)
        {
            case 4:
                numberHorizontal.spacing = horizontalSize_4;
                break;
            case 3:
                numberHorizontal.spacing = horizontalSize_3;
                break;
            case 2:
                numberHorizontal.spacing = horizontalSize_2;
                break;
        }

        for (int i = 0; i < numbers.Length; i++)
        {
            var num = PoolManager.GetNumber((int)temp[i] - 48);
            num.SetActive(true);
            numbers[i] = num;
            numbers[i].transform.SetParent(numbersParent);
            numbers[i].transform.localScale = Vector3.one;
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
