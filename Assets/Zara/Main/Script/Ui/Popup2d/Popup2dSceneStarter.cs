using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zara.Common.ExScene;
using Zara.Common.ExBase;
using Zara.Common.Menu;
using Zara.Expansion.Ui;

namespace Zara.Main.Ui.Popup2d
{
    public class Popup2dSceneStarter : PopupUiSceneStarter<Popup2dSceneStarter>
    {
        [AutoGet]
        [SerializeField]
        Popup2dUi ui;

        protected override IPopupUi PopupUi => ui;
    }
}