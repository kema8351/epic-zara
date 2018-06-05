using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zara.Common.ExScene;
using Zara.Common.ExBase;
using Zara.Common.Menu;
using Zara.Expansion.Ui;

namespace Zara.Main.Ui.Title
{
    public class TitleSceneStarter : CoreUiSceneStarter<TitleSceneStarter>
    {
        [AutoGet]
        [SerializeField]
        TitleUi ui;
        public TitleUi Ui => ui;

        protected override ICoreUi CoreUi => ui;
    }
}