using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Zara.Common;
using Zara.Common.ExScene;
using Zara.Common.ExBase;
using Zara.Common.Menu;
using Zara.Common.Ui;
using Zara.Common.Utility;

namespace Zara.Expansion.Ui
{
    public abstract class UiExitExecutor : ExMonoBehaviour
    {
        public abstract IEnumerator Exit(bool shouldHideImmediately);
        public abstract void SkipToHidden();
    }
}