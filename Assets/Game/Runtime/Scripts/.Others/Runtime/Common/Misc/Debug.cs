using System;
using System.Diagnostics;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using UnityDebug = UnityEngine.Debug;

public static class Debug
{
    public static bool developerConsoleVisible
    {
        get => UnityDebug.developerConsoleVisible;
        set => UnityDebug.developerConsoleVisible = value;
    }

    public static bool isDebugBuild => UnityDebug.isDebugBuild;

    // Used in quantum internal logs
    public static void Log(string message)
    {
        Log(message, null);
    }

    [Conditional("ENABLE_LOGS")]
    public static void Log(object message, UnityObject context = null)
    {
        UnityDebug.Log(message, context);
    }

    [Conditional("ENABLE_LOGS")]
    public static void LogFormat(string format, params object[] args)
    {
        UnityDebug.LogFormat(format, args);
    }

    [Conditional("ENABLE_LOGS")]
    public static void LogFormat(UnityObject context, string format, params object[] args)
    {
        UnityDebug.LogFormat(context, format, args);
    }

    // Used in quantum internal logs
    public static void LogWarning(string message)
    {
        LogWarning(message, null);
    }

    [Conditional("ENABLE_LOGS")]
    public static void LogWarning(object message, UnityObject context = null)
    {
        UnityDebug.LogWarning(message, context);
    }

    [Conditional("ENABLE_LOGS")]
    public static void LogWarningFormat(string format, params object[] args)
    {
        UnityDebug.LogWarningFormat(format, args);
    }

    [Conditional("ENABLE_LOGS")]
    public static void LogWarningFormat(UnityObject context, string format, params object[] args)
    {
        UnityDebug.LogWarningFormat(context, format, args);
    }

    // Used in quantum internal logs
    public static void LogError(string message)
    {
        LogError(message, null);
    }

    [Conditional("ENABLE_LOGS")]
    public static void LogError(object message, UnityObject context = null)
    {
        UnityDebug.LogError(message, context);
    }

    [Conditional("ENABLE_LOGS")]
    public static void LogErrorFormat(string format, params object[] args)
    {
        UnityDebug.LogErrorFormat(format, args);
    }

    [Conditional("ENABLE_LOGS")]
    public static void LogErrorFormat(UnityObject context, string format, params object[] args)
    {
        UnityDebug.LogErrorFormat(context, format, args);
    }

    // Used in quantum internal logs
    public static void LogException(Exception exception)
    {
        LogException(exception, null);
    }

    [Conditional("ENABLE_LOGS")]
    public static void LogException(Exception exception, UnityObject context)
    {
        UnityDebug.LogException(exception, context);
    }

    public static void Assert(bool condition, string message = null, UnityObject context = null)
    {
        UnityDebug.Assert(condition, message, context);
    }

    public static void AssertFormat(bool condition, string format, params object[] args)
    {
        UnityDebug.AssertFormat(condition, format, args);
    }

    public static void AssertFormat(bool condition, UnityObject context, string format, params object[] args)
    {
        UnityDebug.AssertFormat(condition, context, format, args);
    }

    public static void LogAssertion(object message, UnityObject context = null)
    {
        UnityDebug.LogAssertion(message, context);
    }

    public static void LogAssertionFormat(UnityObject context, string format, params object[] args)
    {
        UnityDebug.LogAssertionFormat(context, format, args);
    }

    public static void DrawLine(Vector3 start, Vector3 end)
    {
        DrawLine(start, end, Color.white);
    }

    public static void DrawLine(Vector3 start,
                                Vector3 end,
                                Color color,
                                float duration = 0.0f,
                                bool depthTest = true)
    {
        UnityDebug.DrawLine(start, end, color, duration, depthTest);
    }

    public static void DrawRay(Vector3 start, Vector3 dir)
    {
        DrawRay(start, dir, Color.white);
    }

    public static void DrawRay(Vector3 start,
                               Vector3 dir,
                               Color color,
                               float duration = 0.0f,
                               bool depthTest = true)
    {
        UnityDebug.DrawRay(start, dir, color, duration, depthTest);
    }

    public static void Break()
    {
        UnityDebug.Break();
    }

    public static void DebugBreak()
    {
        UnityDebug.DebugBreak();
    }

    public static void ClearDeveloperConsole()
    {
        UnityDebug.ClearDeveloperConsole();
    }
}
