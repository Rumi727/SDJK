using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SCKRM.SaveLoad;
using System.Linq;
using SCKRM.Resource;
using System.IO;
using SCKRM.Json;

namespace SCKRM.Editor
{
    public class NBSProjectSetting : SettingsProvider
    {
        public NBSProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            if (instance == null)
                instance = new NBSProjectSetting("SC KRM/NBS", SettingsScope.Project);

            return instance;
        }



        bool deleteSafety = true;
        string nameSpace = "";
        Vector2 scrollPos = Vector2.zero;
        public override void OnGUI(string searchContext) => DrawGUI(ref deleteSafety, ref nameSpace, ref scrollPos);

        public static void DrawGUI(ref bool deleteSafety, ref string nameSpace, ref Vector2 scrollPos, float scrollYSize = 0)
        {
            //GUI
            {
                EditorGUILayout.Space();

                CustomInspectorEditor.DeleteSafety(ref deleteSafety);

                EditorGUILayout.Space();
            }

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("네임스페이스", GUILayout.ExpandWidth(false));
            nameSpace = CustomInspectorEditor.DrawNameSpace(nameSpace);

            string path = PathUtility.Combine(Kernel.streamingAssetsPath, ResourceManager.nbsPath.Replace("%NameSpace%", nameSpace));

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
                                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(scrollYSize));
                            else
                                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

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
                                        //soundPath = EditorGUILayout.TextField(soundPath);

                                        //GUI
                                        {
                                            string assetAllPath = PathUtility.Combine(Kernel.streamingAssetsPath, ResourceManager.nbsPath.Replace("%NameSpace%", nameSpace));
                                            string assetAllPathAndName = PathUtility.Combine(assetAllPath, soundPath);

                                            string assetPath = PathUtility.Combine("Assets/StreamingAssets", ResourceManager.nbsPath.Replace("%NameSpace%", nameSpace));
                                            string assetPathAndName = PathUtility.Combine(assetPath, soundPath);

                                            ResourceManager.FileExtensionExists(assetAllPathAndName, out string outPath, "nbs");

                                            DefaultAsset nbs;
                                            if (soundPath != "")
                                            {
                                                nbs = AssetDatabase.LoadAssetAtPath<DefaultAsset>(assetPathAndName + Path.GetExtension(outPath));
                                                nbs = (DefaultAsset)EditorGUILayout.ObjectField(nbs, typeof(DefaultAsset), false);
                                            }
                                            else
                                                nbs = (DefaultAsset)EditorGUILayout.ObjectField(null, typeof(DefaultAsset), false);

                                            string changedAssetPathAneName = AssetDatabase.GetAssetPath(nbs).Replace(assetPath + "/", "");

                                            if (Path.GetExtension(changedAssetPathAneName) == ".nbs")
                                                soundPath = PathUtility.GetPathWithExtension(changedAssetPathAneName);
                                            else
                                            {
                                                GUI.changed = true;
                                                soundPath = "";
                                            }
                                        }

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
    }
}
