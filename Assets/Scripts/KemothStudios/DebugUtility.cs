using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace KemothStudios.Utility
{
    public static class DebugUtility
    {
        [Conditional("DEBUG")]
        public static void Log(string message)
        {
            Debug.Log(message);
        }
        
        [Conditional("DEBUG")]
        public static void LogError(string message)
        {
                Debug.LogError(message);
        }
        
        [Conditional("DEBUG")]
        public static void LogException(Exception exception)
        {
            Debug.LogException(exception);
        }
        
        [Conditional("DEBUG")]
        public static void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }

        [Conditional("DEBUG")]
        public static void LogColored(string colorName, string message)
        {
            Debug.Log($"<color={colorName}>{message}</color>");
        }
    }
}