using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zara.Common.ExScene;
using Zara.Common.ExBase;
using Zara.Common.Menu;
using Zara.Expansion.Ui;

namespace Zara.Main.Ui.Download
{
    public class DownloadSceneStarter : CoreUiSceneStarter<DownloadSceneStarter>
    {
        [AutoGet]
        [SerializeField]
        DownloadUi ui;
        public DownloadUi Ui => ui;

        protected override ICoreUi CoreUi => ui;
    }
}