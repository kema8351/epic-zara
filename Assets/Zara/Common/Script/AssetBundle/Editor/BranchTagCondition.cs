using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zara.Common.Utility;

namespace Zara.Common.ExAssetBundle.Editor
{
    public class BranchTagCondition
    {
        public string Tag { get; }
        public string PartFileName { get; }

        public BranchTagCondition(string tag, string partFileName)
        {
            this.Tag = tag;
            this.PartFileName = partFileName;
        }
    }
}