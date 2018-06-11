using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zara.Common.Utility;

namespace Zara.Main
{
    public class GameRestarter : MonoBehaviour
    {
        IEnumerator Start()
        {
            foreach (Action action in StaticFieldRestarter.EnumerateRestartActions())
            {
                if (TimeUtility.DropFrameExists())
                    yield return null;
                action.Invoke();
            }

            if (TimeUtility.DropFrameExists())
                yield return null;
            Resources.UnloadUnusedAssets();

            if (TimeUtility.DropFrameExists())
                yield return null;
            GC.Collect();


            if (TimeUtility.DropFrameExists())
                yield return null;
            SceneManager.LoadSceneAsync("Permanent");
        }
    }
}