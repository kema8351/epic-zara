using UnityEngine;
using Zara.Common.ExBase;
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