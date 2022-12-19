using SCKRM.Threads;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace SCKRM.Editor
{
    public sealed class SCKRMWindowTabDefault : ISCKRMWindowTab
    {
        [InitializeOnLoadMethod] public static void Init() => SCKRMWindowEditor.TabAdd(new SCKRMWindowTabDefault());

        public string name => "일반";
        public int sortIndex => 0;

        public void OnGUI() => render();
        public static void Render(SCKRMWindowTabDefault window) => window.render();

        void render()
        {
            if (Kernel.isPlayingAndNotPaused)
            {
                EditorGUILayout.LabelField("델타 타임 - " + Kernel.deltaTime);
                EditorGUILayout.LabelField("FPS 델타 타임 - " + Kernel.fpsDeltaTime);
                EditorGUILayout.LabelField("스케일 되지 않은 델타 타임 - " + Kernel.unscaledDeltaTime);
                EditorGUILayout.LabelField("스케일 되지 않은 FPS 델타 타임 - " + Kernel.fpsUnscaledDeltaTime);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("FPS - " + Kernel.fps);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("총 할당된 메모리 (MB) - " + (Profiler.GetTotalAllocatedMemoryLong() / 1048576f).Round(4));

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("메인 스레드 ID - " + ThreadManager.mainThreadId);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("데이터 경로 - " + Kernel.dataPath);
                EditorGUILayout.LabelField("스트리밍 에셋 경로 - " + Kernel.streamingAssetsPath);
                EditorGUILayout.LabelField("영구 데이터 경로 - " + Kernel.persistentDataPath);
                EditorGUILayout.LabelField("임시 캐시 경로 - " + Kernel.temporaryCachePath);
                EditorGUILayout.LabelField("저장 데이터 경로 - " + Kernel.saveDataPath);
                EditorGUILayout.LabelField("리소스팩 경로 - " + Kernel.resourcePackPath);
                EditorGUILayout.LabelField("프로젝트 설정 경로 - " + Kernel.projectSettingPath);

                EditorGUILayout.LabelField("회사 이름 - " + Kernel.companyName);
                EditorGUILayout.LabelField("제품 이름 - " + Kernel.productName);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("버전 - " + Kernel.version);
                EditorGUILayout.LabelField("유니티 버전 - " + Kernel.unityVersion);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("플랫폼 - " + Kernel.platform);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("OS - " + SystemInfo.operatingSystem);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("장치 모델 - " + SystemInfo.deviceModel);
                EditorGUILayout.LabelField("장치 이름 - " + SystemInfo.deviceName);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("배터리 상태 - " + SystemInfo.batteryStatus);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("프로세서 유형 - " + SystemInfo.processorType);
                EditorGUILayout.LabelField("프로세서 주파수 - " + SystemInfo.processorFrequency);
                EditorGUILayout.LabelField("프로세서 수 - " + SystemInfo.processorCount);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("그래픽 장치 이름 - " + SystemInfo.graphicsDeviceName);
                EditorGUILayout.LabelField("그래픽 메모리 크기 (MB) - " + SystemInfo.graphicsMemorySize);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("시스템 메모리 크기 (MB) - " + SystemInfo.systemMemorySize);

                CustomInspectorEditor.DrawLine();

                Kernel.gameSpeed = EditorGUILayout.FloatField("게임 속도", Kernel.gameSpeed);
            }
            else
            {
                EditorGUILayout.LabelField("총 할당된 메모리 (MB) - " + (Profiler.GetTotalAllocatedMemoryLong() / 1048576f).Round(4));

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("메인 스레드 ID - " + ThreadManager.mainThreadId);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("데이터 경로 - " + Kernel.dataPath);
                EditorGUILayout.LabelField("스트리밍 에셋 경로 - " + Kernel.streamingAssetsPath);
                EditorGUILayout.LabelField("영구 데이터 경로 - " + Kernel.persistentDataPath);
                EditorGUILayout.LabelField("임시 캐시 경로 - " + Kernel.temporaryCachePath);
                EditorGUILayout.LabelField("저장 데이터 경로 - " + Kernel.saveDataPath);
                EditorGUILayout.LabelField("리소스팩 경로 - " + Kernel.resourcePackPath);
                EditorGUILayout.LabelField("프로젝트 설정 경로 - " + Kernel.projectSettingPath);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("회사 이름 - " + Kernel.companyName);
                EditorGUILayout.LabelField("제품 이름 - " + Kernel.productName);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("버전 - " + Kernel.version);
                EditorGUILayout.LabelField("유니티 버전 - " + Kernel.unityVersion);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("플랫폼 - " + Kernel.platform);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("OS - " + SystemInfo.operatingSystem);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("장치 모델 - " + SystemInfo.deviceModel);
                EditorGUILayout.LabelField("장치 이름 - " + SystemInfo.deviceName);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("배터리 상태 - " + SystemInfo.batteryStatus);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("프로세서 유형 - " + SystemInfo.processorType);
                EditorGUILayout.LabelField("프로세서 주파수 - " + SystemInfo.processorFrequency);
                EditorGUILayout.LabelField("프로세서 수 - " + SystemInfo.processorCount);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("그래픽 장치 이름 - " + SystemInfo.graphicsDeviceName);
                EditorGUILayout.LabelField("그래픽 메모리 크기 (MB) - " + SystemInfo.graphicsMemorySize);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("시스템 메모리 크기 (MB) - " + SystemInfo.systemMemorySize);
            }
        }
    }
}
