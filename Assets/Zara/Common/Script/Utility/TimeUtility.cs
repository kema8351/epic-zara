using UnityEngine;

namespace Zara.Common.Utility
{
    public static class TimeUtility
    {
        const float DropFrameThresholdSeconds = 1f / 60f;

        public static bool DropFrameExists()
        {
            return Time.unscaledTime - Time.realtimeSinceStartup > DropFrameThresholdSeconds;
        }
    }
}