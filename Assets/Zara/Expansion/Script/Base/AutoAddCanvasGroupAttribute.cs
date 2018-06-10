using UnityEngine;
using Zara.Common.ExBase;
using Zara.Expansion.Utility;

namespace Zara.Expansion.ExBase
{
    public class AutoAddCanvasGroupAttribute : AutoAddAttribute
    {
#if UNITY_EDITOR
        public override void SetDefault(object obj)
        {
            CanvasGroup canvasGroup = Cast<CanvasGroup>(obj);

            if (canvasGroup != null)
                DefaultSettingUtility.SetCanvasGroup(canvasGroup);
        }
#endif
    }
}