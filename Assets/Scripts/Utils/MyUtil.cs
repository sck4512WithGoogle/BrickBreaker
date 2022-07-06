
namespace MJ
{
    using MJ.Data;
    using UnityEngine;
    public class MyUtil
    {
        public static Vector3 GetReflectionVector(Vector3 _IncidenceVector, Vector3 _Normal)
        {
            var dot = Vector3.Dot(_IncidenceVector, _Normal);
            return _IncidenceVector - 2 * _Normal * dot;
        }


        public static int GetBlockCount(int _Round)
        {
            int count;
            float randomValue = Random.Range(0f, 100f);
            if (_Round <= 10)
            {
                if(randomValue < 40f)
                {
                    count = 1;
                }
                else
                {
                    count = 2;
                }
                //count = randBlock < 16 ? 1 : 2;
            }
            else if (_Round <= 20)
            {
                if (randomValue < 40f)
                {
                    count = 1;
                }
                else if(randomValue < 70f)
                {
                    count = 2;
                }
                else
                {
                    count = 3;
                }


                //count = randBlock < 8 ? 1 : (randBlock < 6 ? 2 : 3);
            }
            else if (_Round <= 40)
            {
                if(randomValue < 37.5f)
                {
                    count = 2;
                }
                else if(randomValue < 75f)
                {
                    count = 3;
                }
                else
                {
                    count = 4;
                }


                //count = randBlock < 9 ? 2 : (randBlock < 18 ? 3 : 4);
            }
            else if(_Round <= 100)
            {
                if(randomValue < 40f)
                {
                    count = 2;
                }
                else if(randomValue < 70f)
                {
                    count = 3;
                }
                else if(randomValue < 90f)
                {
                    count = 4;
                }
                else
                {
                    count = 5;
                }


                //count = randBlock < 8 ? 2 : randBlock < 16 ? 3 : (randBlock < 20 ? 4 : 5);
            }
            else if(_Round < Constants.MaxRound)
            {
                if (randomValue < 30f)
                {
                    count = 2;
                }
                else if (randomValue < 63f)
                {
                    count = 3;
                }
                else if (randomValue < 88f)
                {
                    count = 4;
                }
                else
                {
                    count = 5;
                }
            }
            else
            {
                //맥스 라운드 초과일 경우
                if (randomValue < 10f)
                {
                    count = 2;
                }
                else if (randomValue < 45f)
                {
                    count = 3;
                }
                else if (randomValue < 84f)
                {
                    count = 4;
                }
                else
                {
                    count = 5;
                }
            }

            return count;
        }
    }
}