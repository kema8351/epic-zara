using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zara.Common.Ui;

namespace Zara.Expansion.Ui
{
    public static class ConstStratum
    {
        public const int Background = 1;
        public const int Core = 2;
        public const int Resident = 3;
        public const int Popup = 4;
        public const int Overlay = 5;
        public const int Frontmost = 9;

        public static IEnumerable<StratumSetting> EnumerateStrata()
        {
            yield return new StratumSetting(ConstStratum.Background, 100, 10);
            yield return new StratumSetting(ConstStratum.Core, 100, 10);
            yield return new StratumSetting(ConstStratum.Resident, 100, 20);
            yield return new StratumSetting(ConstStratum.Popup, 100, 20);
            yield return new StratumSetting(ConstStratum.Overlay, 100, 10);
            yield return new StratumSetting(ConstStratum.Frontmost, 100, 10);
        }
    }
}