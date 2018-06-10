using System.Collections.Generic;
using UnityEngine;

namespace Zara.Common.Utility
{
    public static class CollectionExtension
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items)
        {
            return new HashSet<T>(items);
        }

        public static bool IsEmpty<T>(this IReadOnlyCollection<T> collection)
        {
            return collection.Count <= 0;
        }

        public static bool TryAdd<T>(this HashSet<T> hashSet, T item, bool shouldLogContained = true)
        {
            if (hashSet.Contains(item))
            {
                if (shouldLogContained)
                    Debug.LogWarning($"{typeof(T).Name} already exists: item={item}, {StackTraceUtility.ExtractStackTrace()}");

                return false;
            }

            hashSet.Add(item);
            return true;
        }

        public static bool TryRemove<T>(this HashSet<T> hashSet, T item, bool shouldLog404 = true)
        {
            if (!hashSet.Contains(item))
            {
                if (shouldLog404)
                    Debug.LogWarning($"cannot find {typeof(T).Name}: item={item}, {StackTraceUtility.ExtractStackTrace()}");

                return false;
            }

            hashSet.Remove(item);
            return true;
        }

        public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, bool shouldLog404 = true)
        {
            if (!dictionary.ContainsKey(key))
            {
                if (shouldLog404)
                    Debug.LogWarning($"cannot find {typeof(TValue).Name}: key={key}, {StackTraceUtility.ExtractStackTrace()}");

                return false;
            }

            dictionary.Remove(key);
            return true;
        }

        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, bool shouldLog404 = true)
        {
            TValue result;
            if (dictionary.TryGetValue(key, out result))
                return result;

            if (shouldLog404)
                Debug.LogError($"cannot find {typeof(TValue).Name}: key={key}, {StackTraceUtility.ExtractStackTrace()}");

            return default(TValue);
        }

        public static TValue? GetNullableValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, bool shouldLog404 = true)
            where TValue : struct
        {
            TValue result;
            if (dictionary.TryGetValue(key, out result))
                return result;

            if (shouldLog404)
                Debug.LogError($"cannot find {typeof(TValue).Name}: key={key}, {StackTraceUtility.ExtractStackTrace()}");

            return null;
        }

        public static T GetValue<T>(this IList<T> list, int index, bool shouldLog404 = true)
        {
            if (0 <= index && index < list.Count)
                return list[index];

            if (shouldLog404)
                Debug.LogError($"cannot get {typeof(T).Name}: index={index}, count={list.Count} {StackTraceUtility.ExtractStackTrace()}");

            return default(T);
        }

        public static T? GetNullableValue<T>(this IList<T> list, int index, bool shouldLog404 = true)
            where T : struct
        {
            if (0 <= index && index < list.Count)
                return list[index];

            if (shouldLog404)
                Debug.LogError($"cannot get {typeof(T).Name}: index={index}, count={list.Count} {StackTraceUtility.ExtractStackTrace()}");

            return null;
        }
    }
}