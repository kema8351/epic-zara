using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zara.Common.Utility
{
    //public static class CollectionExtension
    //{
    //    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items)
    //    {
    //        return new HashSet<T>(items);
    //    }

    //    public static bool IsEmpty<T>(this IReadOnlyCollection<T> collection)
    //    {
    //        return collection.Count <= 0;
    //    }

    //    public static bool TryRemove<T>(this HashSet<T> hashSet, T item, bool shouldLog404 = true)
    //    {
    //        if (!hashSet.Contains(item))
    //        {
    //            if (shouldLog404)
    //                Debug.LogWarning($"cannot find {typeof(T).Name}: item={item}, {StackTraceUtility.ExtractStackTrace()}");

    //            return false;
    //        }

    //        hashSet.Remove(item);
    //        return true;
    //    }

    //    public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, bool shouldLog404 = true)
    //    {
    //        if (!dictionary.ContainsKey(key))
    //        {
    //            if (shouldLog404)
    //                Debug.LogWarning($"cannot find {typeof(TValue).Name}: key={key}, {StackTraceUtility.ExtractStackTrace()}");

    //            return false;
    //        }

    //        dictionary.Remove(key);
    //        return true;
    //    }

    //    public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, bool shouldLog404 = true)
    //    {
    //        TValue result;
    //        if (dictionary.TryGetValue(key, out result))
    //            return result;

    //        if (shouldLog404)
    //            Debug.LogError($"cannot find {typeof(TValue).Name}: key={key}, {StackTraceUtility.ExtractStackTrace()}");

    //        return default(TValue);
    //    }

    //    public static TValue? GetNullableValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, bool shouldLog404 = true)
    //        where TValue : struct
    //    {
    //        TValue result;
    //        if (dictionary.TryGetValue(key, out result))
    //            return result;

    //        if (shouldLog404)
    //            Debug.LogError($"cannot find {typeof(TValue).Name}: key={key}, {StackTraceUtility.ExtractStackTrace()}");

    //        return null;
    //    }
    //}
}