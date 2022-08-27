using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SCKRM.SaveLoad;
using SCKRM.ProjectSetting;
using SCKRM.Input;
using System.Linq;
using UnityEngine.UIElements;

namespace SCKRM.Editor
{
    public class ControlLockProjectSetting : SettingsProvider
    {
        public ControlLockProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            if (instance == null)
                instance = new ControlLockProjectSetting("SC KRM/조작 잠금", SettingsScope.Project);

            return instance;
        }



        public override void OnActivate(string searchContext, VisualElement rootElement)
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
        }

        bool deleteSafety = true;
        Vector2 scrollPos = Vector2.zero;
        public override void OnGUI(string searchContext) => DrawGUI(ref deleteSafety, ref scrollPos);

        public static SaveLoadClass controlProjectSetting;
        public static void DrawGUI(ref bool deleteSafety, ref Vector2 scrollPos, float scrollYSize = 0)
        {
            //GUI
            {
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("안전 삭제 모드 (삭제 할 리스트가 빈 값이 아니면 삭제 금지)", GUILayout.Width(330));
                deleteSafety = EditorGUILayout.Toggle(deleteSafety);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }

            //Input Lock Setting List
            {
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
                        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(scrollYSize));
                    else
                        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(false));

                    List<KeyValuePair<string, bool>> inputLockList = InputManager.Data.inputLockList.ToList();

                    int up = -1;
                    int down = -1;

                    //딕셔너리는 키를 수정할수 없기때문에, 리스트로 분활해줘야함
                    List<string> keyList = new List<string>();
                    List<bool> valueList = new List<bool>();

                    for (int i = 0; i < InputManager.Data.inputLockList.Count; i++)
                    {
                        string key;
                        bool value;
                        KeyValuePair<string, bool> item = inputLockList[i];

                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Label("잠금 키", GUILayout.ExpandWidth(false));
                        key = EditorGUILayout.TextField(item.Key);

                        GUILayout.Label("잠금", GUILayout.ExpandWidth(false));
                        value = EditorGUILayout.Toggle(item.Value);

                        {
                            if (i - 1 < 0)
                                GUI.enabled = false;

                            if (GUILayout.Button("위로", GUILayout.ExpandWidth(false)))
                                up = i;

                            GUI.enabled = true;
                        }

                        {
                            if (i + 1 >= InputManager.Data.inputLockList.Count)
                                GUI.enabled = false;

                            if (GUILayout.Button("아래로", GUILayout.ExpandWidth(false)))
                                down = i;

                            GUI.enabled = true;
                        }

                        {
                            if (key != null && key != "" && deleteSafety)
                                GUI.enabled = false;

                            if (!GUILayout.Button("삭제", GUILayout.ExpandWidth(false)))
                            {
                                keyList.Add(key);
                                valueList.Add(value);
                            }

                            GUI.enabled = true;
                        }

                        GUI.enabled = true;

                        EditorGUILayout.EndHorizontal();
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
    }
}
