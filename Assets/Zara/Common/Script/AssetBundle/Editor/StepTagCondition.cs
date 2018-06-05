using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zara.Common.Utility;

namespace Zara.Common.ExAssetBundle.Editor
{
    public class StepTagCondition
    {
        public string Tag { get; }
        public string PartFileName { get; }
        public int Step { get; }

        public StepTagCondition(string tag, string partFileName, int step)
        {
            this.Tag = tag;
            this.PartFileName = partFileName;
            this.Step = step;
        }
    }
}