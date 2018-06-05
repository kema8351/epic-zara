using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zara.Common;
using Zara.Common.ExCamera;
using Zara.Common.ExScene;
using Zara.Common.ExBase;
using Zara.Common.Ui;
using Zara.Expansion.ExCamera;
using Zara.Expansion.ExScene;
using Zara.Expansion.Ui;
using Zara.Main.Ui;
using Zara.Main.Ui.Header;
using Zara.Main.Ui.Title;
using Zara.Expansion.ExAssetBundle;
using Zara.Common.ExAssetBundle;
using Zara.Main.Ui.Error;

namespace Zara.Main
{
    public class Game : ExMonoBehaviour
    {
        [SerializeField]
        UiCameraGenerator uiCameraGenerator;

        [SerializeField]
        AssetCaller assetCaller;

        [SerializeField]
        GameStarter gameStarter;

        public static SceneCaller Scene { get; private set; }
        public static AssetCaller AssetCaller { get; private set; }
        public static IAssetEntry Asset => AssetCaller.Resource;
        public static HeaderCaller HeaderCaller { get; private set; }
        public static IHeaderUi Header => HeaderCaller.Ui;
        public static ErrorSceneCaller Error { get; private set; }

        IEnumerator Start()
        {
            Scene = InitSceneCaller();
            Error = new ErrorSceneCaller(Scene);
            AssetCaller = InitAssetCaller(Error);
            HeaderCaller = new HeaderCaller();

            yield return gameStarter.StartGame();
        }

        AssetCaller InitAssetCaller(ErrorSceneCaller errorSceneCaller)
        {
            assetCaller.Init(@"http://localhost/AssetBundle/Android/", errorSceneCaller);
            return assetCaller;
        }

        SceneCaller InitSceneCaller()
        {
            CameraDirector cameraDirector = new CameraDirector(
              ConstCamera.EnumerateStudios());

            CanvasDirector canvasDirector = new CanvasDirector(
                ConstStratum.EnumerateStrata(),
                uiCameraGenerator,
                cameraDirector);

            SceneDirector.Instance.Init(LoadSceneFromAssetBundle);
            SceneLoader sceneLoader = new SceneLoader(SceneDirector.Instance);
            BackgroundUiDirector.Instance.Init(canvasDirector);
            CoreUiDirector.Instance.Init(sceneLoader, canvasDirector);
            ResidentUiDirector.Instance.Init(canvasDirector);
            PopupUiDirector.Instance.Init(canvasDirector);
            OverlayUiDirector.Instance.Init(sceneLoader, canvasDirector);
            FrontmostUiDirector.Instance.Init(canvasDirector);

            return new SceneCaller(sceneLoader);
        }

        void LoadSceneFromAssetBundle(string sceneName)
        {
            assetCaller.Resource.LoadSceneAdditive($"Scene/{sceneName}.unity");
        }
    }
}