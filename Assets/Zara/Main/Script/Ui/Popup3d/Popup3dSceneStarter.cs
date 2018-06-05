﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zara.Common.ExScene;
using Zara.Common.ExBase;
using Zara.Common.Menu;
using Zara.Expansion.Ui;

namespace Zara.Main.Ui.Popup3d
{
    public class Popup3dSceneStarter : PopupUiSceneStarter<Popup3dSceneStarter>
    {
        [AutoGet]
        [SerializeField]
        Popup3dUi ui;

        protected override IPopupUi PopupUi => ui;
        protected override bool Has3dObject => true;
    }
}