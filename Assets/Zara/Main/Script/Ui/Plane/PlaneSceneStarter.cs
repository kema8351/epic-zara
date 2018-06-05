using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zara.Common.ExScene;
using Zara.Common.ExBase;
using Zara.Common.Menu;
using Zara.Expansion.Ui;

namespace Zara.Main.Ui.Plane
{
    public class PlaneSceneStarter : BackgroundUiSceneStarter<PlaneSceneStarter>
    {
        [AutoGet]
        [SerializeField]
        PlaneUi ui;

        protected override IBackgroundUi BackgroundUi => ui;
    }
}