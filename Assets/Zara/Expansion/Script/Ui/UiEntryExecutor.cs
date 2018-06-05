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
    public abstract class UiEntryExecutor : ExMonoBehaviour
    {
        public abstract IEnumerator Entry(bool shouldShowImmediately);
        public abstract void SkipToEntried();
    }
}