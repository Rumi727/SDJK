using SCKRM.Input;
using SCKRM.Json;
using SCKRM.Object;
using SCKRM.ProjectSetting;
using SCKRM.Renderer;
using SCKRM.Resource;
using SCKRM.SaveLoad;
using SCKRM.Sound;
using SCKRM.Splash;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
        int settingTabIndex = 0;
        void OnGUI()
        {
            {
                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                tabIndex = GUILayout.Toolbar(tabIndex, new string[] { "일반", "오디오", "NBS", "리소스", "프로젝트 설정" }, GUILayout.ExpandWidth(false));

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                GUILayout.Label("새로고침 딜레이", GUILayout.ExpandWidth(false));
                inspectorUpdate = EditorGUILayout.Toggle(inspectorUpdate, GUILayout.Width(15));

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                CustomInspectorEditor.DrawLine(2);
            }

            switch (tabIndex)
            {
                case 0:
                    Default();
                    break;
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
                    Setting();
                    break;
            }
        }



        void OnInspectorUpdate()
        {
            if (inspectorUpdate && Kernel.isPlaying)
                Repaint();
        }

        void Update()
        {
            if (!inspectorUpdate && Kernel.isPlaying)
                Repaint();
        }

        public static void Default()
        {
            if (Kernel.isPlaying)
            {
                EditorGUILayout.LabelField("델타 타임 - " + Kernel.deltaTime);
                EditorGUILayout.LabelField("FPS 델타 타임 - " + Kernel.fpsDeltaTime);
                EditorGUILayout.LabelField("스케일 되지 않은 델타 타임 - " + Kernel.unscaledDeltaTime);
                EditorGUILayout.LabelField("스케일 되지 않은 FPS 델타 타임 - " + Kernel.fpsUnscaledDeltaTime);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("FPS - " + Kernel.fps);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("데이터 경로 - " + Kernel.dataPath);
                EditorGUILayout.LabelField("스트리밍 에셋 경로 - " + Kernel.streamingAssetsPath);
                EditorGUILayout.LabelField("영구 데이터 경로 - " + Kernel.persistentDataPath);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("회사 이름 - " + Kernel.companyName);
                EditorGUILayout.LabelField("제품 이름 - " + Kernel.productName);
                EditorGUILayout.LabelField("버전 - " + Kernel.version);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("실행 중인 플랫폼 - " + Kernel.platform);

                CustomInspectorEditor.DrawLine();

                Kernel.gameSpeed = EditorGUILayout.FloatField("게임 속도", Kernel.gameSpeed);
            }
            else
            {
                EditorGUILayout.LabelField("데이터 경로 - " + Kernel.dataPath);
                EditorGUILayout.LabelField("스트리밍 에셋 경로 - " + Kernel.streamingAssetsPath);
                EditorGUILayout.LabelField("영구 데이터 경로 - " + Kernel.persistentDataPath);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("회사 이름 - " + Kernel.companyName);
                EditorGUILayout.LabelField("제품 이름 - " + Kernel.productName);
                EditorGUILayout.LabelField("버전 - " + Kernel.version);

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.LabelField("실행 중인 플랫폼 - " + Kernel.platform);

                _ = "asdf";
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

        void Setting()
        {
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                settingTabIndex = GUILayout.Toolbar(settingTabIndex, new string[] { "스플래시 설정", "비디오 설정", "조작 키 설정", "풀링 설정", "오디오 설정", "NBS 설정", "리소스 설정" }, GUILayout.ExpandWidth(false));

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                CustomInspectorEditor.DrawLine(2);
            }

            switch (settingTabIndex)
            {
                case 0:
                    SplashSetting();
                    break;
                case 1:
                    VideoSetting();
                    break;
                case 2:
                    ControlSetting();
                    break;
                case 3:
                    ObjectPoolingSetting();
                    break;
                case 4:
                    AudioSetting();
                    break;
                case 5:
                    NBSSetting();
                    break;
                case 6:
                    ResourceSetting();
                    break;
            }
        }

        public static SaveLoadClass splashProjectSetting = null;
        public void SplashSetting()
        {
            if (!Kernel.isPlaying)
            {
                if (splashProjectSetting == null)
                    SaveLoadManager.Initialize<ProjectSettingSaveLoadAttribute>(typeof(SplashScreen.Data), out splashProjectSetting);
                
                SaveLoadManager.Load(splashProjectSetting, Kernel.projectSettingPath);
            }


            //GUI
            {
                EditorGUILayout.LabelField("스플래시 설정", EditorStyles.boldLabel);
                EditorGUILayout.Space();
            }

            string path = SplashScreen.Data.splashScreenPath;
            string name = SplashScreen.Data.splashScreenName;
            ObjectField<SceneAsset>("재생 할 스플래시 씬", ".unity", ref path, ref name, out bool isChanged);
            SplashScreen.Data.splashScreenPath = path;
            SplashScreen.Data.splashScreenName = name;

            if (isChanged)
                SCKRMSetting.SceneListChanged(false);

            EditorGUILayout.Space();

            path = SplashScreen.Data.kernelObjectPath;
            name = SplashScreen.Data.kernelObjectName;
            ObjectField<Kernel>("사용 될 커널 프리팹", ".prefab", ref path, ref name, out isChanged);
            SplashScreen.Data.kernelObjectPath = path;
            SplashScreen.Data.kernelObjectName = name;

            if (isChanged)
                SCKRMSetting.SceneListChanged(false);


            static void ObjectField<T>(string label, string extension, ref string path, ref string name, out bool isChanged) where T : UnityEngine.Object
            {
                T oldAssets = AssetDatabase.LoadAssetAtPath<T>(PathTool.Combine(path, name) + extension);
                T assets = (T)EditorGUILayout.ObjectField(label, oldAssets, typeof(T), false);

                {
                    string allAssetPath = AssetDatabase.GetAssetPath(assets);
                    if (allAssetPath != "")
                    {
                        string assetPath = allAssetPath.Substring(0, allAssetPath.LastIndexOf("/"));
                        string assetName = allAssetPath.Remove(0, allAssetPath.LastIndexOf("/") + 1);
                        assetName = assetName.Substring(0, assetName.Length - extension.Length);

                        path = assetPath;
                        name = assetName;
                    }
                }

                EditorGUILayout.LabelField($"경로: {PathTool.Combine(path, name) + extension}");
                isChanged = oldAssets != assets;
            }

            //플레이 모드가 아니면 변경한 리스트의 데이터를 잃어버리지 않게 파일로 저장
            if (GUI.changed && !Kernel.isPlaying)
                SaveLoadManager.Save(splashProjectSetting, Kernel.projectSettingPath);
        }

        public static SaveLoadClass videoProjectSetting = null;
        public void VideoSetting()
        {
            if (!Kernel.isPlaying)
            {
                if (videoProjectSetting == null)
                    SaveLoadManager.Initialize<ProjectSettingSaveLoadAttribute>(typeof(VideoManager.Data), out videoProjectSetting);

                SaveLoadManager.Load(videoProjectSetting, Kernel.projectSettingPath);
            }


            //GUI
            {
                EditorGUILayout.LabelField("비디오 설정", EditorStyles.boldLabel);
                EditorGUILayout.Space();
            }

            {
                VideoManager.Data.standardFPS = EditorGUILayout.FloatField("기준 프레임", VideoManager.Data.standardFPS);
                VideoManager.Data.notFocusFpsLimit = EditorGUILayout.IntField("포커스가 아닐때 FPS 제한", VideoManager.Data.notFocusFpsLimit);

                VideoManager.Data.standardFPS = VideoManager.Data.standardFPS;
                VideoManager.Data.notFocusFpsLimit = VideoManager.Data.notFocusFpsLimit;
            }

            //플레이 모드가 아니면 변경한 리스트의 데이터를 잃어버리지 않게 파일로 저장
            if (GUI.changed && !Kernel.isPlaying)
                SaveLoadManager.Save(videoProjectSetting, Kernel.projectSettingPath);
        }

        public static SaveLoadClass controlProjectSetting;
        Vector2 controlSettingScrollPos = Vector2.zero;
        Vector2 controlLockSettingScrollPos = Vector2.zero;
        public void ControlSetting(int scrollYSize = 0)
        {
            if (!Kernel.isPlaying)
            {
                if (controlProjectSetting == null)
                    SaveLoadManager.Initialize<ProjectSettingSaveLoadAttribute>(typeof(InputManager.Data), out controlProjectSetting);

                SaveLoadManager.Load(controlProjectSetting, Kernel.projectSettingPath);
            }

            if (InputManager.Data.controlSettingList == null)
                InputManager.Data.controlSettingList = new Dictionary<string, List<KeyCode>>();
            if (InputManager.Data.inputLockList == null)
                InputManager.Data.inputLockList = new Dictionary<string, bool>();



            //GUI
            {
                EditorGUILayout.LabelField("조작 설정", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("안전 삭제 모드 (삭제 할 리스트가 빈 값이 아니면 삭제 금지)", GUILayout.Width(330));
                deleteSafety = EditorGUILayout.Toggle(deleteSafety);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }

            //Control Setting List
            {
                //GUI
                {
                    EditorGUILayout.BeginHorizontal();

                    {
                        if (InputManager.Data.controlSettingList.ContainsKey(""))
                            GUI.enabled = false;

                        if (GUILayout.Button("추가", GUILayout.ExpandWidth(false)))
                            InputManager.Data.controlSettingList.Add("", new List<KeyCode>());

                        GUI.enabled = true;
                    }

                    {
                        if (InputManager.Data.controlSettingList.Count <= 0 || ((InputManager.Data.controlSettingList.Keys.ToList()[InputManager.Data.controlSettingList.Count - 1] != "" || InputManager.Data.controlSettingList.Values.ToList()[InputManager.Data.controlSettingList.Count - 1].Count != 0) && deleteSafety))
                            GUI.enabled = false;

                        if (GUILayout.Button("삭제", GUILayout.ExpandWidth(false)) && InputManager.Data.controlSettingList.Count > 0)
                            InputManager.Data.controlSettingList.Remove(InputManager.Data.controlSettingList.ToList()[InputManager.Data.controlSettingList.Count - 1].Key);

                        GUI.enabled = true;
                    }

                    {
                        int count = EditorGUILayout.IntField("리스트 길이", InputManager.Data.controlSettingList.Count, GUILayout.Height(21));
                        //변수 설정
                        if (count < 0)
                            count = 0;

                        if (count > InputManager.Data.controlSettingList.Count)
                        {
                            for (int i = InputManager.Data.controlSettingList.Count; i < count; i++)
                            {
                                if (!InputManager.Data.controlSettingList.ContainsKey(""))
                                    InputManager.Data.controlSettingList.Add("", new List<KeyCode>());
                                else
                                    count--;
                            }
                        }
                        else if (count < InputManager.Data.controlSettingList.Count)
                        {
                            for (int i = InputManager.Data.controlSettingList.Count - 1; i >= count; i--)
                            {
                                if ((InputManager.Data.controlSettingList.Keys.ToList()[InputManager.Data.controlSettingList.Count - 1] == "" && InputManager.Data.controlSettingList.Values.ToList()[InputManager.Data.controlSettingList.Count - 1].Count == 0) || !deleteSafety)
                                    InputManager.Data.controlSettingList.Remove(InputManager.Data.controlSettingList.ToList()[InputManager.Data.controlSettingList.Count - 1].Key);
                                else
                                    count++;
                            }
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();


                if (Kernel.isPlaying)
                    EditorGUILayout.HelpBox("플레이 모드에서 바꾼 (인게임 설정에서 바꾼) 조작은 반영되지 않고, 저장되지 않습니다\n기본값만 저장되고 변경됩니다 (키를 초기화한 상태라면, 변경한 키는 인게임에도 적용됩니다)", MessageType.Warning);

                {
                    if (scrollYSize > 0)
                        controlSettingScrollPos = EditorGUILayout.BeginScrollView(controlSettingScrollPos, GUILayout.Height(scrollYSize));
                    else
                        controlSettingScrollPos = EditorGUILayout.BeginScrollView(controlSettingScrollPos, GUILayout.ExpandHeight(false));

                    CustomInspectorEditor.DrawLine();

                    List<KeyValuePair<string, List<KeyCode>>> controlList = InputManager.Data.controlSettingList.ToList();

                    int up = -1;
                    int down = -1;
                    int delete = -1;

                    //딕셔너리는 키를 수정할수 없기때문에, 리스트로 분활해줘야함
                    List<string> keyList = new List<string>();
                    List<List<KeyCode>> valueList = new List<List<KeyCode>>();

                    for (int i = 0; i < InputManager.Data.controlSettingList.Count; i++)
                    {
                        KeyValuePair<string, List<KeyCode>> item = controlList[i];

                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Space(30);

                        GUILayout.Label("키 코드 키", GUILayout.ExpandWidth(false));
                        keyList.Add(EditorGUILayout.TextField(item.Key));

                        {
                            if (i - 1 < 0)
                                GUI.enabled = false;

                            if (GUILayout.Button("위로", GUILayout.ExpandWidth(false)))
                                up = i;

                            GUI.enabled = true;
                        }

                        {
                            if (i + 1 >= InputManager.Data.controlSettingList.Count)
                                GUI.enabled = false;

                            if (GUILayout.Button("아래로", GUILayout.ExpandWidth(false)))
                                down = i;

                            GUI.enabled = true;
                        }

                        {
                            if (keyList[i] != "" || (InputManager.Data.controlSettingList.ContainsKey(keyList[i]) && InputManager.Data.controlSettingList[keyList[i]].Count != 0) && deleteSafety)
                                GUI.enabled = false;

                            if (GUILayout.Button("삭제", GUILayout.ExpandWidth(false)))
                                delete = i;

                            GUI.enabled = true;
                        }



                        EditorGUILayout.EndHorizontal();

                        //리스트
                        {
                            List<KeyCode> keyCodes = item.Value;

                            {
                                CustomInspectorEditor.DrawList(keyCodes, "키 코드", enumPopup, 1, 1, deleteSafety);
                                static KeyCode enumPopup(KeyCode value) => (KeyCode)EditorGUILayout.EnumPopup(value);
                            }

                            valueList.Add(keyCodes);
                        }

                        if (i != InputManager.Data.controlSettingList.Count - 1)
                            CustomInspectorEditor.DrawLine();
                    }

                    EditorGUILayout.EndScrollView();

                    if (up >= 0)
                    {
                        keyList.Move(up, up - 1);
                        valueList.Move(up, up - 1);
                    }
                    else if (down >= 0)
                    {
                        keyList.Move(down, down + 1);
                        valueList.Move(down, down + 1);
                    }
                    else if (delete >= 0)
                    {
                        keyList.RemoveAt(delete);
                        valueList.RemoveAt(delete);
                    }

                    //키 중복 감지
                    bool overlap = keyList.Count != keyList.Distinct().Count();
                    if (!overlap)
                    {
                        //리스트 2개를 딕셔너리로 변환
                        InputManager.Data.controlSettingList = keyList.Zip(valueList, (key, value) => new { key, value }).ToDictionary(a => a.key, a => a.value);
                    }
                }
            }

            CustomInspectorEditor.DrawLine(2);

            //Input Lock Setting List
            {
                EditorGUILayout.LabelField("잠금 설정", EditorStyles.boldLabel);

                //GUI
                {
                    EditorGUILayout.BeginHorizontal();

                    {
                        if (InputManager.Data.inputLockList.ContainsKey(""))
                            GUI.enabled = false;

                        if (GUILayout.Button("추가", GUILayout.ExpandWidth(false)))
                            InputManager.Data.inputLockList.Add("", false);

                        GUI.enabled = true;
                    }

                    {
                        if (InputManager.Data.inputLockList.Count <= 0 || ((InputManager.Data.inputLockList.Keys.ToList()[InputManager.Data.inputLockList.Count - 1] != "" || InputManager.Data.inputLockList.Values.ToList()[InputManager.Data.inputLockList.Count - 1] != false) && deleteSafety))
                            GUI.enabled = false;

                        if (GUILayout.Button("삭제", GUILayout.ExpandWidth(false)) && InputManager.Data.inputLockList.Count > 0)
                            InputManager.Data.inputLockList.Remove(InputManager.Data.inputLockList.ToList()[InputManager.Data.inputLockList.Count - 1].Key);

                        GUI.enabled = true;
                    }

                    {
                        int count = EditorGUILayout.IntField("리스트 길이", InputManager.Data.inputLockList.Count, GUILayout.Height(21));
                        //변수 설정
                        if (count < 0)
                            count = 0;

                        if (count > InputManager.Data.inputLockList.Count)
                        {
                            for (int i = InputManager.Data.inputLockList.Count; i < count; i++)
                            {
                                if (!InputManager.Data.inputLockList.ContainsKey(""))
                                    InputManager.Data.inputLockList.Add("", false);
                                else
                                    count--;
                            }
                        }
                        else if (count < InputManager.Data.inputLockList.Count)
                        {
                            for (int i = InputManager.Data.inputLockList.Count - 1; i >= count; i--)
                            {
                                if ((InputManager.Data.inputLockList.Keys.ToList()[InputManager.Data.inputLockList.Count - 1] == "" && InputManager.Data.inputLockList.Values.ToList()[InputManager.Data.inputLockList.Count - 1] == false) || !deleteSafety)
                                    InputManager.Data.inputLockList.Remove(InputManager.Data.inputLockList.ToList()[InputManager.Data.inputLockList.Count - 1].Key);
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
                        controlLockSettingScrollPos = EditorGUILayout.BeginScrollView(controlLockSettingScrollPos, GUILayout.Height(scrollYSize));
                    else
                        controlLockSettingScrollPos = EditorGUILayout.BeginScrollView(controlLockSettingScrollPos, GUILayout.ExpandHeight(false));

                    List<KeyValuePair<string, bool>> inputLockList = InputManager.Data.inputLockList.ToList();

                    //딕셔너리는 키를 수정할수 없기때문에, 리스트로 분활해줘야함
                    List<string> keyList = new List<string>();
                    List<bool> valueList = new List<bool>();

                    for (int i = 0; i < InputManager.Data.inputLockList.Count; i++)
                    {
                        KeyValuePair<string, bool> item = inputLockList[i];

                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Label("잠금 키", GUILayout.ExpandWidth(false));
                        keyList.Add(EditorGUILayout.TextField(item.Key));

                        GUILayout.Label("잠금", GUILayout.ExpandWidth(false));
                        valueList.Add(EditorGUILayout.Toggle(item.Value));

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndScrollView();

                    //키 중복 감지
                    bool overlap = keyList.Count != keyList.Distinct().Count();
                    if (!overlap)
                    {
                        //리스트 2개를 딕셔너리로 변환
                        InputManager.Data.inputLockList = keyList.Zip(valueList, (key, value) => new { key, value }).ToDictionary(a => a.key, a => a.value);
                    }
                }
            }

            //플레이 모드가 아니면 변경한 리스트의 데이터를 잃어버리지 않게 파일로 저장
            if (GUI.changed && !Kernel.isPlaying)
                SaveLoadManager.Save(controlProjectSetting, Kernel.projectSettingPath);
        }

        public static SaveLoadClass objectPoolingProjectSetting = null;
        Vector2 objectPoolingSettingScrollPos = Vector2.zero;
        public void ObjectPoolingSetting(int scrollYSize = 0)
        {
            if (!Kernel.isPlaying)
            {
                if (objectPoolingProjectSetting == null)
                    SaveLoadManager.Initialize<ProjectSettingSaveLoadAttribute>(typeof(ObjectPoolingSystem.Data), out objectPoolingProjectSetting);

                SaveLoadManager.Load(objectPoolingProjectSetting, Kernel.projectSettingPath);
            }

            if (ObjectPoolingSystem.Data.prefabList == null)
                ObjectPoolingSystem.Data.prefabList = new Dictionary<string, string>();

            //GUI
            {
                EditorGUILayout.LabelField("오브젝트 리스트", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("안전 삭제 모드 (삭제 할 리스트가 빈 값이 아니면 삭제 금지)", GUILayout.Width(330));
                deleteSafety = EditorGUILayout.Toggle(deleteSafety);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }

            //GUI
            {
                EditorGUILayout.BeginHorizontal();

                {
                    if (ObjectPoolingSystem.Data.prefabList.ContainsKey(""))
                        GUI.enabled = false;

                    if (GUILayout.Button("추가", GUILayout.ExpandWidth(false)))
                        ObjectPoolingSystem.Data.prefabList.Add("", "");

                    GUI.enabled = true;
                }

                {
                    if (ObjectPoolingSystem.Data.prefabList.Count <= 0 || ((ObjectPoolingSystem.Data.prefabList.Keys.ToList()[ObjectPoolingSystem.Data.prefabList.Count - 1] != "" || ObjectPoolingSystem.Data.prefabList.Values.ToList()[ObjectPoolingSystem.Data.prefabList.Count - 1] != "") && deleteSafety))
                        GUI.enabled = false;

                    if (GUILayout.Button("삭제", GUILayout.ExpandWidth(false)))
                        ObjectPoolingSystem.Data.prefabList.Remove(ObjectPoolingSystem.Data.prefabList.ToList()[ObjectPoolingSystem.Data.prefabList.Count - 1].Key);

                    GUI.enabled = true;
                }

                {
                    int count = EditorGUILayout.IntField("리스트 길이", ObjectPoolingSystem.Data.prefabList.Count, GUILayout.Height(21));

                    //변수 설정
                    if (count < 0)
                        count = 0;

                    if (count > ObjectPoolingSystem.Data.prefabList.Count)
                    {
                        for (int i = ObjectPoolingSystem.Data.prefabList.Count; i < count; i++)
                        {
                            if (!ObjectPoolingSystem.Data.prefabList.ContainsKey(""))
                                ObjectPoolingSystem.Data.prefabList.Add("", "");
                            else
                                count--;
                        }
                    }
                    else if (count < ObjectPoolingSystem.Data.prefabList.Count)
                    {
                        for (int i = ObjectPoolingSystem.Data.prefabList.Count - 1; i >= count; i--)
                        {
                            if ((ObjectPoolingSystem.Data.prefabList.Keys.ToList()[ObjectPoolingSystem.Data.prefabList.Count - 1] == "" && ObjectPoolingSystem.Data.prefabList.Values.ToList()[ObjectPoolingSystem.Data.prefabList.Count - 1] == "") || !deleteSafety)
                                ObjectPoolingSystem.Data.prefabList.Remove(ObjectPoolingSystem.Data.prefabList.ToList()[ObjectPoolingSystem.Data.prefabList.Count - 1].Key);
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
                    objectPoolingSettingScrollPos = EditorGUILayout.BeginScrollView(objectPoolingSettingScrollPos, GUILayout.Height(scrollYSize));
                else
                    objectPoolingSettingScrollPos = EditorGUILayout.BeginScrollView(objectPoolingSettingScrollPos);

                //PrefabObject의 <string, string>를 <string, GameObject>로 바꿔서 인스펙터에 보여주고 인스펙터에서 변경한걸 <string, string>로 다시 바꿔서 PrefabObject에 저장
                /*
                 * 왜 이렇게 변환하냐면 JSON에 오브젝트를 저장할려면 우선적으로 string 값같은 경로가 있어야하고
                   인스펙터에서 쉽게 드래그로 오브젝트를 바꾸기 위해선
                   GameObject 형식이여야해서 이런 소용돌이가 나오게 된것
                */
                List<KeyValuePair<string, string>> prefabObject = ObjectPoolingSystem.Data.prefabList.ToList();

                //딕셔너리는 키를 수정할수 없기때문에, 리스트로 분활해줘야함
                List<string> keyList = new List<string>();
                List<string> valueList = new List<string>();

                for (int i = 0; i < ObjectPoolingSystem.Data.prefabList.Count; i++)
                {
                    KeyValuePair<string, string> item = prefabObject[i];

                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Label("프리팹 키", GUILayout.ExpandWidth(false));
                    string key = EditorGUILayout.TextField(item.Key);

                    GUILayout.Label("프리팹", GUILayout.ExpandWidth(false));
                    //문자열(경로)을 프리팹으로 변환
                    GameObject gameObject = null;
                    GameObject loadedGameObject = Resources.Load<GameObject>(item.Value);
                    IObjectPooling objectPooling = null;
                    MonoBehaviour monoBehaviour = null;
                    
                    loadedGameObject = (GameObject)EditorGUILayout.ObjectField("", loadedGameObject, typeof(GameObject), true);

                    if (loadedGameObject != null)
                    {
                        objectPooling = loadedGameObject.GetComponent<IObjectPooling>();
                        if (objectPooling != null)
                            monoBehaviour = (MonoBehaviour)objectPooling;
                    }

                    if (objectPooling != null)
                        gameObject = monoBehaviour.gameObject;

                    /*
                     * 변경한 프리팹이 리소스 폴더에 있지 않은경우
                       저장은 되지만 프리팹을 감지할수 없기때문에
                       조건문으로 경고를 표시해주고
                       경로가 중첩되는 현상을 대비하기 위해 경로를 빈 문자열로 변경해줌
                     */
                    if (deleteSafety && key != null && key != "")
                        GUI.enabled = false;

                    if (!GUILayout.Button("삭제", GUILayout.ExpandWidth(false)))
                    {
                        string assetsPath = AssetDatabase.GetAssetPath(gameObject);
                        if (assetsPath.Contains("Resources/"))
                        {
                            keyList.Add(key);

                            assetsPath = assetsPath.Substring(assetsPath.IndexOf("Resources/") + 10);
                            assetsPath = assetsPath.Remove(assetsPath.LastIndexOf("."));

                            valueList.Add(assetsPath);

                            EditorGUILayout.EndHorizontal();
                        }
                        else
                        {
                            keyList.Add(key);
                            valueList.Add("");

                            EditorGUILayout.EndHorizontal();

                            GUI.enabled = true;
                            EditorGUILayout.HelpBox("'Resources' 폴더에 있고 IObjectPooling 인터페이스를 상속받는 스크립트가 최상단에 포함된 오브젝트를 넣어주세요", MessageType.Info);
                        }
                    }
                    else
                        EditorGUILayout.EndHorizontal();

                    GUI.enabled = true;
                }

                EditorGUILayout.EndScrollView();

                //키 중복 감지
                bool overlap = keyList.Count != keyList.Distinct().Count();
                if (!overlap)
                {
                    //리스트 2개를 딕셔너리로 변환
                    ObjectPoolingSystem.Data.prefabList = keyList.Zip(valueList, (key, value) => new { key, value }).ToDictionary(a => a.key, a => a.value);
                }
            }

            //플레이 모드가 아니면 변경한 리스트의 데이터를 잃어버리지 않게 파일로 저장
            if (GUI.changed && !Kernel.isPlaying)
                SaveLoadManager.Save(objectPoolingProjectSetting, Kernel.projectSettingPath);
        }

        public static SaveLoadClass audioProjectSetting = null;
        string audioSettingNameSpace = "";
        Vector2 audioSettingScrollPos = Vector2.zero;
        public void AudioSetting(int scrollYSize = 0)
        {
            if (!Kernel.isPlaying)
            {
                if (audioProjectSetting == null)
                    SaveLoadManager.Initialize<ProjectSettingSaveLoadAttribute>(typeof(SoundManager.Data), out audioProjectSetting);

                SaveLoadManager.Load(audioProjectSetting, Kernel.projectSettingPath);
            }

            //GUI
            {
                EditorGUILayout.LabelField("오디오 설정", EditorStyles.boldLabel);

                {
                    if (Kernel.isPlaying)
                        GUI.enabled = false;

                    SoundManager.Data.useTempo = EditorGUILayout.Toggle("템포 기능 사용", SoundManager.Data.useTempo);
                }

                GUI.enabled = true;

                CustomInspectorEditor.DrawLine();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("안전 삭제 모드 (삭제 할 리스트가 빈 값이 아니면 삭제 금지)", GUILayout.Width(330));
                deleteSafety = EditorGUILayout.Toggle(deleteSafety);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }

            EditorGUILayout.BeginHorizontal();

            audioSettingNameSpace = CustomInspectorEditor.DrawNameSpace("네임스페이스", audioSettingNameSpace);

            string nameSpace = audioSettingNameSpace;
            string path = PathTool.Combine(Kernel.streamingAssetsPath, ResourceManager.soundPath.Replace("%NameSpace%", nameSpace));

            if (Kernel.isPlaying)
                GUI.enabled = false;

            if (!Directory.Exists(path))
            {
                if (GUILayout.Button("sounds 폴더 만들기", GUILayout.ExpandWidth(false)))
                {
                    Directory.CreateDirectory(path);
                    AssetDatabase.Refresh();
                }

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                string jsonPath = path + ".json";
                if (!File.Exists(jsonPath))
                {
                    if (GUILayout.Button("sounds.json 파일 만들기", GUILayout.ExpandWidth(false)))
                    {
                        File.WriteAllText(jsonPath, "{}");
                        AssetDatabase.Refresh();
                    }

                    {
                        if (Directory.GetFiles(path).Length > 0 && deleteSafety)
                            GUI.enabled = false;

                        if (GUILayout.Button("sounds 폴더 지우기", GUILayout.ExpandWidth(false)))
                        {
                            Directory.Delete(path, true);
                            File.Delete(path + ".meta");
                            AssetDatabase.Refresh();
                            EditorGUILayout.EndHorizontal();
                            return;
                        }

                        if (!Kernel.isPlaying)
                            GUI.enabled = true;
                    }

                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    Dictionary<string, SoundData<SoundMetaData>> soundDatas = JsonManager.JsonRead<Dictionary<string, SoundData<SoundMetaData>>>(jsonPath, true);

                    if (soundDatas == null)
                        soundDatas = new Dictionary<string, SoundData<SoundMetaData>>();

                    {
                        if (soundDatas.Count > 0 && deleteSafety)
                            GUI.enabled = false;

                        if (GUILayout.Button("sounds.json 파일 지우기", GUILayout.ExpandWidth(false)))
                        {
                            File.Delete(jsonPath);
                            File.Delete(jsonPath + ".meta");
                            AssetDatabase.Refresh();
                            EditorGUILayout.EndHorizontal();
                            return;
                        }

                        if (!Kernel.isPlaying)
                            GUI.enabled = true;
                    }

                    {
                        if (Directory.GetFiles(path).Length > 0 && deleteSafety)
                            GUI.enabled = false;

                        if (GUILayout.Button("sounds 폴더 지우기", GUILayout.ExpandWidth(false)))
                        {
                            Directory.Delete(path, true);
                            File.Delete(path + ".meta");
                            AssetDatabase.Refresh();
                            EditorGUILayout.EndHorizontal();
                            return;
                        }

                        if (!Kernel.isPlaying)
                            GUI.enabled = true;
                    }

                    EditorGUILayout.EndHorizontal();

                    //GUI
                    {
                        EditorGUILayout.BeginHorizontal();

                        {
                            if (soundDatas.ContainsKey(""))
                                GUI.enabled = false;

                            if (GUILayout.Button("추가", GUILayout.ExpandWidth(false)))
                                soundDatas.Add("", new SoundData<SoundMetaData>("", false, new SoundMetaData[0]));

                            if (!Kernel.isPlaying)
                                GUI.enabled = true;
                        }

                        {
                            if (soundDatas.Count > 0)
                            {
                                SoundData<SoundMetaData> soundData = soundDatas.Values.ToList()[soundDatas.Count - 1];
                                if ((soundDatas.Keys.ToList()[soundDatas.Count - 1] != "" || soundData.subtitle != "" || soundData.sounds == null || soundData.sounds.Count() > 0) && deleteSafety)
                                    GUI.enabled = false;
                            }
                            else
                                GUI.enabled = false;

                            if (GUILayout.Button("삭제", GUILayout.ExpandWidth(false)) && soundDatas.Count > 0)
                                soundDatas.Remove(soundDatas.ToList()[soundDatas.Count - 1].Key);

                            if (!Kernel.isPlaying)
                                GUI.enabled = true;
                        }

                        {
                            int count = EditorGUILayout.IntField("리스트 길이", soundDatas.Count, GUILayout.Height(21));
                            //변수 설정
                            if (count < 0)
                                count = 0;

                            if (count > soundDatas.Count)
                            {
                                for (int i = soundDatas.Count; i < count; i++)
                                {
                                    if (!soundDatas.ContainsKey(""))
                                        soundDatas.Add("", new SoundData<SoundMetaData>("", false, new SoundMetaData[0]));
                                    else
                                        count--;
                                }
                            }
                            else if (count < soundDatas.Count)
                            {
                                SoundData<SoundMetaData> soundData = soundDatas.Values.ToList()[soundDatas.Count - 1];
                                for (int i = soundDatas.Count - 1; i >= count; i--)
                                {
                                    if ((soundDatas.Count > 0 && soundDatas.Keys.ToList()[soundDatas.Count - 1] == "" && soundData.subtitle == "" && soundData.sounds != null && soundData.sounds.Count() <= 0) || !deleteSafety)
                                        soundDatas.Remove(soundDatas.ToList()[soundDatas.Count - 1].Key);
                                    else
                                        count++;
                                }
                            }
                        }

                        EditorGUILayout.EndHorizontal();
                    }



                    {
                        {
                            GUI.enabled = true;

                            if (scrollYSize > 0)
                                audioSettingScrollPos = EditorGUILayout.BeginScrollView(audioSettingScrollPos, GUILayout.Height(scrollYSize));
                            else
                                audioSettingScrollPos = EditorGUILayout.BeginScrollView(audioSettingScrollPos);

                            if (Kernel.isPlaying)
                                GUI.enabled = false;
                        }

                        //GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, false, 0, Color.white * 0.25f, 0, 0);

                        List<KeyValuePair<string, SoundData<SoundMetaData>>> tempSoundDatas = soundDatas.ToList();
                        //딕셔너리는 키를 수정할수 없기때문에, 리스트로 분활해줘야함
                        List<string> keyList = new List<string>();
                        List<SoundData<SoundMetaData>> valueList = new List<SoundData<SoundMetaData>>();

                        for (int i = 0; i < soundDatas.Count; i++)
                        {
                            KeyValuePair<string, SoundData<SoundMetaData>> soundData = tempSoundDatas[i];

                            CustomInspectorEditor.DrawLine();

                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(30);

                            GUILayout.Label("오디오 키", GUILayout.ExpandWidth(false));
                            keyList.Add(EditorGUILayout.TextField(soundData.Key));

                            GUILayout.Label("자막", GUILayout.ExpandWidth(false));
                            string subtitle = EditorGUILayout.TextField(soundData.Value.subtitle);

                            GUILayout.Label("BGM", GUILayout.ExpandWidth(false));
                            bool isBGM = EditorGUILayout.Toggle(soundData.Value.isBGM, GUILayout.Width(15));

                            EditorGUILayout.EndHorizontal();


                            //리스트 안의 리스트
                            {
                                List<SoundMetaData> soundMetaDatas = soundData.Value.sounds.ToList();
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    GUILayout.Space(30);

                                    {
                                        if (GUILayout.Button("추가", GUILayout.ExpandWidth(false)))
                                            soundMetaDatas.Add(new SoundMetaData("", 1, 1, false, 0, null));
                                    }

                                    {
                                        if (soundMetaDatas.Count <= 0 || (soundMetaDatas[soundMetaDatas.Count - 1].path != "" && deleteSafety))
                                            GUI.enabled = false;

                                        if (GUILayout.Button("삭제", GUILayout.ExpandWidth(false)) && soundMetaDatas.Count > 0)
                                            soundMetaDatas.RemoveAt(soundMetaDatas.Count - 1);

                                        if (!Kernel.isPlaying)
                                            GUI.enabled = true;
                                    }

                                    EditorGUILayout.EndHorizontal();
                                }



                                {
                                    //scrollPosList[i] = EditorGUILayout.BeginScrollView(scrollPosList[i]);
                                    //GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, false, 0, Color.white * 0.25f, 0, 0);

                                    for (int j = 0; j < soundMetaDatas.Count; j++)
                                    {
                                        SoundMetaData soundMetaData = soundMetaDatas[j];
                                        string soundPath = soundMetaData.path;
                                        bool stream = soundMetaData.stream;
                                        float pitch = soundMetaData.pitch;
                                        float tempo = soundMetaData.tempo;
                                        float loopStartTime = soundMetaData.loopStartTime;

                                        //CustomInspectorEditor.DrawLine();

                                        EditorGUILayout.BeginHorizontal();
                                        GUILayout.Space(60);

                                        GUILayout.Label("경로", GUILayout.ExpandWidth(false));
                                        soundPath = EditorGUILayout.TextField(soundPath);

                                        //개같네 진짜 안해
                                        /*{
                                            string assetAllPath = PathTool.Combine(Kernel.streamingAssetsPath, ResourceManager.soundPath.Replace("%NameSpace%", nameSpace));
                                            string assetAllPathAndName = PathTool.Combine(assetAllPath, soundPath);

                                            string assetPath = PathTool.Combine("Assets/StreamingAssets", ResourceManager.soundPath.Replace("%NameSpace%", nameSpace));
                                            string assetPathAndName = PathTool.Combine(assetPath, soundPath);

                                            ResourceManager.FileExtensionExists(assetAllPathAndName, out string outPath, ResourceManager.audioExtension);

                                            DefaultAsset audioClip = AssetDatabase.LoadAssetAtPath<DefaultAsset>(assetPathAndName + Path.GetExtension(outPath));
                                            audioClip = (DefaultAsset)EditorGUILayout.ObjectField(audioClip, typeof(DefaultAsset), false);

                                            assetPathAndName = AssetDatabase.GetAssetPath(audioClip);
                                            if (soundData.Key == "jevil")
                                                Debug.Log(assetPathAndName);
                                            string assetName = assetPathAndName.Replace(assetPath + "/", "");
                                            assetAllPathAndName = assetAllPathAndName.Substring(0, assetAllPathAndName.Length - Path.GetExtension(assetAllPathAndName).Length);

                                            if (ResourceManager.FileExtensionExists(assetAllPathAndName, out _, ResourceManager.audioExtension))
                                                soundPath = Path.GetFileNameWithoutExtension(assetName);
                                        }*/

                                        if (soundData.Value.isBGM && SoundManager.Data.useTempo)
                                        {
                                            GUILayout.Label("피치", GUILayout.ExpandWidth(false));
                                            pitch = EditorGUILayout.FloatField(pitch, GUILayout.Width(30)).Clamp(soundMetaData.tempo.Abs() * 0.5f, soundMetaData.tempo.Abs() * 2f);

                                            GUILayout.Label("템포", GUILayout.ExpandWidth(false));
                                            tempo = EditorGUILayout.FloatField(tempo, GUILayout.Width(30));

                                            if (soundMetaData.stream)
                                                tempo = tempo.Clamp(0);
                                        }
                                        else
                                        {
                                            GUILayout.Label("피치", GUILayout.ExpandWidth(false));
                                            pitch = EditorGUILayout.FloatField(pitch, GUILayout.Width(30));

                                            if (soundMetaData.stream)
                                                pitch = pitch.Clamp(0);
                                        }

                                        GUILayout.Label("루프 시작 시간", GUILayout.ExpandWidth(false));
                                        loopStartTime = EditorGUILayout.FloatField(loopStartTime, GUILayout.Width(50)).Clamp(0);

                                        GUILayout.Label("스트림", GUILayout.ExpandWidth(false));
                                        stream = EditorGUILayout.Toggle(stream, GUILayout.Width(20));

                                        EditorGUILayout.EndHorizontal();

                                        soundMetaDatas[j] = new SoundMetaData(soundPath, pitch, tempo, stream, loopStartTime, null);
                                    }
                                    valueList.Add(new SoundData<SoundMetaData>(subtitle, isBGM, soundMetaDatas.ToArray()));

                                    //EditorGUILayout.EndScrollView();
                                }
                            }
                        }

                        EditorGUILayout.EndScrollView();

                        //키 중복 감지
                        bool overlap = keyList.Count != keyList.Distinct().Count();
                        if (!overlap)
                        {
                            //리스트 2개를 딕셔너리로 변환
                            soundDatas = keyList.Zip(valueList, (key, value) => new { key, value }).ToDictionary(a => a.key, a => a.value);
                        }
                    }

                    //플레이 모드가 아니면 변경한 리스트의 데이터를 잃어버리지 않게 파일로 저장
                    if (GUI.changed && !Kernel.isPlaying)
                        File.WriteAllText(jsonPath, JsonManager.ObjectToJson(soundDatas));
                }
            }

            if (GUI.changed && !Kernel.isPlaying)
                SaveLoadManager.Save(audioProjectSetting, Kernel.projectSettingPath);
        }

        string nbsSettingNameSpace = "";
        Vector2 nbsSettingScrollPos = Vector2.zero;
        public void NBSSetting(int scrollYSize = 0)
        {
            //GUI
            {
                EditorGUILayout.LabelField("NBS 설정", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("안전 삭제 모드 (삭제 할 리스트가 빈 값이 아니면 삭제 금지)", GUILayout.Width(330));
                deleteSafety = EditorGUILayout.Toggle(deleteSafety);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }

            EditorGUILayout.BeginHorizontal();

            nbsSettingNameSpace = CustomInspectorEditor.DrawNameSpace("네임스페이스", nbsSettingNameSpace);

            string nameSpace = nbsSettingNameSpace;
            string path = PathTool.Combine(Kernel.streamingAssetsPath, ResourceManager.nbsPath.Replace("%NameSpace%", nameSpace));

            if (Kernel.isPlaying)
                GUI.enabled = false;

            if (!Directory.Exists(path))
            {
                if (GUILayout.Button("nbs 폴더 만들기", GUILayout.ExpandWidth(false)))
                {
                    Directory.CreateDirectory(path);
                    AssetDatabase.Refresh();
                }

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                string jsonPath = path + ".json";
                if (!File.Exists(jsonPath))
                {
                    if (GUILayout.Button("nbs.json 파일 만들기", GUILayout.ExpandWidth(false)))
                    {
                        File.WriteAllText(jsonPath, "{}");
                        AssetDatabase.Refresh();
                    }

                    {
                        if (Directory.GetFiles(path).Length > 0 && deleteSafety)
                            GUI.enabled = false;

                        if (GUILayout.Button("nbs 폴더 지우기", GUILayout.ExpandWidth(false)))
                        {
                            Directory.Delete(path, true);
                            File.Delete(path + ".meta");
                            AssetDatabase.Refresh();
                            EditorGUILayout.EndHorizontal();
                            return;
                        }

                        if (!Kernel.isPlaying)
                            GUI.enabled = true;
                    }

                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    Dictionary<string, SoundData<NBSMetaData>> soundDatas = JsonManager.JsonRead<Dictionary<string, SoundData<NBSMetaData>>>(jsonPath, true);

                    if (soundDatas == null)
                        soundDatas = new Dictionary<string, SoundData<NBSMetaData>>();

                    {
                        if (soundDatas.Count > 0 && deleteSafety)
                            GUI.enabled = false;

                        if (GUILayout.Button("nbs.json 파일 지우기", GUILayout.ExpandWidth(false)))
                        {
                            File.Delete(jsonPath);
                            File.Delete(jsonPath + ".meta");
                            AssetDatabase.Refresh();
                            EditorGUILayout.EndHorizontal();
                            return;
                        }

                        if (!Kernel.isPlaying)
                            GUI.enabled = true;
                    }

                    {
                        if (Directory.GetFiles(path).Length > 0 && deleteSafety)
                            GUI.enabled = false;

                        if (GUILayout.Button("nbs 폴더 지우기", GUILayout.ExpandWidth(false)))
                        {
                            Directory.Delete(path, true);
                            File.Delete(path + ".meta");
                            AssetDatabase.Refresh();
                            EditorGUILayout.EndHorizontal();
                            return;
                        }

                        if (!Kernel.isPlaying)
                            GUI.enabled = true;
                    }

                    EditorGUILayout.EndHorizontal();

                    //GUI
                    {
                        EditorGUILayout.BeginHorizontal();

                        {
                            if (soundDatas.ContainsKey(""))
                                GUI.enabled = false;

                            if (GUILayout.Button("추가", GUILayout.ExpandWidth(false)))
                                soundDatas.Add("", new SoundData<NBSMetaData>("", false, new NBSMetaData[0]));

                            if (!Kernel.isPlaying)
                                GUI.enabled = true;
                        }

                        {
                            if (soundDatas.Count > 0)
                            {
                                SoundData<NBSMetaData> soundData = soundDatas.Values.ToList()[soundDatas.Count - 1];
                                if ((soundDatas.Keys.ToList()[soundDatas.Count - 1] != "" || soundData.subtitle != "" || soundData.sounds == null || soundData.sounds.Count() > 0) && deleteSafety)
                                    GUI.enabled = false;
                            }
                            else
                                GUI.enabled = false;

                            if (GUILayout.Button("삭제", GUILayout.ExpandWidth(false)) && soundDatas.Count > 0)
                                soundDatas.Remove(soundDatas.ToList()[soundDatas.Count - 1].Key);

                            if (!Kernel.isPlaying)
                                GUI.enabled = true;
                        }

                        {
                            int count = EditorGUILayout.IntField("리스트 길이", soundDatas.Count, GUILayout.Height(21));
                            //변수 설정
                            if (count < 0)
                                count = 0;

                            if (count > soundDatas.Count)
                            {
                                for (int i = soundDatas.Count; i < count; i++)
                                {
                                    if (!soundDatas.ContainsKey(""))
                                        soundDatas.Add("", new SoundData<NBSMetaData>("", false, new NBSMetaData[0]));
                                    else
                                        count--;
                                }
                            }
                            else if (count < soundDatas.Count)
                            {
                                SoundData<NBSMetaData> soundData = soundDatas.Values.ToList()[soundDatas.Count - 1];
                                for (int i = soundDatas.Count - 1; i >= count; i--)
                                {
                                    if ((soundDatas.Count > 0 && soundDatas.Keys.ToList()[soundDatas.Count - 1] == "" && soundData.subtitle == "" && soundData.sounds != null && soundData.sounds.Count() <= 0) || !deleteSafety)
                                        soundDatas.Remove(soundDatas.ToList()[soundDatas.Count - 1].Key);
                                    else
                                        count++;
                                }
                            }
                        }

                        EditorGUILayout.EndHorizontal();
                    }



                    {
                        {
                            GUI.enabled = true;

                            if (scrollYSize > 0)
                                nbsSettingScrollPos = EditorGUILayout.BeginScrollView(nbsSettingScrollPos, GUILayout.Height(scrollYSize));
                            else
                                nbsSettingScrollPos = EditorGUILayout.BeginScrollView(nbsSettingScrollPos);

                            if (Kernel.isPlaying)
                                GUI.enabled = false;
                        }

                        //GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, false, 0, Color.white * 0.25f, 0, 0);

                        List<KeyValuePair<string, SoundData<NBSMetaData>>> tempSoundDatas = soundDatas.ToList();
                        //딕셔너리는 키를 수정할수 없기때문에, 리스트로 분활해줘야함
                        List<string> keyList = new List<string>();
                        List<SoundData<NBSMetaData>> valueList = new List<SoundData<NBSMetaData>>();

                        for (int i = 0; i < soundDatas.Count; i++)
                        {
                            KeyValuePair<string, SoundData<NBSMetaData>> soundData = tempSoundDatas[i];

                            CustomInspectorEditor.DrawLine();

                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(30);

                            GUILayout.Label("오디오 키", GUILayout.ExpandWidth(false));
                            keyList.Add(EditorGUILayout.TextField(soundData.Key));

                            GUILayout.Label("자막", GUILayout.ExpandWidth(false));
                            string subtitle = EditorGUILayout.TextField(soundData.Value.subtitle);

                            GUILayout.Label("BGM", GUILayout.ExpandWidth(false));
                            bool isBGM = EditorGUILayout.Toggle(soundData.Value.isBGM, GUILayout.Width(15));

                            EditorGUILayout.EndHorizontal();


                            //리스트 안의 리스트
                            {
                                List<NBSMetaData> soundMetaDatas = soundData.Value.sounds.ToList();
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    GUILayout.Space(30);

                                    {
                                        if (GUILayout.Button("추가", GUILayout.ExpandWidth(false)))
                                            soundMetaDatas.Add(new NBSMetaData("", 1, 1, null));
                                    }

                                    {
                                        if (soundMetaDatas.Count <= 0 || (soundMetaDatas[soundMetaDatas.Count - 1].path != "" && deleteSafety))
                                            GUI.enabled = false;

                                        if (GUILayout.Button("삭제", GUILayout.ExpandWidth(false)) && soundMetaDatas.Count > 0)
                                            soundMetaDatas.RemoveAt(soundMetaDatas.Count - 1);

                                        if (!Kernel.isPlaying)
                                            GUI.enabled = true;
                                    }

                                    EditorGUILayout.EndHorizontal();
                                }



                                {
                                    //scrollPosList[i] = EditorGUILayout.BeginScrollView(scrollPosList[i]);
                                    //GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, false, 0, Color.white * 0.25f, 0, 0);

                                    for (int j = 0; j < soundMetaDatas.Count; j++)
                                    {
                                        NBSMetaData soundMetaData = soundMetaDatas[j];
                                        string soundPath = soundMetaData.path;
                                        float pitch = soundMetaData.pitch;
                                        float tempo = soundMetaData.tempo;

                                        //CustomInspectorEditor.DrawLine();

                                        EditorGUILayout.BeginHorizontal();
                                        GUILayout.Space(60);

                                        GUILayout.Label("경로", GUILayout.ExpandWidth(false));
                                        soundPath = EditorGUILayout.TextField(soundPath);

                                        //개같네 진짜 안해
                                        /*{
                                            string assetAllPath = PathTool.Combine(Kernel.streamingAssetsPath, ResourceManager.soundPath.Replace("%NameSpace%", nameSpace));
                                            string assetAllPathAndName = PathTool.Combine(assetAllPath, soundPath);

                                            string assetPath = PathTool.Combine("Assets/StreamingAssets", ResourceManager.soundPath.Replace("%NameSpace%", nameSpace));
                                            string assetPathAndName = PathTool.Combine(assetPath, soundPath);

                                            ResourceManager.FileExtensionExists(assetAllPathAndName, out string outPath, ResourceManager.audioExtension);

                                            DefaultAsset audioClip = AssetDatabase.LoadAssetAtPath<DefaultAsset>(assetPathAndName + Path.GetExtension(outPath));
                                            audioClip = (DefaultAsset)EditorGUILayout.ObjectField(audioClip, typeof(DefaultAsset), false);

                                            assetPathAndName = AssetDatabase.GetAssetPath(audioClip);
                                            if (soundData.Key == "jevil")
                                                Debug.Log(assetPathAndName);
                                            string assetName = assetPathAndName.Replace(assetPath + "/", "");
                                            assetAllPathAndName = assetAllPathAndName.Substring(0, assetAllPathAndName.Length - Path.GetExtension(assetAllPathAndName).Length);

                                            if (ResourceManager.FileExtensionExists(assetAllPathAndName, out _, ResourceManager.audioExtension))
                                                soundPath = Path.GetFileNameWithoutExtension(assetName);
                                        }*/

                                        GUILayout.Label("피치", GUILayout.ExpandWidth(false));
                                        pitch = EditorGUILayout.FloatField(pitch, GUILayout.Width(30)).Clamp(soundMetaData.tempo.Abs() * 0.5f, soundMetaData.tempo.Abs() * 2f);

                                        GUILayout.Label("템포", GUILayout.ExpandWidth(false));
                                        tempo = EditorGUILayout.FloatField(tempo, GUILayout.Width(30));

                                        EditorGUILayout.EndHorizontal();

                                        soundMetaDatas[j] = new NBSMetaData(soundPath, pitch, tempo, null);
                                    }
                                    valueList.Add(new SoundData<NBSMetaData>(subtitle, isBGM, soundMetaDatas.ToArray()));

                                    //EditorGUILayout.EndScrollView();
                                }
                            }
                        }

                        EditorGUILayout.EndScrollView();

                        //키 중복 감지
                        bool overlap = keyList.Count != keyList.Distinct().Count();
                        if (!overlap)
                        {
                            //리스트 2개를 딕셔너리로 변환
                            soundDatas = keyList.Zip(valueList, (key, value) => new { key, value }).ToDictionary(a => a.key, a => a.value);
                        }
                    }

                    //플레이 모드가 아니면 변경한 리스트의 데이터를 잃어버리지 않게 파일로 저장
                    if (GUI.changed && !Kernel.isPlaying)
                        File.WriteAllText(jsonPath, JsonManager.ObjectToJson(soundDatas));
                }
            }
        }

        public static SaveLoadClass resourceProjectSetting;
        public void ResourceSetting()
        {
            if (!Kernel.isPlaying)
            {
                if (resourceProjectSetting == null)
                    SaveLoadManager.Initialize<ProjectSettingSaveLoadAttribute>(typeof(ResourceManager.Data), out resourceProjectSetting);

                SaveLoadManager.Load(resourceProjectSetting, Kernel.projectSettingPath);
            }

            //GUI
            {
                EditorGUILayout.LabelField("기본 네임스페이스 리스트", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("안전 삭제 모드 (삭제 할 리스트가 빈 값이 아니면 삭제 금지)", GUILayout.Width(330));
                deleteSafety = EditorGUILayout.Toggle(deleteSafety);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }

            CustomInspectorEditor.DrawList(ResourceManager.Data.nameSpaces, "네임스페이스", 0, 0, deleteSafety);

            if (GUI.changed && !Kernel.isPlaying)
                SaveLoadManager.Save(resourceProjectSetting, Kernel.projectSettingPath);
        }
    }
}