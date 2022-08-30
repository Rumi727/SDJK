using SCKRM.Renderer;
using SCKRM.Resource;
using SCKRM.Sound;
using SCKRM.Threads;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace SCKRM.Editor
{
    public class SCKRMWindowEditor : EditorWindow
    {
        public static SCKRMWindowEditor instance { get; private set; }



        void OnEnable()
        {
            if (instance == null)
                instance = this;
            else
                Close();
        }

        bool inspectorUpdate = true;
        bool deleteSafety = true;
        int tabIndex = 0;
        void OnGUI()
        {
            {
                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                tabIndex = GUILayout.Toolbar(tabIndex, new string[] { "일반", "오디오", "NBS", "리소스" }, GUILayout.Width(300));

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if (Kernel.isPlayingAndNotPaused)
                {
                    EditorGUILayout.Space();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    GUILayout.Label("새로고침 딜레이", GUILayout.ExpandWidth(false));
                    inspectorUpdate = EditorGUILayout.Toggle(inspectorUpdate, GUILayout.Width(15));

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }

                CustomInspectorEditor.DrawLine(2);
            }

            switch (tabIndex)
            {
                case 1:
                    Audio();
                    break;
                case 2:
                    NBS();
                    break;
                case 3:
                    Resource();
                    break;

                default:
                    Default();
                    break;
            }
        }



        void OnInspectorUpdate()
        {
            if (inspectorUpdate && Kernel.isPlayingAndNotPaused)
                Repaint();
        }

        void Update()
        {
            if (!inspectorUpdate && Kernel.isPlayingAndNotPaused)
                Repaint();
        }

        public static void Default()
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

        string audioNameSpace = "";
        string audioKey = "";
        AudioClip audioClip;

        float audioVolume = 1;
        bool audioLoop = false;

        float audioPitch = 1;
        float audioTempo = 1;

        float audioPanStereo = 0;
        bool audioSpatial = false;

        float audioMinDistance = 0;
        float audioMaxDistance = 16;
        Vector3 audioLocalPosition = Vector3.zero;

        Vector2 audioScrollPos = Vector2.zero;
        public void Audio(int scrollYSize = 0)
        {
            EditorGUILayout.LabelField("제어판", EditorStyles.boldLabel);

            {
                {
                    {
                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Label("네임스페이스", GUILayout.ExpandWidth(false));
                        audioNameSpace = CustomInspectorEditor.DrawNameSpace(audioNameSpace);
                        GUILayout.Label("오디오 키", GUILayout.ExpandWidth(false));
                        audioKey = CustomInspectorEditor.DrawStringArray(audioKey, ResourceManager.GetSoundDataKeys(audioNameSpace));
                        GUILayout.Label("오디오 클립", GUILayout.ExpandWidth(false));
                        audioClip = (AudioClip)EditorGUILayout.ObjectField(audioClip, typeof(AudioClip), true);

                        if (!Kernel.isPlaying)
                            GUI.enabled = false;

                        GUI.enabled = true;

                        EditorGUILayout.EndHorizontal();
                    }
                    {
                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Label("볼륨", GUILayout.ExpandWidth(false));
                        audioVolume = EditorGUILayout.Slider(audioVolume, 0, 1);
                        GUILayout.Label("반복", GUILayout.ExpandWidth(false));
                        audioLoop = EditorGUILayout.Toggle(audioLoop, GUILayout.Width(15));

                        GUILayout.Label("피치", GUILayout.ExpandWidth(false));
                        audioPitch = EditorGUILayout.Slider(audioPitch, -3, 3);

                        if (SoundManager.Data.useTempo)
                        {
                            GUILayout.Label("템포", GUILayout.ExpandWidth(false));
                            audioTempo = EditorGUILayout.Slider(audioTempo, -3, 3);
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                    {
                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Label("3D", GUILayout.ExpandWidth(false));
                        audioSpatial = EditorGUILayout.Toggle(audioSpatial, GUILayout.Width(15));

                        if (audioSpatial)
                        {
                            GUILayout.Label("최소 거리", GUILayout.ExpandWidth(false));
                            audioMinDistance = EditorGUILayout.Slider(audioMinDistance, 0, 64);
                            GUILayout.Label("최대 거리", GUILayout.ExpandWidth(false));
                            audioMaxDistance = EditorGUILayout.Slider(audioMaxDistance, 0, 64);

                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();

                            GUILayout.Label("위치", GUILayout.ExpandWidth(false));
                            audioLocalPosition = EditorGUILayout.Vector3Field("", audioLocalPosition);

                            EditorGUILayout.EndHorizontal();
                        }
                        else
                        {
                            GUILayout.Label("스테레오", GUILayout.ExpandWidth(false));
                            audioPanStereo = EditorGUILayout.Slider(audioPanStereo, -1, 1);

                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }

                {
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Label($"{SoundManager.soundList.Count} / {SoundManager.maxSoundCount}", GUILayout.ExpandWidth(false));

                    if (!Kernel.isPlaying)
                        GUI.enabled = false;

                    if (GUILayout.Button("모든 음악 정지", GUILayout.ExpandWidth(false)))
                        SoundManager.StopSoundAll(true);
                    if (GUILayout.Button("모든 효과음 정지", GUILayout.ExpandWidth(false)))
                        SoundManager.StopSoundAll(false);
                    if (GUILayout.Button("모든 소리 정지", GUILayout.ExpandWidth(false)))
                        SoundManager.StopSoundAll();

                    EditorGUILayout.Space();

                    if (GUILayout.Button("오디오 리셋", GUILayout.ExpandWidth(false)))
                        ResourceManager.AudioReset().Forget();

                    bool audioPlay = GUILayout.Button("오디오 재생", GUILayout.ExpandWidth(false));
                    if (GUILayout.Button("오디오 정지", GUILayout.ExpandWidth(false)))
                        SoundManager.StopSound(audioKey, audioNameSpace);

                    if (audioPlay)
                    {
                        if (audioSpatial)
                            SoundManager.PlaySound(audioKey, audioNameSpace, audioVolume, audioLoop, audioPitch, audioTempo, audioPanStereo, audioMinDistance, audioMaxDistance, null, audioLocalPosition.x, audioLocalPosition.y, audioLocalPosition.z);
                        else
                            SoundManager.PlaySound(audioKey, audioNameSpace, audioVolume, audioLoop, audioPitch, audioTempo, audioPanStereo);
                    }

                    GUI.enabled = true;

                    EditorGUILayout.EndHorizontal();
                }
            }

            if (Kernel.isPlaying && InitialLoadManager.isInitialLoadEnd)
            {
                CustomInspectorEditor.DrawLine(2);

                EditorGUILayout.LabelField("재생 목록", EditorStyles.boldLabel);
                if (scrollYSize > 0)
                    audioScrollPos = EditorGUILayout.BeginScrollView(audioScrollPos, GUILayout.Height(scrollYSize));
                else
                    audioScrollPos = EditorGUILayout.BeginScrollView(audioScrollPos);

                for (int i = SoundManager.soundList.Count - 1; i >= 0; i--)
                {
                    SoundObjectEditor.GUI(SoundManager.soundList[i]);
                    CustomInspectorEditor.DrawLine(2);
                }

                EditorGUILayout.EndScrollView();
            }
        }

        string nbsNameSpace = "";
        string nbsKey = "";

        float nbsVolume = 1;
        bool nbsLoop = false;

        float nbsPitch = 1;
        float nbsTempo = 1;

        float nbsPanStereo = 0;
        bool nbsSpatial = false;

        float nbsMinDistance = 0;   
        float nbsMaxDistance = 48;
        Vector3 nbsLocalPosition = Vector3.zero;

        Vector2 nbsScrollPos = Vector2.zero;
        public void NBS(int scrollYSize = 0)
        {
            EditorGUILayout.LabelField("제어판", EditorStyles.boldLabel);

            {
                {
                    bool nbsPlay;
                    {
                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Label("네임스페이스", GUILayout.ExpandWidth(false));
                        nbsNameSpace = CustomInspectorEditor.DrawNameSpace(nbsNameSpace);
                        GUILayout.Label("NBS 키", GUILayout.ExpandWidth(false));
                        nbsKey = CustomInspectorEditor.DrawStringArray(nbsKey, ResourceManager.GetNBSDataKeys(nbsNameSpace));

                        if (!Kernel.isPlaying)
                            GUI.enabled = false;

                        nbsPlay = GUILayout.Button("NBS 재생", GUILayout.ExpandWidth(false));
                        if (GUILayout.Button("NBS 정지", GUILayout.ExpandWidth(false)))
                            SoundManager.StopNBS(nbsKey, nbsNameSpace);
                        if (GUILayout.Button("모든 NBS 정지", GUILayout.ExpandWidth(false)))
                            SoundManager.StopNBSAll();

                        GUI.enabled = true;

                        EditorGUILayout.EndHorizontal();
                    }
                    {
                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Label("볼륨", GUILayout.ExpandWidth(false));
                        nbsVolume = EditorGUILayout.Slider(nbsVolume, 0, 1);
                        GUILayout.Label("반복", GUILayout.ExpandWidth(false));
                        nbsLoop = EditorGUILayout.Toggle(nbsLoop, GUILayout.Width(15));

                        GUILayout.Label("피치", GUILayout.ExpandWidth(false));
                        nbsPitch = EditorGUILayout.Slider(nbsPitch, -3, 3);
                        GUILayout.Label("템포", GUILayout.ExpandWidth(false));
                        nbsTempo = EditorGUILayout.Slider(nbsTempo, -3, 3);

                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Label($"{SoundManager.nbsList.Count} / {SoundManager.maxNBSCount}", GUILayout.ExpandWidth(false));

                        GUILayout.Label("3D", GUILayout.ExpandWidth(false));
                        nbsSpatial = EditorGUILayout.Toggle(nbsSpatial, GUILayout.Width(15));

                        if (nbsSpatial)
                        {
                            GUILayout.Label("최소 거리", GUILayout.ExpandWidth(false));
                            nbsMinDistance = EditorGUILayout.Slider(nbsMinDistance, 0, 64);
                            GUILayout.Label("최대 거리", GUILayout.ExpandWidth(false));
                            nbsMaxDistance = EditorGUILayout.Slider(nbsMaxDistance, 0, 64);

                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();

                            GUILayout.Label("위치", GUILayout.ExpandWidth(false));
                            nbsLocalPosition = EditorGUILayout.Vector3Field("", nbsLocalPosition);

                            EditorGUILayout.EndHorizontal();
                        }
                        else
                        {
                            GUILayout.Label("스테레오", GUILayout.ExpandWidth(false));
                            nbsPanStereo = EditorGUILayout.Slider(nbsPanStereo, -1, 1);

                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    if (nbsPlay)
                    {
                        if (nbsSpatial)
                            SoundManager.PlayNBS(nbsKey, nbsNameSpace, nbsVolume, nbsLoop, nbsPitch, nbsTempo, nbsPanStereo, nbsMinDistance, nbsMaxDistance, null, nbsLocalPosition.x, nbsLocalPosition.y, nbsLocalPosition.z);
                        else
                            SoundManager.PlayNBS(nbsKey, nbsNameSpace, nbsVolume, nbsLoop, nbsPitch, nbsTempo, nbsPanStereo);
                    }
                }
            }

            if (Kernel.isPlaying && InitialLoadManager.isInitialLoadEnd)
            {
                CustomInspectorEditor.DrawLine(2);

                EditorGUILayout.LabelField("재생 목록", EditorStyles.boldLabel);

                if (scrollYSize > 0)
                    nbsScrollPos = EditorGUILayout.BeginScrollView(nbsScrollPos, GUILayout.Height(scrollYSize));
                else
                    nbsScrollPos = EditorGUILayout.BeginScrollView(nbsScrollPos);

                for (int i = SoundManager.nbsList.Count - 1; i >= 0; i--)
                {
                    NBSPlayerEditor.GUI(SoundManager.nbsList[i]);
                    CustomInspectorEditor.DrawLine(2);
                }

                EditorGUILayout.EndScrollView();
            }

            GUI.enabled = true;
        }

        Vector2 resourceScrollPos = Vector2.zero;
        public void Resource(int scrollYSize = 0)
        {
            GUILayout.Label("제어판", EditorStyles.boldLabel);

            {
                GUILayout.BeginHorizontal();

                if (!Kernel.isPlaying)
                    GUI.enabled = false;

                if (GUILayout.Button("텍스트 새로고침", GUILayout.ExpandWidth(false)))
                    RendererManager.AllTextRerender(true);

                if (GUILayout.Button("모든 리소스 새로고침", GUILayout.ExpandWidth(false)))
                    Kernel.AllRefresh().Forget();

                GUI.enabled = true;

                GUILayout.EndHorizontal();
            }

            if (Kernel.isPlaying && InitialLoadManager.isInitialLoadEnd)
            {
                CustomInspectorEditor.DrawLine(2);

                {
                    GUILayout.Label("리소스팩 리스트", EditorStyles.boldLabel);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("안전 삭제 모드 (삭제 할 리스트가 빈 값이 아니면 삭제 금지)", GUILayout.Width(330));
                    deleteSafety = EditorGUILayout.Toggle(deleteSafety);
                    EditorGUILayout.EndHorizontal();

                    //CustomInspectorEditor.DrawList(ResourceManager.resourcePacks, "리소스팩 경로", resourceScrollPos, deleteSafety);
                    DrawList(ResourceManager.SaveData.resourcePacks, "리소스팩 경로", deleteSafety);

                    void DrawList(List<string> list, string label, bool deleteSafety = true)
                    {
                        //GUI
                        {
                            EditorGUILayout.BeginHorizontal();

                            {
                                if (GUILayout.Button("추가", GUILayout.ExpandWidth(false)))
                                    list.Insert(0, "");
                            }

                            {
                                if (list.Count <= 0 || (list[0] != null && list[0] != "" && deleteSafety))
                                    GUI.enabled = false;

                                if (GUILayout.Button("삭제", GUILayout.ExpandWidth(false)) && list.Count > 0)
                                    list.RemoveAt(0);

                                GUI.enabled = true;
                            }

                            {
                                int count = EditorGUILayout.IntField("리스트 길이", list.Count, GUILayout.Height(21));
                                //변수 설정
                                if (count < 0)
                                    count = 0;

                                if (count > list.Count)
                                {
                                    for (int i = list.Count; i < count; i++)
                                        list.Insert(0, "");
                                }
                                else if (count < list.Count)
                                {
                                    for (int i = list.Count - 1; i >= count; i--)
                                    {
                                        if (list.Count > 0 && (list[0] == null || list[0] == "" || !deleteSafety))
                                            list.RemoveAt(0);
                                        else
                                            count++;
                                    }
                                }
                            }
                            
                            EditorGUILayout.EndHorizontal();
                        }

                        EditorGUILayout.Space();

                        {
                            if (scrollYSize > 0)
                                resourceScrollPos = EditorGUILayout.BeginScrollView(resourceScrollPos, GUILayout.Height(scrollYSize));
                            else
                                resourceScrollPos = EditorGUILayout.BeginScrollView(resourceScrollPos);

                            for (int i = 0; i < list.Count; i++)
                            {
                                if (i == list.Count - 1)
                                    GUI.enabled = false;

                                EditorGUILayout.BeginHorizontal();

                                GUILayout.Label(label, GUILayout.ExpandWidth(false));
                                list[i] = EditorGUILayout.TextField(list[i]);

                                {
                                    if (i - 1 < 0)
                                        GUI.enabled = false;

                                    if (GUILayout.Button("위로", GUILayout.ExpandWidth(false)))
                                        list.Move(i, i - 1);

                                    if (i != list.Count - 1)
                                        GUI.enabled = true;
                                }

                                {
                                    if (i + 1 >= list.Count - 1)
                                        GUI.enabled = false;

                                    if (GUILayout.Button("아래로", GUILayout.ExpandWidth(false)))
                                        list.Move(i, i + 1);

                                    if (i != list.Count - 1)
                                        GUI.enabled = true;
                                }

                                {
                                    if (i < list.Count - 1 && list[i] != null && list[i] != "")
                                        GUI.enabled = false;

                                    if (GUILayout.Button("삭제", GUILayout.ExpandWidth(false)))
                                        list.RemoveAt(i);

                                    GUI.enabled = true;
                                }

                                EditorGUILayout.EndHorizontal();
                            }

                            EditorGUILayout.EndScrollView();
                        }
                    }
                }
            }
        }
    }
}