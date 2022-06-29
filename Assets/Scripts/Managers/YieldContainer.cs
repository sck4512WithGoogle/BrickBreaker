

namespace MJ.Manager
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class YieldContainer
    {
        private static Dictionary<float, WaitForSeconds> waitSeconds;

        static YieldContainer()
        {
            waitSeconds = new Dictionary<float, WaitForSeconds>();
        }

        public static WaitForSeconds GetWaitForSeconds(float _WaitTime)
        {
            if(!waitSeconds.ContainsKey(_WaitTime))
            {
                waitSeconds.Add(_WaitTime ,new WaitForSeconds(_WaitTime));
            }
            return waitSeconds[_WaitTime];
        }
    }
}
