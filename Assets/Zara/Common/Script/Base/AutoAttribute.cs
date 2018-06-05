using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Zara.Common.ExBase
{
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public abstract class AutoAttribute : Attribute
    {
        public abstract bool ShouldAddAutomatilcally { get; }

        public virtual void SetDefault(object obj) { }

        protected T Cast<T>(object obj) where T : Component
        {
            T t = obj as T;
            if (t == null)
                Debug.LogError($"cannot cast from {obj?.GetType().Name} to {typeof(T).Name}");

            return t;
        }
    }
}