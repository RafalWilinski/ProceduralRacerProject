//
// CameraShakeBase.cs
//
// Author(s):
//       Josh Montoute <josh@thinksquirrel.com>
//
// Copyright (c) 2012-2014 Thinksquirrel Software, LLC
//
using UnityEngine;

[assembly:System.Runtime.CompilerServices.InternalsVisibleTo("Assembly-CSharp-Editor")]

#if !(UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
namespace Thinksquirrel.Utilities
{
#endif
    /// <summary>
    /// The base class for all Camera Shake components.
    /// </summary>
    [AddComponentMenu("")]
    public abstract class CameraShakeBase : MonoBehaviour
    {
        /// <summary>
        /// Logs a prefixed message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="prefix">The prefix to log with the message.</param>
        /// <param name="type">An identifier formatted with the prefix.</param>
        public static void Log(object message, string prefix, string type)
        {
            Debug.Log(string.Format("[{0}] ({1}): {2}", prefix, type, message));
        }
        /// <summary>
        /// Logs a prefixed warning.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="prefix">The prefix to log with the message.</param>
        /// <param name="type">An identifier formatted with the prefix.</param>
        public static void LogWarning(object message, string prefix, string type)
        {
            Debug.LogWarning(string.Format("[{0}] ({1}): {2}", prefix, type, message));
        }
        /// <summary>
        /// Logs a prefixed erorr.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="prefix">The prefix to log with the message.</param>
        /// <param name="type">An identifier formatted with the prefix.</param>
        public static void LogError(object message, string prefix, string type)
        {
            Debug.LogError(string.Format("[{0}] ({1}): {2}", prefix, type, message));
        }
        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        public static void LogException(System.Exception ex)
        {
#if UNITY_3_5
            Debug.LogError(string.Format("{0}\n{1}", ex.Message, ex.StackTrace));
#else
            Debug.LogException(ex);
#endif
        }
        /// <summary>
        /// Logs a prefixed message, with context.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="prefix">The prefix to log with the message.</param>
        /// <param name="type">An identifier formatted with the prefix.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void Log(object message, string prefix, string type, Object context)
        {
            Debug.Log(string.Format("[{0}] ({1}): {2}", prefix, type, message), context);
        }
        /// <summary>
        /// Logs a prefixed warning, with context.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="prefix">The prefix to log with the message.</param>
        /// <param name="type">An identifier formatted with the prefix.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogWarning(object message, string prefix, string type, Object context)
        {
            Debug.LogWarning(string.Format("[{0}] ({1}): {2}", prefix, type, message), context);
        }
        /// <summary>
        /// Logs a prefixed exception, with context.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="prefix">The prefix to log with the message.</param>
        /// <param name="type">An identifier formatted with the prefix.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogError(object message, string prefix, string type, Object context)
        {
            Debug.LogError(string.Format("[{0}] ({1}): {2}", prefix, type, message), context);
        }
        /// <summary>
        /// Logs an exception, with context.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="context">Object to which the message applies.</param>
        public static void LogException(System.Exception ex, Object context)
        {
#if UNITY_3_5
            Debug.LogError(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), context);
#else
            Debug.LogException(ex, context);
#endif
        }
    }
#if !(UNITY_3_3 || UNITY_3_4 || UNITY_3_5)
}
#endif
