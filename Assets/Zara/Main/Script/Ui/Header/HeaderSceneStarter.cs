using System.Collections;
using UnityEngine;
using Zara.Common.ExBase;
using Zara.Common.Ui;
using Zara.Expansion.Ui;

namespace Zara.Main.Ui.Header
{
    public class HeaderSceneStarter : ResidentUiSceneStarter<HeaderSceneStarter>
    {
        [AutoGet]
        [SerializeField]
        HeaderUi ui;
        public HeaderUi Ui => ui;

        protected override IResidentUi ResidentUi => ui;

        protected override int OrderInStratum => 1;
    }
}