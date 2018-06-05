using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zara.Common;
using Zara.Common.ExScene;
using Zara.Common.ExBase;
using Zara.Common.Menu;
using Zara.Common.Ui;
using Zara.Common.Utility;
using Zara.Expansion.ExBase;

namespace Zara.Expansion.Ui
{
    public abstract class BaseUi : ExMonoBehaviour
    {
        [AutoAddCanvasGroup]
        [SerializeField]
        CanvasGroup canvasGroup;

        [AutoGet]
        [SerializeField]
        UiEntryExecutor uiEntryExecutor;

        [AutoGet]
        [SerializeField]
        UiExitExecutor uiExitExecutor;

        Coroutine prepareCoroutine = null;
        protected bool IsPrepared { get; private set; } = false;
        List<Action> preparedActionList = null;

        public void AddPreparedAction(Action onPrepared)
        {
            if (IsPrepared)
            {
                onPrepared?.Invoke();
                return;
            }

            AddToList(onPrepared, ref preparedActionList);
        }

        protected Coroutine Prepare()
        {
            if (prepareCoroutine != null || IsPrepared)
                return prepareCoroutine;

            prepareCoroutine = StartCoroutine(PrepareAsync());
            return prepareCoroutine;
        }

        IEnumerator PrepareAsync()
        {
            yield return OnUiStarted();
            IsPrepared = true;
            prepareCoroutine = null;

            InvokeAndClearActionList(ref preparedActionList);
        }

        protected abstract IEnumerator OnUiStarted();

        #region Entry

        Coroutine entryCoroutine = null;
        protected bool IsShown { get; private set; } = false;
        List<Action> entriedActionList = null;
        bool shouldShowImmediately = false;

        public void AddEntriedAction(Action onEntried)
        {
            if (IsShown)
            {
                onEntried?.Invoke();
                return;
            }

            AddToList(onEntried, ref entriedActionList);
        }

        protected Coroutine Entry()
        {
            return Entry(false);
        }

        protected Coroutine EntryImmediately()
        {
            return Entry(true);
        }

        Coroutine Entry(bool shouldEntryImmediately)
        {
            if (IsShown)
                return null;

            this.shouldShowImmediately |= shouldEntryImmediately;

            if (entryCoroutine != null)
            {
                if (shouldShowImmediately)
                    uiEntryExecutor.SkipToEntried();
                return entryCoroutine;
            }

            entryCoroutine = StartCoroutine(EntryAsync());
            return entryCoroutine;
        }

        IEnumerator EntryAsync()
        {
            while (!IsPrepared)
                yield return null;

            while (!IsHidden)
                yield return null;
            IsHidden = false;

            if (shouldUnload)
                yield break;

            yield return OnUiEntrying();
            yield return uiEntryExecutor.Entry(shouldShowImmediately);
            yield return OnUiEntried();
            IsShown = true;
            shouldShowImmediately = false;
            entryCoroutine = null;

            InvokeAndClearActionList(ref entriedActionList);
        }

        protected abstract IEnumerator OnUiEntrying();
        protected abstract IEnumerator OnUiEntried();

        #endregion

        #region Exit

        Coroutine exitCoroutine = null;
        protected bool IsHidden { get; private set; } = true;
        List<Action> exitedActionList = null;
        bool shouldHideImmediately = false;

        public void AddExitedAction(Action onExited)
        {
            if (IsHidden)
            {
                onExited?.Invoke();
                return;
            }

            AddToList(onExited, ref exitedActionList);
        }

        protected Coroutine Exit(bool shouldUnload)
        {
            return Exit(false, shouldUnload);
        }

        protected Coroutine ExitImmediately(bool shouldUnload)
        {
            return Exit(true, shouldUnload);
        }

        Coroutine Exit(bool shouldExitImmediately, bool shouldUnload)
        {
            this.shouldHideImmediately |= shouldExitImmediately;
            this.shouldUnload |= shouldUnload;

            if (exitCoroutine != null)
            {
                uiExitExecutor.SkipToHidden();
                return exitCoroutine;
            }

            exitCoroutine = StartCoroutine(ExitAsync());
            return exitCoroutine;
        }

        IEnumerator ExitAsync()
        {
            while (!IsShown)
                yield return null;
            IsShown = false;

            yield return OnUiExiting();
            yield return uiExitExecutor.Exit(shouldHideImmediately);
            yield return OnUiExited();
            IsHidden = true;
            shouldHideImmediately = false;
            exitCoroutine = null;

            InvokeAndClearActionList(ref exitedActionList);

            TryUnload();
        }

        protected abstract IEnumerator OnUiExiting();
        protected abstract IEnumerator OnUiExited();

        #endregion

        #region Unload

        bool shouldUnload = false;
        List<Action<AsyncOperation>> unloadedActionList = null;

        public void AddUnloadedAction(Action<AsyncOperation> onUnloaded)
        {
            AddToList(onUnloaded, ref unloadedActionList);
        }

        void TryUnload()
        {
            if (!shouldUnload)
                return;

            AsyncOperation unloadingOperation = SceneManager.UnloadSceneAsync(this.gameObject.scene);

            if (unloadedActionList != null)
            {
                foreach (Action<AsyncOperation> action in unloadedActionList)
                    unloadingOperation.completed += action;
                ListPool<Action<AsyncOperation>>.Pool.Put(ref unloadedActionList);
                unloadedActionList = null;
            }
        }

        #endregion

        #region Utility

        void AddToList<T>(T t, ref List<T> list) where T : class
        {
            if (t == null)
                return;

            if (list == null)
                list = ListPool<T>.Pool.Get();
            list.Add(t);
        }

        void InvokeAndClearActionList(ref List<Action> actionList)
        {
            if (actionList == null)
                return;

            foreach (Action action in actionList)
                action.Invoke();
            ListPool<Action>.Pool.Put(ref actionList);
            actionList = null;
        }

        #endregion
    }
}
