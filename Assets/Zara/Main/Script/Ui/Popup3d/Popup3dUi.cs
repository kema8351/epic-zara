using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zara.Common.ExScene;
using Zara.Expansion.Ui;
using Zara.Main.Ui.Popup2d;

namespace Zara.Main.Ui.Popup3d
{
    public class Popup3dUi : PopupUi
    {
        [SerializeField] PopupButtonSet buttonSetPrefab;
        [SerializeField] Transform cubeTransform;

        protected override IEnumerator OnUiStarted()
        {
            cubeTransform.localPosition = new Vector3(Random.Range(-800f, 800f), Random.Range(-600f, 600f), 0f);
            cubeTransform.localScale = new Vector3(Random.Range(100f, 500f), Random.Range(100f, 500f), Random.Range(100f, 500f));
            cubeTransform.localRotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));

            PopupButtonSet buttonSet = Instantiate(buttonSetPrefab, this.transform, false);
            buttonSet.Init(() => Exit(true));

            yield break;
        }

        protected override IEnumerator OnUiEntrying() { yield break; }
        protected override IEnumerator OnUiEntried() { yield break; }
        protected override IEnumerator OnUiExiting() { yield break; }
        protected override IEnumerator OnUiExited() { yield break; }
    }
}