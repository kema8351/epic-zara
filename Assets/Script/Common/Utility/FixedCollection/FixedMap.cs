using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Zara.Common.Utility
{
    public class FixedMap<TKey, TValue>
    {
        Dictionary<TKey, TValue> dictionary;

        public FixedMap(IEnumerable<TValue> values, Func<TValue, TKey> getKeyFunc)
        {
            try
            {
                dictionary = values.ToDictionary(
                    value => getKeyFunc.Invoke(value),
                    value => value);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                dictionary = new Dictionary<TKey, TValue>();
            }
        }

        public TValue Get(TKey key, bool shouldLog404 = true)
        {
            return dictionary.GetValue(key, shouldLog404);
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public int Count => dictionary.Count;
        public IEnumerable<TKey> Keys => dictionary.Keys;
        public IEnumerable<TValue> Values => dictionary.Values;
    }
}