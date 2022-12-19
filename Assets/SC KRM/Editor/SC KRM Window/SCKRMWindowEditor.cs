using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SCKRM.Editor
{
    public class SCKRMWindowEditor : EditorWindow
    {
        static List<ISCKRMWindowTab> tabs = new List<ISCKRMWindowTab>();
        static List<string> tabNames = new List<string>();



        bool inspectorUpdate = true;
        int tabIndex = 0;
        void OnGUI()
        {
            {
                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                tabIndex = GUILayout.Toolbar(tabIndex, tabNames.ToArray(), GUILayout.ExpandWidth(false));

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

            tabs[tabIndex].OnGUI();
        }



        public static void TabAdd(ISCKRMWindowTab tab)
        {
            tabs.Add(tab);
            tabs.Sort((a, b) =>
            {
                if (a.sortIndex == b.sortIndex)
                    return 0;
                else if (a.sortIndex < b.sortIndex)
                    return -1;
                else
                    return 1;
            });

            tabNames.Clear();
            for (int i = 0; i < tabs.Count; i++)
                tabNames.Add(tabs[i].name);
        }

        public static void TabRemove(ISCKRMWindowTab tab)
        {
            tabs.Remove(tab);
            tabNames.Remove(tab.name);
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
    }

    public interface ISCKRMWindowTab
    {
        string name { get; }
        int sortIndex { get; }

        void OnGUI();
    }
}