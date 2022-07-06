
namespace MJ.Manager
{
    using System.Collections.Generic;
    using UnityEngine;

    public enum ParticleType
    {
        Red, Blue, Green
    }
    public class PoolManager : MonoBehaviour
    {
        private static GameObject blockPrefab;
        private static GameObject iceBlockPrefab;
        private static GameObject ballPrefab;

        private static List<CommonBlock> blocks;
        private static List<MagicBlock> iceBlocks;
        private static List<Ball> balls;


        private static GameObject[] numberPrefabs;
        private static Dictionary<int, List<GameObject>> numbers;


        private static GameObject blockBreakEffectPrefab;
        private static GameObject iceBlockBreakEffectPrefab;
        private static List<GameObject> blockBreakEffects;
        private static List<GameObject> iceBlockBreakEffects;



        private static GameObject speakerPrefab;
        private static List<AudioSource> speakers;


        private static GameObject numberTextPrefab;
        private static List<NumberText> numberTexts;

        static PoolManager()
        {
            blockPrefab = Resources.Load<GameObject>("Prefabs/Block");
            blocks = new List<CommonBlock>();

            iceBlockPrefab = Resources.Load<GameObject>("Prefabs/IceBlock");
            iceBlocks = new List<MagicBlock>();

            ballPrefab = Resources.Load<GameObject>("Prefabs/Ball");
            balls = new List<Ball>();

   
            numbers = new Dictionary<int, List<GameObject>>();
            numberPrefabs = new GameObject[10];
            for (int i = 0; i <= 9; i++)
            {
                numberPrefabs[i] = Resources.Load<GameObject>($"Prefabs/Number/{i}");
                numbers.Add(i, new List<GameObject>());
            }


            blockBreakEffectPrefab = Resources.Load<GameObject>("Prefabs/BlockBreakEffect");
            iceBlockBreakEffectPrefab = Resources.Load<GameObject>("Prefabs/IceBlockBreakEffect");
            blockBreakEffects = new List<GameObject>();
            iceBlockBreakEffects = new List<GameObject>();


            speakerPrefab = Resources.Load<GameObject>("Prefabs/Speaker");
            speakers = new List<AudioSource>();



            numberTextPrefab = Resources.Load<GameObject>("Prefabs/NumberText");
            numberTexts = new List<NumberText>();
        }

        public static void Init()
        {
            balls.Clear();
            blocks.Clear();
            iceBlocks.Clear();

            blockBreakEffects.Clear();
            iceBlockBreakEffects.Clear();
            for (int i = 0; i < numbers.Count; i++)
            {
                numbers[i].Clear();
            }

            speakers.Clear();
            //4�� ������
            for (int i = 0; i < 4; i++)
            {
                var newSpeaker = Instantiate(speakerPrefab).GetComponent<AudioSource>();
                newSpeaker.playOnAwake = false;
                newSpeaker.loop = false;
                speakers.Add(newSpeaker);
            }

            numberTexts.Clear();
        }

        public static NumberText GetNumberText()
        {
            foreach (var numberText in numberTexts)
            {
                if(!numberText.gameObject.activeSelf)
                {
                    return numberText;
                }
            }
            var newNumberText = Instantiate(numberTextPrefab).GetComponent<NumberText>();
            numberTexts.Add(newNumberText);
            return newNumberText;
        }


        public static AudioSource GetSpeaker()
        {
            foreach (var speaker in speakers)
            {
                if(!speaker.isPlaying)
                {
                    return speaker;
                }
            }

            var newSpeaker = Instantiate(speakerPrefab).GetComponent<AudioSource>();
            newSpeaker.playOnAwake = false;
            newSpeaker.loop = false;
            speakers.Add(newSpeaker);
            return newSpeaker;
        }

        public static CommonBlock GetCommonBlock()
        {
            foreach (var block in blocks)
            {
                if (!block.gameObject.activeSelf)
                {
                    return block;
                }
            }

            var blockComponent = Instantiate(blockPrefab).GetComponent<CommonBlock>();
            blocks.Add(blockComponent);
            return blockComponent;
        }

        public static Ball GetBall()
        {
            foreach (var ball in balls)
            {
                if (!ball.gameObject.activeSelf)
                {
                    return ball;
                }
            }

            var ballComponent = Instantiate(ballPrefab).GetComponent<Ball>();
            balls.Add(ballComponent);
            return ballComponent;
        }


        public static MagicBlock GetIceBlock()
        {
            foreach (var iceBlock in iceBlocks)
            {
                if (!iceBlock.gameObject.activeSelf)
                {
                    return iceBlock;
                }
            }

            var iceBlockObj = Instantiate(iceBlockPrefab).GetComponent<MagicBlock>();
            iceBlocks.Add(iceBlockObj);
            return iceBlockObj;
        }

        public static GameObject GetNumber(int _Number)
        {
            int number = Mathf.Clamp(_Number, 0, 9);

            var list = numbers[number];
            var numberPrefab = numberPrefabs[number];
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].activeSelf)
                {
                    return list[i];
                }
            }
            var numberObj = Instantiate(numberPrefab);
            list.Add(numberObj);
            return numberObj;
        }


        public static GameObject GetBlockBreakEffect()
        {
            for (int i = 0; i < blockBreakEffects.Count; i++)
            {
                if(!blockBreakEffects[i].activeSelf)
                {
                    return blockBreakEffects[i];
                }
            }

            var blockEffect = Instantiate(blockBreakEffectPrefab);
            blockBreakEffects.Add(blockEffect);
            return blockEffect;
        }



        public static GameObject GetIceBlockBreakEffect()
        {
            for (int i = 0; i < iceBlockBreakEffects.Count; i++)
            {
                if (!iceBlockBreakEffects[i].activeSelf)
                {
                    return iceBlockBreakEffects[i];
                }
            }

            var iceBlockEffect = Instantiate(iceBlockBreakEffectPrefab);
            iceBlockBreakEffects.Add(iceBlockEffect);
            return iceBlockEffect;
        }
    }


}
