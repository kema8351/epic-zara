using System;
using System.Collections.Generic;

namespace Zara.Common.Utility
{
    public class StaticFieldRestarter
    {
        static Dictionary<Type, Action> restartActions = new Dictionary<Type, Action>();

        public static void RegisterRestartAction(Type type, Action restartAction)
        {
            if (restartActions.ContainsKey(type))
                return;

            restartActions[type] = restartAction;
        }

        public static IEnumerable<Action> EnumerateRestartActions()
        {
            return restartActions.Values;
        }
    }
}