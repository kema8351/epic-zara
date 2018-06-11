using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Zara.Expansion.Menu.Editor
{
#if UNITY_EDITOR
    public partial class ApplicationBuilder
    {        [MenuItem(MenuDirPath + "Debug/Android")]
        static void BuildForDebugOfAndroid()
        {
            Build(BuildTarget.Android, BuildOptions.StrictMode | BuildOptions.Development | BuildOptions.AllowDebugging);
        }
                [MenuItem(MenuDirPath + "Release/Android")]
        static void BuildForReleaseOfAndroid()
        {
            Build(BuildTarget.Android, BuildOptions.StrictMode);
        }
                [MenuItem(MenuDirPath + "Debug/iOS")]
        static void BuildForDebugOfiOS()
        {
            Build(BuildTarget.iOS, BuildOptions.StrictMode | BuildOptions.Development | BuildOptions.AllowDebugging);
        }
                [MenuItem(MenuDirPath + "Release/iOS")]
        static void BuildForReleaseOfiOS()
        {
            Build(BuildTarget.iOS, BuildOptions.StrictMode);
        }
                [MenuItem(MenuDirPath + "Debug/WebGL")]
        static void BuildForDebugOfWebGL()
        {
            Build(BuildTarget.WebGL, BuildOptions.StrictMode | BuildOptions.Development | BuildOptions.AllowDebugging);
        }
                [MenuItem(MenuDirPath + "Release/WebGL")]
        static void BuildForReleaseOfWebGL()
        {
            Build(BuildTarget.WebGL, BuildOptions.StrictMode);
        }
                static IEnumerable<string> EnumerateSceneNames()
        {            yield return "Permanent";            yield return "Restart";            yield return "Error";        }
    }
#endif
}
