using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace SFramework.Core.Runtime
{
    public static class SFDebug
    {
        public static bool IsDebug { get; private set; }

        private static HashSet<ILogger> _loggers = new();

        public static void RegisterLogger(ILogger logger)
        {
            _loggers.Add(logger);
        }

        [StringFormatMethod("message")]
        public static void Log(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.Log(message);
            }
        }
        
        [StringFormatMethod("message")]
        public static void LogWarning(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.Log(LogType.Warning, message);
            }
        }
        
        [StringFormatMethod("message")]
        public static void LogError(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.Log(LogType.Error, message);
            }
        }

        [StringFormatMethod("message")]
        public static void Log(string message, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.LogFormat(LogType.Log, message, args);
            }
        }
        
        [StringFormatMethod("message")]
        public static void LogWarning(string message, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.LogFormat(LogType.Warning, message, args);
            }
        }
        
        [StringFormatMethod("message")]
        public static void LogError(string message, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.LogFormat(LogType.Error, message, args);
            }
        }

        [StringFormatMethod("message")]
        public static void Log(LogType logType, string message, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.LogFormat(logType, message, args);
            }
        }

        [StringFormatMethod("message")]
        public static void Log(LogType logType, UnityEngine.Object context, string message, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.LogFormat(logType, context, message, args);
            }
        }

        public static void LogException(Exception exception)
        {
            foreach (var logger in _loggers)
            {
                logger.LogException(exception);
            }
        }

        public static void LogException(Exception exception, UnityEngine.Object context)
        {
            foreach (var logger in _loggers)
            {
                logger.LogException(exception, context);
            }
        }

        public static void SetDebug(bool isDebug)
        {
            IsDebug = isDebug;
        }
    }
}