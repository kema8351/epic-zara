using System.Collections.Generic;
using Zara.Common.ExCamera;

namespace Zara.Expansion.ExCamera
{
    public static class ConstCamera
    {
        public const int ThreeDimension = 1;
        public const int Ui = 2;
        public const int TouchEffect = 9;

        public static IEnumerable<StudioSetting> EnumerateStudios()
        {
            yield return new StudioSetting(ConstCamera.ThreeDimension, 1, 100);
            yield return new StudioSetting(ConstCamera.Ui, 1, 100);
            yield return new StudioSetting(ConstCamera.TouchEffect, 1, 10);
        }
    }
}