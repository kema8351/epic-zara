using System;
using UnityEngine;
using UnityEngine.UI;
using Zara.Common.ExBase;

namespace Zara.Main.Ui.Popup2d
{
    public class PopupButtonSet : ExMonoBehaviour
    {
        [SerializeField] Button add2dButton;
        [SerializeField] Button add3dButton;
        [SerializeField] Button removeButton;
        Action exitAction;

        public void Init(Action exitAction)
        {
            this.exitAction = exitAction;

            add2dButton.onClick.AddListener(OnAdd2dButtonClicked);
            add3dButton.onClick.AddListener(OnAdd3dButtonClicked);
            removeButton.onClick.AddListener(OnRemoveButtonClicked);
        }

        void OnAdd2dButtonClicked()
        {
            Game.Scene.LoadPopup2d();
        }

        void OnAdd3dButtonClicked()
        {
            Game.Scene.LoadPopup3d();
        }

        void OnRemoveButtonClicked()
        {
            exitAction.Invoke();
        }
    }
}