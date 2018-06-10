using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zara.Common.Menu;

namespace Zara.Common.Utility
{
#if UNITY_EDITOR
    public class ModificationUtility
    {
        public static T GetAndAddComponent<T>(MonoBehaviour behaviour) where T : Component
        {
            var result = behaviour.GetComponent<T>();
            if (result == null)
            {
                Debug.Log($"add {typeof(T).Name}: Name ={behaviour.gameObject.name}, Type ={behaviour.GetType().Name}");
                result = behaviour.gameObject.AddComponent<T>();

                // 作成したコンポーネントがModify対象なら実行
                var modifiedComponent = result as IModifiedComponent;
                if (modifiedComponent != null)
                    modifiedComponent.Modify();
            }
            return result;
        }

        public static T GetComponent<T>(MonoBehaviour behaviour) where T : Component
        {
            var result = behaviour.GetComponent<T>();
            if (result == null)
                Debug.LogError($"cannot get {typeof(T).Name}: Name ={behaviour.gameObject.name}, Type ={behaviour.GetType().Name}");
            return result;
        }

        public static void DeleteComponent<T>(MonoBehaviour behaviour) where T : Component
        {
            var component = behaviour.GetComponent<T>();
            if (component != null)
                GameObject.Destroy(component);
        }

        public static T[] GetDirectChildComponents<T>(Component component) where T : Component
        {
            return GetDirectChildComponents<T>(component.transform).ToArray();
        }

        static IEnumerable<T> GetDirectChildComponents<T>(Transform transform) where T : Component
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                T component = child.GetComponent<T>();
                if (component != null)
                {
                    yield return component;
                }
                else
                {
                    foreach (T t in GetDirectChildComponents<T>(child))
                        yield return t;
                }
            }
        }
    }
#endif
}