using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zara.Expansion.Ui;

namespace Zara.Main.Ui.Popup2d
{
    public class Popup2dUi : PopupUi
    {
        [SerializeField] PopupButtonSet buttonSetPrefab;
        [SerializeField] Image image;

        protected override IEnumerator OnUiStarted()
        {
            image.transform.localPosition = new Vector2(Random.Range(-800f, 800f), Random.Range(-600f, 600f));
            image.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

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