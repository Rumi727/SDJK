using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SCKRM
{
    /// <summary> 무한 루프 검사 및 방지(에디터 전용) </summary>
    [WikiDescription("무한 루프 검사 및 방지를 위한 클래스 입니다")]
    public static class InfiniteLoopDetector
    {
        static string prevPoint = "";
        static int detectionCount = 0;
        const int detectionThreshold = 10000;

        [Conditional("UNITY_EDITOR")]
        public static void Run([CallerMemberName] string mn = "", [CallerFilePath] string fp = "", [CallerLineNumber] int ln = 0)
        {
            string currentPoint = $"{fp}:{ln}, {mn}()";

            if (prevPoint == currentPoint)
                detectionCount++;
            else
                detectionCount = 0;

            if (detectionCount > detectionThreshold)
                throw new Exception($"Infinite Loop Detected: \n{currentPoint}\n\n");

            prevPoint = currentPoint;
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void Init()
        {
            UnityEditor.EditorApplication.update += () =>
            {
                detectionCount = 0;
            };
        }
#endif
    }
}