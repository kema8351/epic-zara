using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Zara.Common.ExBase;
using Zara.Common.Menu;

namespace Zara.Common.Utility
{
#if UNITY_EDITOR
    public class AutoAttributeUtility
    {
        public static void SetComponentAutomatilcally(Component component)
        {
            var componentType = component.GetType();

            foreach (var field in Cache.GetFields(componentType))
            {
                FieldInfo fieldInfo = field.FieldInfo;
                AutoAttribute autoAttribute = field.AutoAttribute;
                Type fieldType = fieldInfo.FieldType;
                Component necessaryComponent = component.GetComponent(fieldType);
                if (necessaryComponent != null)
                {
                    // 何もする必要はない
                    // necessary to do nothing
                }
                else if (!autoAttribute.ShouldAddAutomatilcally)
                {
                    Debug.LogError($"cannot get {fieldType.Name}: Name ={component.gameObject.name}, Type ={component.GetType().Name}");
                }
                else
                {
                    Debug.Log($"add {fieldType.Name}: Name ={component.gameObject.name}, Type ={component.GetType().Name}");
                    necessaryComponent = component.gameObject.AddComponent(fieldType);

                    if (necessaryComponent == null)
                        Debug.LogError($"cannot add {fieldType.Name}: Name ={component.gameObject.name}, Type ={component.GetType().Name}");

                    var modifiedComponent = necessaryComponent as IModifiedComponent;
                    if (modifiedComponent != null)
                        modifiedComponent.Modify();
                }

                if (necessaryComponent != null)
                {
                    autoAttribute.SetDefault(necessaryComponent);
                    fieldInfo.SetValue(component, necessaryComponent);
                }
            }
        }

        public class Cache
        {
            public class Field
            {
                public FieldInfo FieldInfo { get; private set; }
                public AutoAttribute AutoAttribute { get; private set; }

                public Field(FieldInfo fieldInfo, AutoAttribute autoAttribute)
                {
                    this.FieldInfo = fieldInfo;
                    this.AutoAttribute = autoAttribute;
                }
            }

            static Dictionary<Type, IEnumerable<Field>> fieldsDictionary = new Dictionary<Type, IEnumerable<Field>>();
            public static IEnumerable<Field> GetFields(Type type)
            {
                IEnumerable<Field> result = null;
                if (fieldsDictionary.TryGetValue(type, out result))
                    return result;

                result = EnumerateQualifiedFields(type).ToList();
                fieldsDictionary.Add(type, result);
                return result;
            }

            static IEnumerable<Field> EnumerateQualifiedFields(Type type)
            {
                foreach (FieldInfo fieldInfo in EnumerateFieldInfos(type))
                {
                    AutoAttribute autoAttribute = fieldInfo.GetCustomAttributes(true).Select(obj => obj as AutoAttribute).FirstOrDefault(att => att != null);
                    if (autoAttribute != null)
                        yield return new Field(fieldInfo, autoAttribute);
                }
            }

            static IEnumerable<FieldInfo> EnumerateFieldInfos(Type argType)
            {
                foreach (FieldInfo fieldInfo in argType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                    yield return fieldInfo;

                Type type = argType;
                while (type != null)
                {
                    foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                        yield return fieldInfo;

                    type = type.BaseType;
                }
            }
        }
    }
#endif
}