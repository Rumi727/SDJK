using SCKRM.Sound;
using SCKRM.Text;
using SCKRM.Threads;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;

namespace SCKRM.DebugUI
{
    [AddComponentMenu("SC KRM/Debug/UI/Debug Text")]
    public sealed class DebugText : UI.UI
    {
        public delegate void DebugTextAction(FastString fastString);
        public static event DebugTextAction leftDebugText;
        public static event DebugTextAction rightDebugText;

        public static FastString leftFastString { get; } = new FastString(2048);
        public static FastString rightFastString { get; } = new FastString(2048);

        [SerializeField] TMP_Text _leftText; public TMP_Text leftText => _leftText;
        [SerializeField] TMP_Text _rightText; public TMP_Text rightText => _rightText;

        protected override void Awake()
        {
            leftDebugText += LeftDebug;
            rightDebugText += RightDebug;

            static void LeftDebug(FastString fastString)
            {
                LabelValue("Delta Time", Kernel.deltaTime, fastString);
                LabelValue("FPS Delta Time", Kernel.fpsDeltaTime, fastString, true);

                LabelValue("Unscaled Delta Time", Kernel.unscaledDeltaTime, fastString);
                LabelValue("Unscaled FPS Delta Time", Kernel.fpsUnscaledDeltaTime, fastString, true);

                LabelValue("FPS", Kernel.fps, fastString, true);

                LabelValue("Total Allocated Memory (MB)", (Profiler.GetTotalAllocatedMemoryLong() / 1048576f).Round(4), fastString, true);

                LabelValue("Game Speed", Kernel.gameSpeed, fastString, true);

                LabelValue("Async Tasks Count", AsyncTaskManager.asyncTasks.Count, fastString, true);

                LabelValue("Main Thread ID", ThreadManager.mainThreadId, fastString);
                LabelValue("Running Threads Count", ThreadManager.runningThreads.Count, fastString, true);

                LabelValue("Sound List Count", SoundManager.soundList.Count, fastString);
                LabelValue("NBS List Count", SoundManager.nbsList.Count, fastString);
            }

            static void RightDebug(FastString fastString)
            {
                LabelValue("Data Path", Kernel.dataPath, fastString);
                LabelValue("Streaming Assets Path", Kernel.streamingAssetsPath, fastString);
                LabelValue("Persistent Data Path", Kernel.persistentDataPath, fastString);
                LabelValue("Temporary Cache Path", Kernel.temporaryCachePath, fastString);
                LabelValue("Save Data Path", Kernel.saveDataPath, fastString);
                LabelValue("Resource Pack Path", Kernel.resourcePackPath, fastString);
                LabelValue("Project Setting Path", Kernel.projectSettingPath, fastString, true);

                LabelValue("Company Name", Kernel.companyName, fastString);
                LabelValue("Product Name", Kernel.productName, fastString, true);

                LabelValue("Version", Kernel.version, fastString);
                LabelValue("Unity Version", Application.unityVersion, fastString, true);

                LabelValue("Platform", Kernel.platform.ToString(), fastString, true);


                LabelValue("OS", SystemInfo.operatingSystem, fastString, true);

                LabelValue("Device Model", SystemInfo.deviceModel, fastString);
                LabelValue("Device Name", SystemInfo.deviceName, fastString, true);

                LabelValue("Battery Status", SystemInfo.batteryStatus.ToString(), fastString, true);

                LabelValue("Processor Type", SystemInfo.processorType, fastString);
                LabelValue("Processor Frequency", SystemInfo.processorFrequency, fastString);
                LabelValue("Processor Count", SystemInfo.processorCount, fastString, true);

                LabelValue("Graphics Device Name", SystemInfo.graphicsDeviceName, fastString);
                LabelValue("Graphics Memory Size", SystemInfo.graphicsMemorySize, fastString, true);

                LabelValue("System Memory Size (MB)", SystemInfo.systemMemorySize, fastString);
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



        public void LeftRefresh()
        {
            leftFastString.Clear();
            leftDebugText?.Invoke(leftFastString);

            leftText.text = leftFastString.ToString();
        }

        public void Refresh()
        {
            leftFastString.Clear();
            rightFastString.Clear();

            leftDebugText?.Invoke(leftFastString);
            rightDebugText?.Invoke(rightFastString);

            leftText.text = leftFastString.ToString();
            rightText.text = rightFastString.ToString();
        }



        #region LabelValue
        public static void LabelValue(string label, string value, FastString fastString, bool line = false)
        {
            fastString.Append(label);
            fastString.Append(" - ");
            fastString.Append(value);

            if (line)
                fastString.Append("\n\n");
            else
                fastString.Append("\n");
        }

        public static void LabelValue(string label, int value, FastString fastString, bool line = false)
        {
            fastString.Append(label);
            fastString.Append(" - ");
            fastString.Append(value);

            if (line)
                fastString.Append("\n\n");
            else
                fastString.Append("\n");
        }

        public static void LabelValue(string label, float value, FastString fastString, bool line = false)
        {
            fastString.Append(label);
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
