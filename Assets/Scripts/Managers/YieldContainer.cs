

namespace MJ.Manager
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class YieldContainer
    {
        private static Dictionary<float, WaitForSeconds> waitForSeconds;
        private static Dictionary<float, WaitForSecondsRealtime> waitForSecondRealTimes;
        static YieldContainer()
        {
            waitForSeconds = new Dictionary<float, WaitForSeconds>();
            waitForSecondRealTimes = new Dictionary<float, WaitForSecondsRealtime>();
        }

        public static WaitForSeconds GetWaitForSeconds(float _WaitTime)
        {
            if(!waitForSeconds.ContainsKey(_WaitTime))
            {
                waitForSeconds.Add(_WaitTime ,new WaitForSeconds(_WaitTime));
            }
            return waitForSeconds[_WaitTime];
        }

        public static WaitForSecondsRealtime GetWaitForSecondsRealtime(float _WaitTime)
        {
            if (!waitForSecondRealTimes.ContainsKey(_WaitTime))
            {
                waitForSecondRealTimes.Add(_WaitTime, new WaitForSecondsRealtime(_WaitTime));
            }
            return waitForSecondRealTimes[_WaitTime];
        }
    }
}
