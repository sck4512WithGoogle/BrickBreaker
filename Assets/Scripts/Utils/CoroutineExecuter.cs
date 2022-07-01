using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineExecuter : MonoBehaviour
{
    class Behaviour : MonoBehaviour
    {
        public static Behaviour Init
        {
            get
            {
                if (init == null)
                {
                    init = new GameObject().AddComponent<Behaviour>();
                    DontDestroyOnLoad(init.gameObject);
                }
                return init;
            }
        }
        private static Behaviour init;



        public Coroutine Excute(IEnumerator _Coroutine)
        {
            return StartCoroutine(_Coroutine);
        }

        public void MyStopCoroutine(Coroutine _Coroutine)
        {
            StopCoroutine(_Coroutine);
        }

        public void ExcuteAfterWaitTime(System.Action _Action, float _WaitTime)
        {
            StartCoroutine(ExcuteAfterWaitTimeRoutine(_Action, _WaitTime));
            IEnumerator ExcuteAfterWaitTimeRoutine(System.Action _Action, float _WaitTime)
            {
                yield return MJ.Manager.YieldContainer.GetWaitForSeconds(_WaitTime);
                _Action.Invoke();
            }
        }
    }

    public static Coroutine Excute(IEnumerator _Coroutine)
    {
        return Behaviour.Init.Excute(_Coroutine);
    }

    public static void MyStopCoroutine(Coroutine _Coroutine)
    {
        Behaviour.Init.MyStopCoroutine(_Coroutine);
    }

    public static void ExcuteAfterWaitTime(System.Action _Action, float _WaitTime)
    {
        Behaviour.Init.ExcuteAfterWaitTime(_Action, _WaitTime);
    }
}
