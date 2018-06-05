using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zara.Common.ExScene;
using Zara.Common.Ui;
using Zara.Common.Utility;
using Zara.Expansion.ExScene;

namespace Zara.Expansion.Ui
{
    public class CoreUiDirector : ICoreUiBank
    {
        public static CoreUiDirector Instance { get; } = new CoreUiDirector();

        public CoreUiDirector()
        {
            onExited = OnExited;
            exitPreviousUiAction = ExitPreviousUi;
            entryCurrentUiAction = EntryCurrentUi;
        }

        ISceneLoader sceneLoader;
        ICanvasBank canvasBank;
        ICoreUiCanvas previousCoreUiCanvas;
        ICoreUiCanvas currentCoreUiCanvas;
        bool isChanging = false;
        Action<AsyncOperation> onExited;
        Action exitPreviousUiAction;
        Action entryCurrentUiAction;
        bool isExited = false;
        bool isPrepared = false;
        OverlayUiOperator? overlayUiOperator;

        public void Init(ISceneLoader sceneLoader, ICanvasBank canvasBank)
        {
            this.sceneLoader = sceneLoader;
            this.canvasBank = canvasBank;
        }

        bool CheckInited()
        {
            if (sceneLoader == null || canvasBank == null)
            {
                Debug.LogError($"has not inited {this.GetType().Name}");
                return false;
            }
            return true;
        }

        bool CheckChanging()
        {
            if (isChanging)
            {
                Debug.LogError("core ui is changing");
                return false;
            }
            return true;
        }

        public void ChangeCoreUi<T>(Action<T> onLoaded = null, OverlayUiOperator? overlayUiOperator = null) where T : ICoreUiCanvas
        {
            if (!CheckInited() || !CheckChanging())
                return;

            this.overlayUiOperator = overlayUiOperator;

            isChanging = true;
            isExited = false;
            isPrepared = false;
            sceneLoader.Load<T>(onLoaded);
            previousCoreUiCanvas = currentCoreUiCanvas;
            currentCoreUiCanvas = null;

            if (overlayUiOperator.HasValue)
                overlayUiOperator.Value.AddShownAction(exitPreviousUiAction);
            else
                ExitPreviousUi();
        }

        void ExitPreviousUi()
        {
            isExited = (previousCoreUiCanvas == null);

            if (overlayUiOperator.HasValue)
                // 画面はOverlayUiで隠れているので即時退場
                // Because the current ui is hidden by an overlay ui
                previousCoreUiCanvas?.ExitImmediately(onExited);
            else
                previousCoreUiCanvas?.Exit(onExited);

            previousCoreUiCanvas = null;
        }

        void OnExited(AsyncOperation _)
        {
            isExited = true;
            TryEntry();
        }

        void TryEntry()
        {
            if (!isExited || !isPrepared)
                return;

            if (overlayUiOperator.HasValue)
                overlayUiOperator.Value.AddShownAction(entryCurrentUiAction);
            else
                EntryCurrentUi();
        }

        void EntryCurrentUi()
        {
            if (overlayUiOperator.HasValue)
                // 画面はOverlayUiで隠れているので即時出現
                // Because the current ui is hidden by an overlay ui
                currentCoreUiCanvas?.EntryImmediately();
            else
                currentCoreUiCanvas?.Entry();

            overlayUiOperator?.NotifyTermination();
            isChanging = false;
        }

        void ICoreUiBank.NotifyPrepared<T>(T coreUiCanvas)
        {
            if (!CheckInited())
                return;

            currentCoreUiCanvas = coreUiCanvas;

            CanvasInStratum canvasInStratum = new CanvasInStratum(
                ConstStratum.Core,
                // 呼び出し順に登録するのですべて同じOrderInStratumで登録する
                0,
                coreUiCanvas.CanvasOrderUpdater);
            canvasBank.AddCanvas(canvasInStratum);

            isPrepared = true;
            TryEntry();
        }
    }

    public interface ICoreUiBank
    {
        void NotifyPrepared<T>(T coreUiCanvas) where T : ICoreUiCanvas;
    }
}