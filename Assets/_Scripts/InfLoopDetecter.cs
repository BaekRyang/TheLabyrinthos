using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEditor;

/// <summary> 무한 루프 검사 및 방지(에디터 전용) </summary>
public static class InfLoopDetector
{
    private static string prevPoint = "";
    private static int detectionCount;
    private const int DetectionThreshold = 100000;

    [Conditional("UNITY_EDITOR")]
    public static void Run(
        [CallerMemberName] string mn = "",
        [CallerFilePath] string fp = "",
        [CallerLineNumber] int ln = 0
    )
    {
        string currentPoint = $"{fp}:{ln}, {mn}()";

        if (prevPoint == currentPoint)
            detectionCount++;
        else
            detectionCount = 0;

        if (detectionCount > DetectionThreshold)
            throw new Exception($"Infinite Loop Detected: \n{currentPoint}\n\n");

        prevPoint = currentPoint;
    }

#if UNITY_EDITOR
    [InitializeOnLoadMethod]
    private static void Init()
    {
        EditorApplication.update += () =>
        {
            detectionCount = 0;
        };
    }
#endif
}