using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Zara.Common.Menu;

namespace Zara.Expansion.Menu.Editor
{
#if UNITY_EDITOR
    public partial class ApplicationBuilder
    {
        const string MenuDirPath = ConstMenu.MenuRootPath + "ApplicationBuilder/";
        const string OutputDirPath = "Build/";

        static void Build(BuildTarget buildTarget, BuildOptions buildOptions)
        {
            var buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = EnumerateScenePaths().ToArray();
            buildPlayerOptions.locationPathName = $"{OutputDirPath}{buildTarget.ToString()}";
            buildPlayerOptions.target = buildTarget;
            buildPlayerOptions.options = buildOptions;
            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        static IEnumerable<string> EnumerateScenePaths()
        {
            var allScenePaths = AssetDatabase.FindAssets("t:Scene")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid));

            var errorMessages = new List<string>();
            var sceneNamePathPairs = new Dictionary<string, string>();
            foreach (string scenePath in allScenePaths)
            {
                string sceneName = Path.GetFileNameWithoutExtension(scenePath);

                if (!sceneNamePathPairs.ContainsKey(sceneName))
                {
                    sceneNamePathPairs.Add(sceneName, scenePath);
                }
                else
                {
                    string message = $"Scenes that name is {sceneName} exist. Rename those secens./ {sceneName}という名前のシーンが複数存在します。違う名前になるように変更してください。";
                    Debug.LogError(message);
                    errorMessages.Add(message);
                }
            }

            foreach (string sceneName in EnumerateSceneNames())
            {
                string scenePath = null;
                if (sceneNamePathPairs.TryGetValue(sceneName, out scenePath))
                {
                    yield return scenePath;
                }
                else
                {
                    string message = $"Scene {sceneName} does not exist. / {sceneName}という名前のシーンが存在しません。";
                    Debug.LogError(message);
                    errorMessages.Add(message);
                }
            }

            if (errorMessages.Any())
            {
                string message = errorMessages.Aggregate((s1, s2) => $"{s1}\n{s2}");
                throw new System.Exception(message);
            }
        }
    }
#endif
}