using System;
using UnityEngine;

namespace Zara.Expansion.ExScene
{
    public partial class SceneLoader : ISceneLoader
    {
        class LoadingActionCache<T>
        {
            public static Action<Action<T>> Action;
        }

        void ISceneLoader.Load<T>(Action<T> onLoaded)
        {
            Action<Action<T>> loadingAction = LoadingActionCache<T>.Action;

            if (loadingAction == null)
            {
                Debug.LogError($"has not registered loading action: {typeof(T).Name}");
                return;
            }

            loadingAction.Invoke(onLoaded);
        }
    }
}
