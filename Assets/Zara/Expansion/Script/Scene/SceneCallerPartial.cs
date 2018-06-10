﻿







// Generated by T4Template
using System;
using Zara.Expansion.Ui;

namespace Zara.Expansion.ExScene
{
    public partial class SceneCaller
    {
        
        public void LoadError(Action<Zara.Main.Ui.Error.ErrorSceneStarter> onLoaded = null) { sceneLoader.Load(onLoaded); }
        public OverlayUiOperator ShowLoading() { return OverlayUiDirector.Instance.Show<Zara.Main.Ui.Loading.LoadingSceneStarter>(); }
        public void LoadTitle(Action<Zara.Main.Ui.Title.TitleSceneStarter> onLoaded = null, OverlayUiOperator? overlayUiOperator = null) { CoreUiDirector.Instance.ChangeCoreUi<Zara.Main.Ui.Title.TitleSceneStarter>(onLoaded, overlayUiOperator); }
        public void LoadDownload(Action<Zara.Main.Ui.Download.DownloadSceneStarter> onLoaded = null, OverlayUiOperator? overlayUiOperator = null) { CoreUiDirector.Instance.ChangeCoreUi<Zara.Main.Ui.Download.DownloadSceneStarter>(onLoaded, overlayUiOperator); }
        public void LoadFirst(Action<Zara.Main.Ui.First.FirstSceneStarter> onLoaded = null, OverlayUiOperator? overlayUiOperator = null) { CoreUiDirector.Instance.ChangeCoreUi<Zara.Main.Ui.First.FirstSceneStarter>(onLoaded, overlayUiOperator); }
        public void LoadPlane(Action<Zara.Main.Ui.Plane.PlaneSceneStarter> onLoaded = null) { sceneLoader.Load(onLoaded); }
        public void LoadHeader(Action<Zara.Main.Ui.Header.HeaderSceneStarter> onLoaded = null) { sceneLoader.Load(onLoaded); }
        public void LoadPopup2d(Action<Zara.Main.Ui.Popup2d.Popup2dSceneStarter> onLoaded = null) { sceneLoader.Load(onLoaded); }
        public void LoadPopup3d(Action<Zara.Main.Ui.Popup3d.Popup3dSceneStarter> onLoaded = null) { sceneLoader.Load(onLoaded); }
    }
}
