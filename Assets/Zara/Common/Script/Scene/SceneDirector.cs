using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zara.Common.Utility;

namespace Zara.Common.ExScene
{
    public class SceneDirector : Singleton<SceneDirector>, ILoadedActionHolder
    {
        class LoadedActionCache<T> : StaticInstanceCache<LoadedActionCache<T>, Queue<Action<T>>>
        {
        }

        Action<string> loadFromAssetBundleAction;
        MonoBehaviour coroutineStarter;

        public SceneDirector() : base()
        {
        }

        public void Init(Action<string> loadFromAssetBundleAction)
        {
            this.loadFromAssetBundleAction = loadFromAssetBundleAction;
        }

        public void LoadFromBuiltinAsset<T>(string sceneName, Action<T> onLoaded = null)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            EnqueueLoadedAction(onLoaded);
        }

        public void LoadFromAssetBundle<T>(string sceneName, Action<T> onLoaded = null)
        {
            loadFromAssetBundleAction.Invoke(sceneName);
            EnqueueLoadedAction(onLoaded);
        }

        void EnqueueLoadedAction<T>(Action<T> onLoaded)
        {
            LoadedActionCache<T>.Cache.Enqueue(onLoaded);
        }

        void ILoadedActionHolder.RunLoadedAction<T>(T sceneStarter)
        {
            var queue = LoadedActionCache<T>.Cache;
            if (queue.IsEmpty())
            {
                Debug.LogError($"empty loaded action cache: {typeof(T).Name}");
                return;
            }

            Action<T> onLoaded = queue.Dequeue();
            onLoaded?.Invoke(sceneStarter);
        }

        public void Unload(Scene scene, Action<AsyncOperation> onUnloaded = null)
        {
            AsyncOperation op = SceneManager.UnloadSceneAsync(scene);
            if (onUnloaded != null)
                op.completed += onUnloaded;
        }
    }

    public interface ILoadedActionHolder
    {
        void RunLoadedAction<T>(T sceneSetter);
    }
}