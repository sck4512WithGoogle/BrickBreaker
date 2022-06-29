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
                }
                return init;
            }
        }
        private static Behaviour init;



        public void Excute(IEnumerator _Coroutine)
        {
            StartCoroutine(_Coroutine);
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

    public static void Excute(IEnumerator _Coroutine)
    {
        Behaviour.Init.Excute(_Coroutine);
    }

    public static void ExcuteAfterWaitTime(System.Action _Action, float _WaitTime)
    {
        Behaviour.Init.ExcuteAfterWaitTime(_Action, _WaitTime);
    }
}
