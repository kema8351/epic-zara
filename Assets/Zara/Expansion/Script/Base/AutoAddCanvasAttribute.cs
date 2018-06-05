using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zara.Common.ExBase;
using Zara.Expansion.Utility;

namespace Zara.Expansion.ExBase
{
    public class AutoAddCanvasAttribute : AutoAddAttribute
    {
#if UNITY_EDITOR
        public override void SetDefault(object obj)
        {
            Canvas canvas = Cast<Canvas>(obj);

            if (canvas != null)
                DefaultSettingUtility.SetCanvas(canvas);
        }
#endif
    }
}