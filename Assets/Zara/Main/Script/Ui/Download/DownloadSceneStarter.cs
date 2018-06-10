using UnityEngine;
using Zara.Common.ExBase;
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