using UnityEngine;
using MJ.Data;
using System;
using MJ.Manager;
using UnityEngine.UI;


public class CommonBlock : Block
{
    [SerializeField] private Transform numbersParent;
    [SerializeField] private Sprite takeDamageSprite;
    private static readonly float horizontalSize_4 = -94.3f;
    private static readonly float horizontalSize_3 = -95.5f;
    private static readonly float horizontalSize_2 = -96.8f;

    public event Action OnDisableAction;
    public int Count => count;
    private int count;
    public bool HasTakeDamaged => spriteRenderer.sprite == takeDamageSprite;

    private int disableScore;
    private GameObject[] numbers;
    private Sprite startSprite;
    private Vector3 startScale;
    private Vector3 takeDamageScale;
    private HorizontalLayoutGroup numberHorizontal;
    protected override void Awake()
    {
        base.Awake();
        startScale = transform.localScale;
        takeDamageScale = new Vector3(startScale.x, startScale.y - 0.02f, startScale.z);
        numbers = new GameObject[0];

        startSprite = spriteRenderer.sprite;
        numberHorizontal = numbersParent.GetComponent<HorizontalLayoutGroup>();
    }

    private void OnEnable()
    {
        spriteRenderer.sprite = startSprite;
        transform.localScale = startScale;
    }

   
    public void SetTakeDamage(bool _IsTakeDamaged)
    {
        if(_IsTakeDamaged)
        {
            spriteRenderer.sprite = takeDamageSprite;
        }
    }

    protected override void Die()
    {
        var blockBreakEffect = PoolManager.GetBlockBreakEffect();
        blockBreakEffect.transform.position = transform.position;
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

        disableScore = 1;
        if(20 < count)
        {
            disableScore++;
        }

        if (40 < count)
        {
            disableScore++;
        }

        if (80 < count)
        {
            disableScore++;
        }

        if (150 < count)
        {
            disableScore++;
        }
    }


    private void OnCollisionEnter2D(Collision2D _Other)
    {
        if(_Other.gameObject.CompareTag(Tags.BallTag))
        {
            count -= DataManager.BallDamage;
            transform.localScale = takeDamageScale;
            spriteRenderer.sprite = takeDamageSprite;

            SoundManager.PlayBlockSound();


            if(count > 0)
            {
                ScoreManager.AddScore(1);
                RenderNumber(count);
            }
            else
            {
                Die();
            }
        }
    }    
}
