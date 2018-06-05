using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using Zara.Common.ExBase;

namespace Zara.Common.Menu
{
#if UNITY_EDITOR

    public class ComponentModifier
    {
        const string DefaultRootPath = "Assets";
        const string SceneExtension = ".unity";
        const string PrefabExtension = ".prefab";
        const string MenuPath = ConstMenu.MenuRootPath + "Modifier/ComponentModify";

        [MenuItem(MenuPath + "All")]
        public static void ModifyAll()
        {
            Modify(EnumerateFilePaths(DefaultRootPath));
        }

        [MenuItem(MenuPath + "Selecting")]
        static void ModifySelecting()
        {
            Modify(GetSelectingFilePaths());
        }

        static void Modify(IEnumerable<string> filePaths)
        {
            List<string> prefabPaths = new List<string>();
            List<string> scenePaths = new List<string>();

            foreach (var path in filePaths)
            {
                string extension = Path.GetExtension(path);

                switch (extension)
                {
                    case PrefabExtension:
                        prefabPaths.Add(path);
                        break;
                    case SceneExtension:
                        scenePaths.Add(path);
                        break;
                }
            }

            if (scenePaths.Any())
            {
                // シーンが変更されている場合は保存ダイアログを表示する
                int sceneCount = EditorSceneManager.sceneCount;
                bool hasBeenChanged = Enumerable.Range(0, sceneCount)
                    .Select(i => EditorSceneManager.GetSceneAt(i))
                    .Any(scene => scene.isDirty);

                if (hasBeenChanged)
                {
                    bool canSave = EditorUtility.DisplayDialog(
                        "Confirmation",
                        "Any loaded scenes have been changed. To save the scenes is necessary to start this process. Can the scenes be saved? ",
                        "OK",
                        "Cancel");

                    if (!canSave)
                        return;
                }

                ModifyScenes(scenePaths);
            }

            ModifyPrefabs(prefabPaths);

            EditorUtility.DisplayProgressBar("Saving assets", "Final phase", 0f);

            AssetDatabase.SaveAssets();

            EditorUtility.ClearProgressBar();
        }

        static void ModifyPrefabs(List<string> prefabPaths)
        {
            for (int i = 0; i < prefabPaths.Count; i++)
            {
                string path = prefabPaths[i];
                EditorUtility.DisplayProgressBar("Checking prefabs", $"({i}/{prefabPaths.Count}) {path}", (float)i / (float)prefabPaths.Count);
                ModifyPrefab(path);
            }
        }

        static void ModifyScenes(List<string> scenePaths)
        {
            int openingSceneCount = EditorSceneManager.sceneCount;

            // 現在編集中のシーンを保存する
            EditorSceneManager.SaveOpenScenes();

            // 処理後にシーン状態を戻すために現在のシーン一覧を記録しておく
            string[] openingScenePaths = Enumerable.Range(0, openingSceneCount)
                .Select(i => EditorSceneManager.GetSceneAt(i))
                // 保存されていないシーンは取り除く
                .Where(s => !string.IsNullOrEmpty(s.path))
                .Select(s => s.path)
                .ToArray();

            // 既に開かれているシーンを再度開こうとすると処理が止まるので、新規シーンのみにする
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            for (int i = 0; i < scenePaths.Count; i++)
            {
                string path = scenePaths[i];
                EditorUtility.DisplayProgressBar("Checking scenes", $"({i}/{scenePaths.Count}) {path}", (float)i / (float)scenePaths.Count);
                ModifyScene(path);
            }

            // 編集前のシーン状態に戻す
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            if (openingScenePaths.Any())
            {
                EditorSceneManager.OpenScene(openingScenePaths[0], OpenSceneMode.Single);
                for (int i = 1; i < openingScenePaths.Length; i++)
                    EditorSceneManager.OpenScene(openingScenePaths[i], OpenSceneMode.Additive);
            }
        }

        static void ModifyScene(string path)
        {
            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            bool hasChanged = false;
            List<GameObject> objectList = GameObject.FindObjectsOfType<GameObject>().ToList();

            foreach (var obj in objectList)
            {
                PrefabType prefabType = PrefabUtility.GetPrefabType(obj);
                bool isPrefabInstance = (prefabType == PrefabType.PrefabInstance);

                // スクリプトによる変更を有効にするためにPrefabとのリンクを切る
                if (isPrefabInstance)
                    PrefabUtility.DisconnectPrefabInstance(obj);

                hasChanged |= ModifyGameObject(obj, path);

                // Prefabとのリンクを再接続
                if (isPrefabInstance)
                {
                    GameObject original = PrefabUtility.GetPrefabParent(obj) as GameObject;
                    GameObject root = PrefabUtility.FindPrefabRoot(obj);
                    PrefabUtility.ConnectGameObjectToPrefab(root, original);
                }
            }

            if (hasChanged)
            {
                // UnityにSceneの変更を通知する（通知しないと保存されない）
                EditorSceneManager.MarkAllScenesDirty();

                EditorSceneManager.SaveOpenScenes();
                Debug.Log($"Save Scene: {path}");
            }
            else
            {
                Debug.Log($"Scene has not changed: {path}");
            }
        }

        static void ModifyPrefab(string path)
        {
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            ModifyGameObject(obj, path);
        }

        static bool ModifyGameObject(GameObject obj, string path)
        {
            bool hasChanged = false;

            foreach (var component in obj.GetComponentsInChildren<Component>())
            {
                IModifiedComponent modifiedComponent = component as IModifiedComponent;
                if (modifiedComponent == null)
                    continue;

                Debug.Log($"Modify {path}: GameObjectName={component.gameObject.name}, ComponentName={component.GetType().Name}");
                hasChanged = true;
                modifiedComponent.Modify();

                // UnityにComponentの変更を通知する（通知しないと保存されない）
                EditorUtility.SetDirty(component);
            }

            return hasChanged;
        }

        static IEnumerable<string> GetSelectingFilePaths()
        {
            string[] guids = Selection.assetGUIDs;
            if (guids != null)
                return guids
                    .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                    .SelectMany(assetPath => EnumerateFilePaths(assetPath));
            else
                return Enumerable.Empty<string>();
        }

        static IEnumerable<string> EnumerateFilePaths(string path)
        {
            if (File.Exists(path))
            {
                yield return path;
                yield break;
            }

            if (!Directory.Exists(path))
                yield break;

            foreach (string subPath in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                yield return subPath;
        }
    }

    public interface IModifiedComponent
    {
        void Modify();
    }
#endif
}