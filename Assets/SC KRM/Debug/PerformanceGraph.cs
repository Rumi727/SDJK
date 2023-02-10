using SCKRM.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI.Extensions;

namespace SCKRM.DebugUI
{
    [WikiDescription("F3 디버그 모드의 FPS 및 메모리 그래프를 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/Debug/UI/Performance Graph")]
    public sealed class PerformanceGraph : UIAniBase
    {
        [SerializeField] UILineRenderer deltaTime;
        [SerializeField] UILineRenderer memory;

        [SerializeField] List<FPSLine> fpsLines = new List<FPSLine>();
        [SerializeField] List<MemoryLine> memoryLines = new List<MemoryLine>();

        readonly List<float> deltaTimeList = new List<float>();
        readonly List<long> memoryList = new List<long>();
        readonly List<float> yList = new List<float>();

        float deltaTimeHeight = 0.0001f;
        float memoryHeight = 104857600f;

        float timer = 0;
        float deltaTimeXSize = 0;
        float memoryXSize = 0;
        void Update()
        {
            deltaTimeXSize = deltaTime.rectTransform.rect.size.x / DebugManager.SaveData.graphSpeed;
            memoryXSize = memory.rectTransform.rect.size.x / DebugManager.SaveData.graphSpeed;
            timer += Kernel.unscaledDeltaTime;

            DeltaTimeRefresh();
            MemoryRefresh();

            if (timer >= DebugManager.SaveData.graphRefreshDelay)
            {
                DeltaTimeRender();
                MemoryRender();
                timer = 0;
            }
        }

        protected override void OnDisable()
        {
            deltaTime.Points = new Vector2[0];
            memory.Points = new Vector2[0];

            deltaTimeList.Clear();
            memoryList.Clear();

            deltaTimeHeight = 0.0001f;
            memoryHeight = 104857600;

            timer = 0;
        }

        void DeltaTimeRefresh()
        {
            deltaTimeList.Add(Kernel.unscaledDeltaTime);

            int length = deltaTimeXSize.CeilToInt();
            while (deltaTimeList.Count > length)
                deltaTimeList.RemoveAt(0);



            float max = deltaTimeList.Max().Clamp(0.0001f);
            if (!Kernel.isPlaying || !lerp)
                deltaTimeHeight = max;
            else
                deltaTimeHeight = deltaTimeHeight.Lerp(max, lerpValue * Kernel.fpsUnscaledSmoothDeltaTime);
        }

        void DeltaTimeRender()
        {
            if (deltaTime.Points.Length != deltaTimeList.Count)
                deltaTime.Points = new Vector2[deltaTimeList.Count];

            for (int i = deltaTimeList.Count - 1; i >= 0; i--)
                deltaTime.Points[i] = new Vector2(i / deltaTimeXSize, deltaTimeList[i] / deltaTimeHeight);

            deltaTime.SetAllDirty();
            DeltaTimeFpsLineRefresh();
        }

        void DeltaTimeFpsLineRefresh()
        {
            yList.Clear();

            for (int i = 0; i < fpsLines.Count; i++)
            {
                FPSLine fpsLine = fpsLines[i];
                float y = fpsLine.ms * 0.001f / deltaTimeHeight;

                if (y <= 0.2f)
                    yList.Add(-1);
                else
                    yList.Add(y);
            }

            yList.Deduplicate(0.2f, -1);

            for (int i = 0; i < yList.Count; i++)
            {
                FPSLine fpsLine = fpsLines[i];
                float y = yList[i];

                fpsLine.rectTransform.anchorMin = new Vector2(0, y);
                fpsLine.rectTransform.anchorMax = new Vector2(1, y);
            }
        }



        void MemoryRefresh()
        {
            int length = memoryXSize.CeilToInt();

            memoryList.Add(Profiler.GetTotalAllocatedMemoryLong());

            while (memoryList.Count > length)
                memoryList.RemoveAt(0);



            long max = memoryList.Max().Clamp(104857600);
            if (!Kernel.isPlaying || !lerp)
                memoryHeight = max;
            else
                memoryHeight = memoryHeight.Lerp(max, lerpValue * Kernel.fpsUnscaledSmoothDeltaTime);
        }

        void MemoryRender()
        {
            if (memory.Points.Length != memoryList.Count)
                memory.Points = new Vector2[memoryList.Count];

            for (int i = memoryList.Count - 1; i >= 0; i--)
                memory.Points[i] = new Vector2(i / memoryXSize, memoryList[i] / memoryHeight);

            memory.SetAllDirty();
            MemoryLineRefresh();
        }

        void MemoryLineRefresh()
        {
            yList.Clear();

            for (int i = 0; i < memoryLines.Count; i++)
            {
                MemoryLine memoryLine = memoryLines[i];
                float y = memoryLine.b / memoryHeight;

                if (y <= 0.2f)
                    yList.Add(-1);
                else
                    yList.Add(y);
            }

            yList.Deduplicate(0.2f, -1);

            for (int i = 0; i < memoryLines.Count; i++)
            {
                MemoryLine memoryLine = memoryLines[i];
                float y = yList[i];

                memoryLine.rectTransform.anchorMin = new Vector2(0, y);
                memoryLine.rectTransform.anchorMax = new Vector2(1, y);
            }
        }

        [Serializable]
        class FPSLine
        {
            public RectTransform rectTransform;
            public float ms = 0.1f;
        }

        [Serializable]
        class MemoryLine
        {
            public RectTransform rectTransform;
            public long b = 1024;
        }
    }
}
