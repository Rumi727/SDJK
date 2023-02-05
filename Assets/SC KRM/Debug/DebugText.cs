using K4.Threading;
using SCKRM.Resource;
using SCKRM.Sound;
using SCKRM.Text;
using SCKRM.Threads;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;

namespace SCKRM.DebugUI
{
    [WikiDescription("F3 디버그 모드의 텍스트를 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/Debug/UI/Debug Text")]
    public sealed class DebugText : UI.UI, ITextRefreshable
    {
        public delegate void DebugTextAction(FastString fastString);
        [WikiDescription("F3 디버그 모드의 왼쪽 텍스트가 새로고침될때 호출되는 이벤트입니다")] public static event DebugTextAction leftDebugText;
        [WikiDescription("F3 디버그 모드의 오른쪽 텍스트가 새로고침될때 호출되는 이벤트입니다")] public static event DebugTextAction rightDebugText;

        [WikiDescription("F3 디버그 모드의 왼쪽 텍스트를 표시하기 위한 FastString 인스턴스를 가져옵니다")] public static FastString leftFastString { get; } = new FastString(2048);
        [WikiDescription("F3 디버그 모드의 오른쪽 텍스트를 표시하기 위한 FastString 인스턴스를 가져옵니다")] public static FastString rightFastString { get; } = new FastString(2048);

        [SerializeField] TMP_Text _leftText; [WikiDescription("왼쪽 텍스트 컴포넌트를 가져옵니다")] public TMP_Text leftText => _leftText;
        [SerializeField] TMP_Text _rightText; [WikiDescription("오른쪽 텍스트 컴포넌트를 가져옵니다")] public TMP_Text rightText => _rightText;

        protected override void Awake()
        {
            leftDebugText += LeftDebug;
            rightDebugText += RightDebug;

            static void LeftDebug(FastString fastString)
            {
                LabelValue("deltaTime", Kernel.deltaTime, fastString);
                LabelValue("smoothDeltaTime", Kernel.smoothDeltaTime, fastString, true);

                LabelValue("fpsDeltaTime", Kernel.fpsDeltaTime, fastString);
                LabelValue("fpsSmoothDeltaTime", Kernel.fpsSmoothDeltaTime, fastString, true);

                LabelValue("unscaledDeltaTime", Kernel.unscaledDeltaTime, fastString);
                LabelValue("unscaledSmoothDeltaTime", Kernel.unscaledSmoothDeltaTime, fastString, true);

                LabelValue("fpsUnscaledDeltaTime", Kernel.fpsUnscaledDeltaTime, fastString);
                LabelValue("fpsUnscaledSmoothDeltaTime", Kernel.fpsUnscaledSmoothDeltaTime, fastString, true);

                LabelValue("fps", Kernel.fps, fastString, true);

                LabelValue("totalAllocatedMemory", (Profiler.GetTotalAllocatedMemoryLong() / 1048576f).Round(4), fastString, true);

                LabelValue("gameSpeed", Kernel.gameSpeed, fastString, true);

                LabelValue("asyncTasksCount", AsyncTaskManager.asyncTasks.Count, fastString, true);

                LabelValue("mainThreadId", ThreadManager.mainThreadId, fastString);
                LabelValue("runningThreadsCount", ThreadManager.runningThreads.Count, fastString, true);

                LabelValue("soundListCount", SoundManager.soundList.Count, fastString);
                LabelValue("nbsListCount", SoundManager.nbsList.Count, fastString);
            }

            static void RightDebug(FastString fastString)
            {
                LabelValue("dataPath", Kernel.dataPath, fastString);
                LabelValue("streamingAssetsPath", Kernel.streamingAssetsPath, fastString);
                LabelValue("persistentDataPath", Kernel.persistentDataPath, fastString);
                LabelValue("temporaryCachePath", Kernel.temporaryCachePath, fastString);
                LabelValue("saveDataPath", Kernel.saveDataPath, fastString);
                LabelValue("resourcePackPath", Kernel.resourcePackPath, fastString);
                LabelValue("projectSettingPath", Kernel.projectSettingPath, fastString, true);

                LabelValue("companyName", Kernel.companyName, fastString);
                LabelValue("productName", Kernel.productName, fastString, true);

                LabelValue("version", Kernel.version, fastString);
                LabelValue("unityVersion", Application.unityVersion, fastString, true);

                LabelValue("platform", Kernel.platform.ToString(), fastString, true);


                LabelValue("operatingSystem", SystemInfo.operatingSystem, fastString, true);

                LabelValue("deviceModel", SystemInfo.deviceModel, fastString);
                LabelValue("deviceName", SystemInfo.deviceName, fastString, true);

                LabelValue("batteryStatus", SystemInfo.batteryStatus.ToString(), fastString, true);

                LabelValue("processorType", SystemInfo.processorType, fastString);
                LabelValue("processorFrequency", SystemInfo.processorFrequency, fastString);
                LabelValue("processorCount", SystemInfo.processorCount, fastString, true);

                LabelValue("graphicsDeviceName", SystemInfo.graphicsDeviceName, fastString);
                LabelValue("graphicsMemorySize", SystemInfo.graphicsMemorySize, fastString, true);

                LabelValue("systemMemorySize", SystemInfo.systemMemorySize, fastString);
            }
        }

