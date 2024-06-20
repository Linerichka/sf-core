using System;
using JetBrains.Annotations;
using UnityEngine;
namespace SFramework.Core.Runtime
{
    public static class SFDebug
    {
        public static bool IsDebug { get; private set; }

        private static bool CanLog => !Application.isPlaying || IsDebug;

        [StringFormatMethod("message")]
        public static void Log(string message)
        {
            if (!CanLog) return;
            Debug.Log(message);
        }
        
        [StringFormatMethod("message")]
        public static void Log(string message, params object[] args)
        {
            if (!CanLog) return;
            Debug.LogFormat(LogType.Log, LogOption.None, null, message, args);
        }

        [StringFormatMethod("message")]
        public static void Log(LogType logType, string message, params object[] args)
        {
            if (!CanLog) return;
            Debug.LogFormat(logType, LogOption.None, null, message, args);
        }

        [StringFormatMethod("message")]
        public static void Log(LogType logType, UnityEngine.Object context, string message, params object[] args)
        {
            if (!CanLog) return;
            Debug.LogFormat(logType, LogOption.None, context, message, args);
        }

        public static void Exception(Exception exception)
        {
            Debug.LogException(exception);
        }

        public static void Exception(Exception exception, UnityEngine.Object context)
        {
            Debug.LogException(exception, context);
        }

        public static void SetDebug(bool isDebug)
        {
            IsDebug = isDebug;
        }
    }
}
