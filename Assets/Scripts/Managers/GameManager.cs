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
    [SerializeField] private Ball startBall; //제일 처음 주는 공

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI ballCountText;

    public event Action OnStartShootBall; //공 쏘기 시작
    public event Action OnBlockCreateDone; //블럭이 다 만들어지고 난 후 실행되는 액션


    public static GameManager Instance => instance;
    private static GameManager instance;
    private const float circleRaycaseRadius = 1.3f;

    private int round;
    private Vector3 firstClickPos;
    private Vector3 secondClickPos;
    private Vector3 shootDirection;
    private float shootDirectionLength;
    private Vector3 totalBallPos;
    public Vector3 TotalBallPos => totalBallPos;


    private List<Ball> balls;
    private HashSet<CommonBlock> commonBlocks;
    private HashSet<IceBlock> iceBlocks;
    private HashSet<Movable> moves;

    private int currentBallCount;

    private void Awake()
    {     
        PoolManager.Init();
        ScoreManager.Init();
   
        instance = this;
        round = 1;
        currentBallCount = 1;

        totalBallPos = new Vector3(0f, Constants.BottomY, 0f);
        balls = new List<Ball>();
        //balls.Add(startBall);

        commonBlocks = new HashSet<CommonBlock>();
        iceBlocks = new HashSet<IceBlock>();
        moves = new HashSet<Movable>();
    }

    private void OnDisable()
    {
        //게임 끝나면 끔
        IceBlock.StopChangeTimePeriod();    
    }

    private void Start()
    {
        ballShootInputListener.OnBeginDragAction += _Position =>
        {
            firstClickPos = Camera.main.ScreenToWorldPoint(_Position) + Vector3.forward * 10f;
            shootDirectionLength = 0f;

            arrowTransform.gameObject.SetActive(true);

            //에이밍 옵션 켜져있으면 보여줌
            ballPreviewTransform.gameObject.SetActive(OptionManager.IsAiming);
        };

        ballShootInputListener.OnDragAction += _Position =>
        {
            secondClickPos = Camera.main.ScreenToWorldPoint(_Position) + Vector3.forward * 10f;

            shootDirection = secondClickPos - firstClickPos;
            shootDirectionLength = shootDirection.magnitude;

            shootDirection.Normalize();
            shootDirection = new Vector3(shootDirection.y >= 0 ? shootDirection.x : (shootDirection.x >= 0 ? 1 : -1), Mathf.Clamp(shootDirection.y, 0.2f, 1), 0f);

            
            arrowTransform.position = startBall.transform.position; //원래 totalBallPos인데 에러 있어서
            arrowTransform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg);
            
            //에임 옵션 끈 상태면 안 보여줌
            if(!OptionManager.IsAiming)
            {
                return;
            }



            //2바운드 인지
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
                //쏨
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
            ScoreManager.AddScore(playData.score);


            var commonBlockData = playData.commonBlockData;
            for (int i = 0; i < commonBlockData.Length; i++)
            {
                var commonBlock = PoolManager.GetCommonBlock();
                commonBlock.SetPosY(commonBlockData[i].posY - 1);
                commonBlock.SetNumber(commonBlockData[i].leftCount);
                commonBlock.transform.position = commonBlockData[i].position + Vector3.up * Constants.BlockColumnSize;
                commonBlock.SetDamageCount(commonBlockData[i].damageCount);

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
            //다 내려줌
            foreach (var moveObj in moves)
            {
                moveObj.MoveToBottom();
            }
            CoroutineExecuter.ExcuteAfterWaitTime(() => 
            {
                IceBlock.ChangeTimePeriod();
                ballShootInputListener.SetInputActive(true);
            }, 0.7f);
        }
        else
        {            
            //1초 뒤에 블럭나오게 함
            CoroutineExecuter.ExcuteAfterWaitTime(() => 
            {
                CreateBlocks();
                IceBlock.ChangeTimePeriod();
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
            int count;
            int randBlock = Random.Range(0, 24);
            if (round <= 10)
            {
                count = randBlock < 16 ? 1 : 2;
            }
            else if (round <= 20)
            {
                count = randBlock < 8 ? 1 : (randBlock < 6 ? 2 : 3);
            }
            else if (round <= 40)
            {
                count = randBlock < 9 ? 2 : (randBlock < 18 ? 3 : 4);
            }
            else
            {
                count = randBlock < 8 ? 2 : randBlock < 16 ? 3 : (randBlock < 20 ? 4 : 5);
            }

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
                //체크용도
                blocks.Add(block);
                //일단 뺌
                moves.Add(block);
                //내려줌
                block.OnCreateBlock();

                spawnPoses.RemoveAt(rand);
            }

            var iceBlock = PoolManager.GetIceBlock();
            iceBlock.gameObject.SetActive(true);
            iceBlock.transform.position = spawnPoses[Random.Range(0, spawnPoses.Count)];
            iceBlock.OnCreateBlock();
            if (!iceBlocks.Contains(iceBlock))
            {
                iceBlocks.Add(iceBlock);
            }
            //나중에 넣어줌
            blocks.Add(iceBlock);
            moves.Add(iceBlock);


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
            yield return new WaitUntil(check); //다 내려올때까지 기다림

            //블럭들 내려줌
            foreach (var movableObj in moves)
            {
                if (movableObj.gameObject.activeSelf)
                {
                    movableObj.MoveToBottom();
                }
            }

       



            //다 내려왔는지 체크함
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
        //블럭 다 내려오고 저장해줌
        SaveData();


        OnBlockCreateDone?.Invoke();
    }

    private IEnumerator ShootBalls()
    {
        //공 쏠때 실행
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
        //다 넣고 끔
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

            //3번 기다림
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
        //첫 번째 빼고 전부 다 끔
        for (int i = 1; i < balls.Count; i++)
        {
            balls[i].gameObject.SetActive(false);
        }
        balls.Clear();
        //다 끝나고 여기 부분
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

        List<CommonBlockData> commonBlocksData = new List<CommonBlockData>();
        foreach (var commonBlock in commonBlocks)
        {
            if (commonBlock.gameObject.activeSelf)
            {
                var pos = commonBlock.transform.position;
                var posY = commonBlock.CurrentPosY;
                var count = commonBlock.Count;
                var hasTakeDamaged = commonBlock.CurrentDamageCount;
                commonBlocksData.Add(new CommonBlockData(pos, posY, count, hasTakeDamaged));
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
