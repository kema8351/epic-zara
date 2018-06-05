using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zara.Common.Utility
{
    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }
    }
}