using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SCKRM.SaveLoad;
using SCKRM.ProjectSetting;
using System.Linq;
using UnityEngine.UIElements;
using SCKRM.Sound;
using SCKRM.Resource;
using System.IO;
using SCKRM.Json;

namespace SCKRM.Editor
{
    public class AudioProjectSetting : SettingsProvider
    {
        public AudioProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            if (instance == null)
                instance = new AudioProjectSetting("SC KRM/오디오", SettingsScope.Project);

            return instance;
        }



        bool deleteSafety = true;
        string nameSpace = "";
        Vector2 scrollPos = Vector2.zero;
        public override void OnGUI(string searchContext) => DrawGUI(ref deleteSafety, ref nameSpace, ref scrollPos);

        public static SaveLoadClass audioProjectSetting = null;
        public static void DrawGUI(ref bool deleteSafety, ref string nameSpace, ref Vector2 scrollPos, float scrollYSize = 0)
        {
            //GUI
            {
                EditorGUILayout.LabelField("오디오 설정", EditorStyles.boldLabel);

                CustomInspectorEditor.DeleteSafety(ref deleteSafety);

                EditorGUILayout.Space();
            }

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("네임스페이스", GUILayout.ExpandWidth(false));
            nameSpace = CustomInspectorEditor.DrawNameSpace(nameSpace);

            string path = PathUtility.Combine(Kernel.streamingAssetsPath, ResourceManager.soundPath.Replace("%NameSpace%", nameSpace));

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
                                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(scrollYSize));
                            else
                                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

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
                                        int loopStartIndex = soundMetaData.loopStartIndex;

                                        //CustomInspectorEditor.DrawLine();

                                        EditorGUILayout.BeginHorizontal();
                                        GUILayout.Space(60);

                                        GUILayout.Label("경로", GUILayout.ExpandWidth(false));
                                        //soundPath = EditorGUILayout.TextField(soundPath);

                                        //GUI
                                        {
                                            string assetAllPath = PathUtility.Combine(Kernel.streamingAssetsPath, ResourceManager.soundPath.Replace("%NameSpace%", nameSpace));
                                            string assetAllPathAndName = PathUtility.Combine(assetAllPath, soundPath);

                                            string assetPath = PathUtility.Combine("Assets/StreamingAssets", ResourceManager.soundPath.Replace("%NameSpace%", nameSpace));
                                            string assetPathAndName = PathUtility.Combine(assetPath, soundPath);

                                            ResourceManager.FileExtensionExists(assetAllPathAndName, out string outPath, ResourceManager.audioExtension);

                                            DefaultAsset audioClip;
                                            if (soundPath != "")
                                            {
                                                audioClip = AssetDatabase.LoadAssetAtPath<DefaultAsset>(assetPathAndName + Path.GetExtension(outPath));
                                                audioClip = (DefaultAsset)EditorGUILayout.ObjectField(audioClip, typeof(DefaultAsset), false);
                                            }
                                            else
                                                audioClip = (DefaultAsset)EditorGUILayout.ObjectField(null, typeof(DefaultAsset), false);

                                            string changedAssetPathAneName = AssetDatabase.GetAssetPath(audioClip).Replace(assetPath + "/", "");

                                            bool exists = false;
                                            for (int k = 0; k < ResourceManager.audioExtension.Length; k++)
                                            {
                                                if (Path.GetExtension(changedAssetPathAneName) == "." + ResourceManager.audioExtension[k])
                                                {
                                                    exists = true;
                                                    soundPath = PathUtility.GetPathWithExtension(changedAssetPathAneName);

                                                    continue;
                                                }
                                            }

                                            if (!exists)
                                            {
                                                GUI.changed = true;
                                                soundPath = "";
                                            }
                                        }

                                        GUILayout.Label("피치", GUILayout.ExpandWidth(false));
                                        pitch = EditorGUILayout.FloatField(pitch, GUILayout.Width(30)).Clamp(soundMetaData.tempo.Abs() * 0.5f, soundMetaData.tempo.Abs() * 2f);

                                        GUILayout.Label("템포", GUILayout.ExpandWidth(false));
                                        tempo = EditorGUILayout.FloatField(tempo, GUILayout.Width(30));

                                        if (soundMetaData.stream)
                                            tempo = tempo.Clamp(0);

                                        GUILayout.Label("루프 시작 시간", GUILayout.ExpandWidth(false));
                                        loopStartIndex = EditorGUILayout.IntField(loopStartIndex, GUILayout.Width(50)).Clamp(0);

                                        GUILayout.Label("스트림", GUILayout.ExpandWidth(false));
                                        stream = EditorGUILayout.Toggle(stream, GUILayout.Width(20));

                                        EditorGUILayout.EndHorizontal();

                                        soundMetaDatas[j] = new SoundMetaData(soundPath, pitch, tempo, stream, loopStartIndex, null);
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
    }
}
