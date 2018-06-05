using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zara.Common.ExScene;
using Zara.Common.ExBase;
using Zara.Common.Menu;
using Zara.Expansion.Ui;

namespace Zara.Main.Ui.First
{
    public class FirstSceneStarter : CoreUiSceneStarter<FirstSceneStarter>
    {
        [AutoGet]
        [SerializeField]
        FirstUi ui;

        protected override ICoreUi CoreUi => ui;
    }
}