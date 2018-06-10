using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zara.Expansion.Ui;

namespace Zara.Main.Ui.Error
{
    public class ErrorUi : FrontmostUi
    {
        [SerializeField] Text messageText;
        [SerializeField] Button quitButton;
        [SerializeField] Button retryButton;

        Action retryAction;
        Action hiddenAction;
        public bool CanShow { get; private set; } = true;

        public void Init(Action retryAction, Action onHidden)
        {
            this.retryAction = retryAction;
            this.hiddenAction = onHidden;
        }

        protected override IEnumerator OnUiStarted()
        {
            quitButton.onClick.AddListener(OnQuitButtonClicked);
            retryButton.onClick.AddListener(OnRetryButtonClicked);
            yield break;
        }

        protected override IEnumerator OnUiEntrying() { yield break; }
        protected override IEnumerator OnUiEntried() { yield break; }
        protected override IEnumerator OnUiExiting() { yield break; }

        protected override IEnumerator OnUiExited()
        {
            this.CanShow = true;
            hiddenAction?.Invoke();
            yield break;
        }

        public void Show(string message, bool canRetry)
        {
            if (!this.CanShow)
            {
                Debug.LogError($"cannot show error message: {message}");
                return;
            }

            this.CanShow = false;
            messageText.text = message;
            retryButton.gameObject.SetActive(canRetry);
            Entry();
        }

        void OnQuitButtonClicked()
        {
            Application.Quit();
        }

        void OnRetryButtonClicked()
        {
            Exit(false);
            retryAction?.Invoke();
        }
    }
}