        protected override void OnEnable() => Refresh();

        float timer = 0;
        void Update()
        {
            timer += Kernel.unscaledDeltaTime;

            if (timer >= DebugManager.SaveData.textRefreshDelay)
            {
                LeftRefresh();
                timer = 0;
            }
        }

        protected override void OnDisable() => timer = 0;



        [WikiDescription("왼쪽 텍스트를 새로고칩니다")]
        public void LeftRefresh()
        {
            leftFastString.Clear();
            leftDebugText?.Invoke(leftFastString);

            leftText.text = leftFastString.ToString();
        }

        [WikiDescription("모든 텍스트를 새로고칩니다")]
        public void Refresh() => K4UnityThreadDispatcher.Execute(refresh);

        void refresh()
        {
            leftFastString.Clear();
            rightFastString.Clear();

            leftDebugText?.Invoke(leftFastString);
            rightDebugText?.Invoke(rightFastString);

            leftText.text = leftFastString.ToString();
            rightText.text = rightFastString.ToString();
        }



        #region LabelValue
        [WikiDescription(
@"라벨을 만듭니다

예시 코드:
```C#
LabelValue(""Delta Time"", Kernel.deltaTime, fastString);
//결과: Delta Time - 0.016666666
```
")]
        public static void LabelValue(string labelKey, string value, FastString fastString, bool line = false)
        {
            string searchedLabel = ResourceManager.SearchLanguage(labelKey, "sc-krm-debug");
            if (searchedLabel != "")
                fastString.Append(searchedLabel);
            else
                fastString.Append(labelKey);

            fastString.Append(" - ");
            fastString.Append(value);

            if (line)
                fastString.Append("\n\n");
            else
                fastString.Append("\n");
        }

        [WikiIgnore]
        public static void LabelValue(string labelKey, int value, FastString fastString, bool line = false)
        {
            string searchedLabel = ResourceManager.SearchLanguage(labelKey, "sc-krm-debug");
            if (searchedLabel != "")
                fastString.Append(searchedLabel);
            else
                fastString.Append(labelKey);

            fastString.Append(" - ");
            fastString.Append(value);

            if (line)
                fastString.Append("\n\n");
            else
                fastString.Append("\n");
        }

        [WikiIgnore]
        public static void LabelValue(string labelKey, float value, FastString fastString, bool line = false)
        {
            string searchedLabel = ResourceManager.SearchLanguage(labelKey, "sc-krm-debug");
            if (searchedLabel != "")
                fastString.Append(searchedLabel);
            else
                fastString.Append(labelKey);

            fastString.Append(" - ");
            fastString.Append(value);

            if (line)
                fastString.Append("\n\n");
            else
                fastString.Append("\n");
        }
        #endregion
    }
}
