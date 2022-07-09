using MJ.Data;
using MJ.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


using Random = UnityEngine.Random;

public struct BallCountAndDamage
{
    public int Count;
    public int Damage;
}

[DisallowMultipleComponent]
public sealed  class GameManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnTransforms;
    [SerializeField] private BallShootInputListner ballShootInputListener;
    [SerializeField] private Transform arrowTransform;
    [SerializeField] private Transform ballPreviewTransform;
    [SerializeField] private LineRenderer ballShootLineRenderer;
    [SerializeField] private LineRenderer ballShootLineRenderer2;
    [SerializeField] private Ball startBall; //���� ó�� �ִ� ��
   

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI ballCountText;

    public event Action OnStartShootBall; //�� ��� ����
    public event Action OnBlockCreateDone; //���� �� ��������� �� �� ����Ǵ� �׼�


    public static GameManager Instance => instance;
    private static GameManager instance;
    private const float circleRaycaseRadius = 1.35f;  

    private int round;
    private Vector3 firstClickPos;
    private Vector3 secondClickPos;
    private Vector3 shootDirection;
    private float shootDirectionLength;
    private Vector3 totalBallPos;
    public Vector3 TotalBallPos => totalBallPos;


    private List<Ball> balls;
    private HashSet<CommonBlock> commonBlocks;
    private HashSet<MagicBlock> iceBlocks;
    private HashSet<Movable> moves;

    private int currentBallCount;
    public bool HasResurrected => hasResurrected;
    private bool hasResurrected = false; //��Ȱ ����ߴ���

    private void Awake()
    {     
        PoolManager.Init();
        ScoreManager.Init();
   
        instance = this;
        round = 1;
        currentBallCount = 1;

        totalBallPos = new Vector3(0f, Constants.BottomY, 0f);
        balls = new List<Ball>();
       
        commonBlocks = new HashSet<CommonBlock>();
        iceBlocks = new HashSet<MagicBlock>();
        moves = new HashSet<Movable>();
    }

    private void OnDisable()
    {
        //���� ������ ��
        MagicBlock.StopChangeTimePeriod();    
    }

    private void Start()
    {
        ballShootInputListener.OnBeginDragAction += _Position =>
        {
            firstClickPos = Camera.main.ScreenToWorldPoint(_Position) + Vector3.forward * 10f;
                       

            shootDirectionLength = 0f;

            arrowTransform.gameObject.SetActive(true);

            //���̹� �ɼ� ���������� ������
            ballPreviewTransform.gameObject.SetActive(OptionManager.IsAiming);
        };

        ballShootInputListener.OnDragAction += _Position =>
        {
            secondClickPos = Camera.main.ScreenToWorldPoint(_Position) + Vector3.forward * 10f;
            



            shootDirection = secondClickPos - firstClickPos;
            shootDirectionLength = shootDirection.magnitude;

            shootDirection.Normalize();
            shootDirection = new Vector3(shootDirection.y >= 0 ? shootDirection.x : (shootDirection.x >= 0 ? 1 : -1), Mathf.Clamp(shootDirection.y, 0.2f, 1), 0f);

            
            arrowTransform.position = startBall.transform.position; //���� totalBallPos�ε� ���� �־
            arrowTransform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg);
            
            //���� �ɼ� �� ���¸� �� ������
            if(!OptionManager.IsAiming)
            {
                return;
            }



            //2�ٿ�� ����
            bool isTwoBound = OptionManager.IsTwoBoundItemOptionOn;
            
            
            var firstBoundCollider = Physics2D.CircleCast(new Vector2(Mathf.Clamp(totalBallPos.x, -54, 54), Constants.BottomY), circleRaycaseRadius, shootDirection, Mathf.Infinity, 1 << LayerMask.NameToLayer(Layers.WallLayerName) | 1 << LayerMask.NameToLayer(Layers.BlockLayerName));
            ballShootLineRenderer.SetPosition(0, totalBallPos);
            ballShootLineRenderer.SetPosition(1, firstBoundCollider.centroid);
           
            if (isTwoBound)
            {
                Vector3 secondShootDirection = Vector2.Reflect(shootDirection, firstBoundCollider.normal);
                secondShootDirection.Normalize();

                var secondPos = Physics2D.CircleCast(firstBoundCollider.centroid + (Vector2)secondShootDirection * 0.02f, circleRaycaseRadius, secondShootDirection, Mathf.Infinity, 1 << LayerMask.NameToLayer(Layers.WallLayerName) | 1 << LayerMask.NameToLayer(Layers.BlockLayerName)).centroid;

                if (secondPos.y < Constants.BottomY)
                    secondPos.y = Constants.BottomY;
                ballPreviewTransform.position = secondPos;
                ballShootLineRenderer2.SetPosition(0, firstBoundCollider.centroid);
                ballShootLineRenderer2.SetPosition(1, ballPreviewTransform.position - secondShootDirection * 1.3f);
            }
            else
            {
                ballPreviewTransform.position = firstBoundCollider.centroid;
            }
        };
        

        ballShootInputListener.OnEndDragAction += () =>
        {
            ballShootLineRenderer.SetPosition(0, Vector3.zero);
            ballShootLineRenderer.SetPosition(1, Vector3.zero);

            ballShootLineRenderer2.SetPosition(0, Vector3.zero);
            ballShootLineRenderer2.SetPosition(1, Vector3.zero);


            totalBallPos = Vector3.zero;
            firstClickPos = Vector3.zero;


            ballPreviewTransform.gameObject.SetActive(false);
            arrowTransform.gameObject.SetActive(false);

            if(shootDirectionLength >= 1f)
            {
                //��
                StartCoroutine(ShootBalls());

                ballShootInputListener.SetInputActive(false);
            }
        };

       
        if(DataManager.IsContinuePlay)
        {
            var playData = PlayMapDataManager.GetData();
            totalBallPos = playData.totalBallPos;
            startBall.transform.position = totalBallPos;
            currentBallCount = playData.ballCount;
            round = playData.round;
            hasResurrected = playData.hasResurrected;
            ScoreManager.AddScore(playData.score);


            var commonBlockData = playData.commonBlockData;
            for (int i = 0; i < commonBlockData.Length; i++)
            {
                var commonBlock = PoolManager.GetCommonBlock();
                commonBlock.SetPosY(commonBlockData[i].posY - 1);
                commonBlock.SetNumber(commonBlockData[i].leftCount);
                commonBlock.transform.position = commonBlockData[i].position + Vector3.up * Constants.BlockColumnSize;
                commonBlock.SetDamageCount(commonBlockData[i].damageCount);
                commonBlock.SetAddScore(commonBlockData[i].addScore);

                commonBlocks.Add(commonBlock);
                moves.Add(commonBlock);
                commonBlock.gameObject.SetActive(true);
            }
            var iceBlockData = playData.iceBlockData;
            for (int i = 0; i < iceBlockData.Length; i++)
            {
                var iceBlock = PoolManager.GetIceBlock();
                iceBlock.SetPosY(iceBlockData[i].posY - 1);
                iceBlock.transform.position = iceBlockData[i].position + Vector3.up * Constants.BlockColumnSize;
                iceBlocks.Add(iceBlock);
                moves.Add(iceBlock);
                iceBlock.gameObject.SetActive(true);
            }
            //�� ������
            foreach (var moveObj in moves)
            {
                moveObj.MoveToBottom();
            }
            CoroutineExecuter.ExcuteAfterWaitTime(() => 
            {
                MagicBlock.ChangeTimePeriod();
                ballShootInputListener.SetInputActive(true);
            }, 0.7f);
        }
        else
        {            
            //1�� �ڿ� �������� ��
            CoroutineExecuter.ExcuteAfterWaitTime(() => 
            {
                CreateBlocks();
                MagicBlock.ChangeTimePeriod();
            }, 0.7f);
        }


        OnBallCountChange(currentBallCount);
    }

    
    private void OnBallCountChange(int _Count)
    {
        ballCountText.text = "x " + _Count;
    }

    public void OnResurrect()
    {
        foreach (var commonBlock in commonBlocks)
        {
            if (commonBlock.gameObject.activeSelf)
            {
                commonBlock.Suicide();
            }
        }

        foreach (var iceBlock in iceBlocks)
        {
            if (iceBlock.gameObject.activeSelf)
            {
                iceBlock.Suicide();
            }
        }
        ballShootInputListener.SetInputActive(true);
        hasResurrected = true;


        SaveData();
    }

    public void SetTotalBallPos(Vector3 _Pos)
    {
        if (totalBallPos == Vector3.zero)
        {
            _Pos.x = Mathf.Clamp(_Pos.x, Constants.ClampX * -1f, Constants.ClampX);
            totalBallPos = _Pos;
        }
    }

    public void AddBall()
    {
        ++currentBallCount;
        OnBallCountChange(currentBallCount);
    }

    private void CreateBlocks()
    {
        StartCoroutine(CreateBlocksRoutine());
        IEnumerator CreateBlocksRoutine()
        {
            int count = MJ.MyUtil.GetBlockCount(round);            
           

            List<Vector3> spawnPoses = new List<Vector3>();
            for (int i = 0; i < spawnTransforms.Length; i++)
            {
                spawnPoses.Add(spawnTransforms[i].position);
            }

            List<Block> blocks = new List<Block>();
            for (int i = 0; i < count; i++)
            {
                int rand = Random.Range(0, spawnPoses.Count);
                var block = PoolManager.GetCommonBlock();
                block.transform.position = spawnPoses[rand];
                block.SetNumber(round);
                block.gameObject.SetActive(true);
              
                commonBlocks.Add(block);
                //üũ�뵵
                blocks.Add(block);
                //�ϴ� ��
                moves.Add(block);
                //������
                block.OnCreateBlock();

                spawnPoses.RemoveAt(rand);
            }

            if (Constants.MaxRound > round)
            {
                var iceBlock = PoolManager.GetIceBlock();
                iceBlock.gameObject.SetActive(true);
                iceBlock.transform.position = spawnPoses[Random.Range(0, spawnPoses.Count)];
                iceBlock.OnCreateBlock();

                iceBlocks.Add(iceBlock);
                blocks.Add(iceBlock);
                moves.Add(iceBlock);
            }

            Func<bool> check = () =>
            {
                foreach (var block in blocks)
                {
                    if (!block.IsScaleChangeDone)
                    {
                        return false;
                    }
                }
                return true;
            };
            yield return new WaitUntil(check); //�� �����ö����� ��ٸ�
     
            //���� ������
            foreach (var movableObj in moves)
            {
                if (movableObj.gameObject.activeSelf)
                {
                    movableObj.MoveToBottom();
                }
            }

       



            //�� �����Դ��� üũ��
            StartCoroutine(CheckAllMovableObjMovedone());
        }
    }

    IEnumerator CheckAllMovableObjMovedone()
    {
        Func<bool> check = () =>
        {
            foreach (var moveObj in moves)
            {
                if(!moveObj.gameObject.activeSelf)
                {
                    continue;
                }

                if (moveObj.IsMoving)
                {
                    return false;
                }
            }            
            return true;
        };

        yield return new WaitUntil(check);

        ballShootInputListener.SetInputActive(true);
        //�� �� �������� ��������
        SaveData();


        OnBlockCreateDone?.Invoke();
    }

    private IEnumerator ShootBalls()
    {
        //�� �� ����
        OnStartShootBall?.Invoke();

        if (OptionManager.IsTwoBoundItemOptionOn)
        {
            --DataManager.TwoBoundItemCount;
        }
        if(OptionManager.IsSpeedUpItemOptionOn)
        {
            Time.timeScale = Constants.SpeedUpAmount;
            --DataManager.SpeedUpItemCount;
        }

        int shootBallCount;
        if(OptionManager.IsPowerUpItemOptionOn)
        {
            var ballCountAndDamage = GetBallCountAndDamage(currentBallCount);
            DataManager.BallDamage = ballCountAndDamage.Damage;
            shootBallCount = ballCountAndDamage.Count;
            --DataManager.PowerUpItemCount;
        }
        else
        {
            DataManager.BallDamage = 1;
            shootBallCount = currentBallCount;
        }

        balls.Add(startBall);
        for (int i = 2; i <= shootBallCount; i++)
        {
            var ball = PoolManager.GetBall();
            ball.gameObject.SetActive(true);
            balls.Add(ball);
        }
        //�� �ְ� ��
        for (int i = 1; i < balls.Count; i++)
        {
            balls[i].gameObject.SetActive(false);
        }



        var startPos = startBall.transform.position;

        foreach (var ball in balls)
        {
            ball.gameObject.SetActive(true);
            ball.transform.position = startPos;
            ball.Shoot(shootDirection);

            //3�� ��ٸ�
            yield return YieldContainer.WaitForFixedUpdate;
            yield return YieldContainer.WaitForFixedUpdate;
            yield return YieldContainer.WaitForFixedUpdate;
        }

        StartCoroutine(CheckAllBallsMoving());
    }

    private IEnumerator CheckAllBallsMoving()
    {
        Func<bool> checkBalls = () =>
        {
            foreach (var ball in balls)
            {
                if (ball.IsMoving)
                {
                    return false;
                }
            }
            return true;
        };
        yield return new WaitUntil(checkBalls);
        Time.timeScale = 1f;

        ++round;
        if(round > Constants.MaxRound)
        {
            //�ִ� �̰� �� ����
            round = Constants.MaxRound;
        }


        //ù ��° ���� ���� �� ��
        for (int i = 1; i < balls.Count; i++)
        {
            balls[i].gameObject.SetActive(false);
        }
        balls.Clear();

        //Ȥ�ó� ĳ�� �� �� ��� �̰ɷ� ����
        if (totalBallPos == Vector3.zero)
        {
            totalBallPos = startBall.transform.position;
        }


        //�� ������ ���� �κ�
        CreateBlocks();
    }


    private BallCountAndDamage GetBallCountAndDamage(int _CurBallCount)
    {
        var result = new BallCountAndDamage();
        int maxBallCount = 20;

        if (_CurBallCount == 1)
        {
            result.Count = 1;
            result.Damage = 1;
        }
        else
        {
            var damage = 2 + (_CurBallCount / maxBallCount);
            var count = (_CurBallCount / damage) + 1;
            result.Count = count;
            result.Damage = damage;
        }

        return result;
    }

    private void SaveData()
    {
        PlayData playMapData = new PlayData();
        playMapData.ballCount = currentBallCount;
        playMapData.totalBallPos = totalBallPos;
        playMapData.round = round;
        playMapData.score = ScoreManager.CurScore;
        playMapData.hasResurrected = hasResurrected;

        List<CommonBlockData> commonBlocksData = new List<CommonBlockData>();
        foreach (var commonBlock in commonBlocks)
        {
            if (commonBlock.gameObject.activeSelf)
            {
                var pos = commonBlock.transform.position;
                var posY = commonBlock.CurrentPosY;
                var count = commonBlock.Count;
                var hasTakeDamaged = commonBlock.CurrentDamageCount;
                var addScore = commonBlock.AddScore;
                commonBlocksData.Add(new CommonBlockData(pos, posY, count, hasTakeDamaged, addScore));
            }
        }
     

        List<IceBlockData> iceBlocksData = new List<IceBlockData>();
        foreach (var iceBlock in iceBlocks)
        {
            if (iceBlock.gameObject.activeSelf)
            {
                var pos = iceBlock.transform.position;
                var posY = iceBlock.CurrentPosY;
                iceBlocksData.Add(new IceBlockData(pos, posY));
            }
        }
                     

        playMapData.commonBlockData = commonBlocksData.ToArray();
        playMapData.iceBlockData = iceBlocksData.ToArray();

        PlayMapDataManager.SaveData(playMapData);
    }
}
