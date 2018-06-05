using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zara.Common.ExScene;
using Zara.Expansion.Ui;

namespace Zara.Main.Ui.First
{
    public class FirstUi : CoreUi
    {
        [SerializeField] Button changeBackgroundButton;
        [SerializeField] Button reloadButton;
        [SerializeField] Image image;

        protected override IEnumerator OnUiStarted()
        {
            image.transform.localPosition = new Vector2(UnityEngine.Random.Range(-800f, 800f), UnityEngine.Random.Range(-600f, 600f));
            image.color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));

            changeBackgroundButton.onClick.AddListener(OnChangeBackgroundButtonClicked);
            reloadButton.onClick.AddListener(OnReloaddButtonClicked);

            yield break;
        }

        protected override IEnumerator OnUiEntrying()
        {
            Game.Scene.LoadPlane();
            Game.HeaderCaller.Show();

            yield break;
        }

        protected override IEnumerator OnUiEntried() { yield break; }
        protected override IEnumerator OnUiExiting() { yield break; }
        protected override IEnumerator OnUiExited() { yield break; }

        void OnChangeBackgroundButtonClicked()
        {
            Game.Scene.LoadPlane();
        }

        void OnReloaddButtonClicked()
        {
            Game.Scene.LoadFirst();
        }
    }
}