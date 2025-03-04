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
        public static void Log(string message, params object[] args)
        {
            foreach (var logger in _loggers)
            {
                logger.LogFormat(LogType.Log, message, args);
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

        public static void Exception(Exception exception)
        {
            foreach (var logger in _loggers)
            {
                logger.LogException(exception);
            }
        }

        public static void Exception(Exception exception, UnityEngine.Object context)
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