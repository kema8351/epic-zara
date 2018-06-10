using System;
using System.Collections.Generic;
using Zara.Expansion.ExScene;

namespace Zara.Main.Ui.Error
{
    public class ErrorSceneCaller
    {
        ErrorUi ui = null;
        string errorMessage = null;
        Queue<RetryTask> retryTasks = new Queue<RetryTask>();

        public bool IsLoaded => ui != null;

        public ErrorSceneCaller(SceneCaller sceneCaller)
        {
            sceneCaller.LoadError(ss =>
            {
                ui = ss.Ui;
                ui.Init(Retry, TryShow);
                TryShow();
            });
        }

        public void NotifyError(string message)
        {
            if (errorMessage != null)
                errorMessage = message;

            TryShow();
        }

        public void AskRetry(string message, Action retryAction)
        {
            retryTasks.Enqueue(new RetryTask(message, retryAction));
            TryShow();
        }

        void Retry()
        {
            if (errorMessage != null)
                return;

            while (retryTasks.Count > 0)
            {
                RetryTask task = retryTasks.Dequeue();
                task.RetryAction?.Invoke();
            }
        }

        void TryShow()
        {
            if (ui == null)
                return;

            if (!ui.CanShow)
                return;

            if (errorMessage != null)
            {
                ui.Show(errorMessage, false);
                return;
            }

            if (retryTasks.Count > 0)
            {
                ui.Show(retryTasks.Peek().Message, true);
                return;
            }
        }
    }

    public struct RetryTask
    {
        public string Message { get; private set; }
        public Action RetryAction { get; private set; }

        public RetryTask(string message, Action retryAction)
        {
            this.Message = message;
            this.RetryAction = retryAction;
        }
    }
}