using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zara.Common.ExBase;
using Zara.Common.Utility;

namespace Zara.Test
{
    public class Title : ExMonoBehaviour
    {
        [SerializeField]
        Toggle toggleEn = null;

        [SerializeField]
        Toggle toggleJp = null;

        [SerializeField]
        Button startButton = null;

        [SerializeField]
        Button clearButton = null;

        AssetLanguage language = AssetLanguage.English;

        public void Init(Action<AssetLanguage> startAction, Action clearAction)
        {
            toggleEn.onValueChanged.RemoveAllListeners();
            toggleEn.onValueChanged.AddListener(b => { if (b) language = AssetLanguage.English; });
            toggleJp.onValueChanged.RemoveAllListeners();
            toggleJp.onValueChanged.AddListener(b => { if (b) language = AssetLanguage.Japanese; });
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(() => startAction?.Invoke(language));
            clearButton.onClick.RemoveAllListeners();
            clearButton.onClick.AddListener(() => clearAction?.Invoke());
        }
    }
}