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
    public class ControlProjectSetting : SettingsProvider
    {
        public ControlProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            if (instance == null)
                instance = new ControlProjectSetting("SC KRM/조작", SettingsScope.Project);

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

                CustomInspectorEditor.DeleteSafety(ref deleteSafety);

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



                if (Kernel.isPlaying)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("플레이 모드에서 바꾼 (인게임 설정에서 바꾼) 조작은 반영되지 않고, 저장되지 않습니다\n기본값만 저장되고 변경됩니다 (키를 초기화한 상태라면, 변경한 키는 인게임에도 적용됩니다)", MessageType.Warning);
                }

                CustomInspectorEditor.DrawLine();

                {
                    if (scrollYSize > 0)
                        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(scrollYSize));
                    else
                        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(false));

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

            //플레이 모드가 아니면 변경한 리스트의 데이터를 잃어버리지 않게 파일로 저장
            if (GUI.changed && !Kernel.isPlaying)
                SaveLoadManager.Save(controlProjectSetting, Kernel.projectSettingPath);
        }
    }
}
