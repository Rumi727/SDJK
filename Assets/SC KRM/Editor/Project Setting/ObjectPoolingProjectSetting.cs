using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SCKRM.SaveLoad;
using SCKRM.ProjectSetting;
using System.Linq;
using UnityEngine.UIElements;
using SCKRM.Object;

namespace SCKRM.Editor
{
    public class ObjectPoolingProjectSetting : SettingsProvider
    {
        public ObjectPoolingProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            if (instance == null)
                instance = new ObjectPoolingProjectSetting("SC KRM/오브젝트 풀링", SettingsScope.Project);

            return instance;
        }



        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            if (!Kernel.isPlaying)
            {
                if (objectPoolingProjectSetting == null)
                    SaveLoadManager.Initialize<ProjectSettingSaveLoadAttribute>(typeof(ObjectPoolingSystem.Data), out objectPoolingProjectSetting);

                SaveLoadManager.Load(objectPoolingProjectSetting, Kernel.projectSettingPath);
            }

            if (ObjectPoolingSystem.Data.prefabList == null)
                ObjectPoolingSystem.Data.prefabList = new Dictionary<string, string>();
        }

        bool deleteSafety = true;
        Vector2 scrollPos = Vector2.zero;
        public override void OnGUI(string searchContext) => DrawGUI(ref deleteSafety, ref scrollPos);

        public static SaveLoadClass objectPoolingProjectSetting = null;
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

            CustomInspectorEditor.DrawLine();



            {
                if (scrollYSize > 0)
                    scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(scrollYSize));
                else
                    scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

                //PrefabObject의 <string, string>를 <string, GameObject>로 바꿔서 인스펙터에 보여주고 인스펙터에서 변경한걸 <string, string>로 다시 바꿔서 PrefabObject에 저장
                /*
                 * 왜 이렇게 변환하냐면 JSON에 오브젝트를 저장할려면 우선적으로 string 값같은 경로가 있어야하고
                   인스펙터에서 쉽게 드래그로 오브젝트를 바꾸기 위해선
                   GameObject 형식이여야해서 이런 소용돌이가 나오게 된것
                */
                List<KeyValuePair<string, string>> prefabObject = ObjectPoolingSystem.Data.prefabList.ToList();

                int up = -1;
                int down = -1;

                //딕셔너리는 키를 수정할수 없기때문에, 리스트로 분활해줘야함
                List<string> keyList = new List<string>();
                List<string> valueList = new List<string>();

                for (int i = 0; i < ObjectPoolingSystem.Data.prefabList.Count; i++)
                {
                    KeyValuePair<string, string> item = prefabObject[i];

                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Label("프리팹 키", GUILayout.ExpandWidth(false));
                    string key = EditorGUILayout.TextField(item.Key);

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Space(20);

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

                    {
                        if (i - 1 < 0)
                            GUI.enabled = false;

                        if (GUILayout.Button("위로", GUILayout.ExpandWidth(false)))
                            up = i;

                        GUI.enabled = true;
                    }

                    {
                        if (i + 1 >= ObjectPoolingSystem.Data.prefabList.Count)
                            GUI.enabled = false;

                        if (GUILayout.Button("아래로", GUILayout.ExpandWidth(false)))
                            down = i;

                        GUI.enabled = true;
                    }

                    if (deleteSafety && key != null && key != "")
                        GUI.enabled = false;

                    if (!GUILayout.Button("삭제", GUILayout.ExpandWidth(false)))
                    {
                        /*
                        * 변경한 프리팹이 리소스 폴더에 있지 않은경우
                        저장은 되지만 프리팹을 감지할수 없기때문에
                        조건문으로 경고를 표시해주고
                        경로가 중첩되는 현상을 대비하기 위해 경로를 빈 문자열로 변경해줌
                        */

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
                            GUI.changed = true;
                            EditorGUILayout.HelpBox("'Resources' 폴더에 있고 IObjectPooling 인터페이스를 상속받는 스크립트가 최상단에 포함된 오브젝트를 넣어주세요", MessageType.Info);
                        }
                    }
                    else
                        EditorGUILayout.EndHorizontal();

                    GUI.enabled = true;

                    if (i != ObjectPoolingSystem.Data.prefabList.Count - 1)
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
    }
}
