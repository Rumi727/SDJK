using Cysharp.Threading.Tasks;
using SCKRM.Renderer;
using SCKRM.Resource;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SCKRM.Editor
{
    public sealed class SCKRMWindowTabResource : ISCKRMWindowTab
    {
        [InitializeOnLoadMethod] public static void Init() => SCKRMWindowEditor.TabAdd(new SCKRMWindowTabResource());

        public string name => "리소스";
        public int sortIndex => 300;


        public void OnGUI() => render(0);
        public static void Render(SCKRMWindowTabResource window, int scrollYSize = 0) => window.render(scrollYSize);

        bool deleteSafety = true;
        Vector2 resourceScrollPos = Vector2.zero;
        void render(int scrollYSize = 0)
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
                    CustomInspectorEditor.DeleteSafety(ref deleteSafety);

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
