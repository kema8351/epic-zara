using UnityEngine;
using Zara.Common.ExBase;
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