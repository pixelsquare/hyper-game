using System;
using System.Diagnostics;
using UnityEngine;

namespace Kumu.Extensions
{
    public static class DebugExtensions 
    {
        public static string WrapColor(this string value, Color color)
        {
            var hex = ColorUtility.ToHtmlStringRGBA(color);
            return $"<color=#{hex}>{value}</color>";
        }

        public static string WrapColor(this object value, Color color)
        {
            var hex = ColorUtility.ToHtmlStringRGBA(color);
            return $"<color=#{hex}>{value}</color>";
        }

        [Conditional("ENABLE_LOGS")]
        public static void Log(this string value)
        {
            Debug.Log(value);
        }

        [Conditional("ENABLE_LOGS")]
        public static void Log(this string value, Color color)
        {
            var hex = ColorUtility.ToHtmlStringRGBA(color);
            Debug.Log($"<color=#{hex}>{value}</color>");
        }

        [Conditional("ENABLE_LOGS")]
        public static void LogWarning(this string value)
        {
            Debug.LogWarning(value);
        }

        [Conditional("ENABLE_LOGS")]
        public static void LogError(this string value)
        {
            Debug.LogError(value);
        }
        
        [Conditional("ENABLE_LOGS")]
        public static void LogException(this Exception value)
        {
            Debug.LogException(value);
        }
    }
}
