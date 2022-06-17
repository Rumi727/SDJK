using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using SCKRM.Resource;

namespace SCKRM.Editor
{
    public class CustomInspectorEditor : UnityEditor.Editor
    {
        [System.NonSerialized] bool repaint = false;

        /// <summary>
        /// Please put base.OnEnable() when overriding
        /// </summary>
        protected virtual void OnEnable()
        {
            if (Kernel.isPlaying)
            {
                repaint = true;
                Repainter();
            }
        }

        /// <summary>
        /// Please put base.OnDisable() when overriding
        /// </summary>
        protected virtual void OnDisable() => repaint = false;

        async void Repainter()
        {
            while (repaint)
            {
                Repaint();
                await Task.Delay(100);
            }
        }

        public override void OnInspectorGUI()
        {

        }



        public SerializedProperty UseProperty(string propertyName) => useProperty(propertyName, "", false);
        public SerializedProperty UseProperty(string propertyName, string label) => useProperty(propertyName, label, true);
        SerializedProperty useProperty(string propertyName, string label, bool labelShow)
        {
            if (propertyName == null)
                propertyName = "";
            if (label == null)
                label = "";

            GUIContent guiContent = null;
            if (labelShow)
            {
                guiContent = new GUIContent();
                guiContent.text = label;
            }

            SerializedProperty tps = serializedObject.FindProperty(propertyName);
            EditorGUI.BeginChangeCheck();

            if (!labelShow)
                EditorGUILayout.PropertyField(tps, true);
            else
                EditorGUILayout.PropertyField(tps, guiContent, true);

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            return tps;
        }

        public string UsePropertyAndDrawStringArray(string propertyName, string value, string[] array) => usePropertyAndDrawStringArray(propertyName, "", value, array, false, out _, out _);
        public string UsePropertyAndDrawStringArray(string propertyName, string label, string value, string[] array) => usePropertyAndDrawStringArray(propertyName, label, value, array, true, out _, out _);
        public string UsePropertyAndDrawStringArray(string propertyName, string value, string[] array, out int index) => usePropertyAndDrawStringArray(propertyName, "", value, array, false, out index, out _);
        public string UsePropertyAndDrawStringArray(string propertyName, string label, string value, string[] array, out int index) => usePropertyAndDrawStringArray(propertyName, label, value, array, true, out index, out _);
        public string UsePropertyAndDrawStringArray(string propertyName, string value, string[] array, out int index, out bool usePropertyChanged) => usePropertyAndDrawStringArray(propertyName, "", value, array, false, out index, out usePropertyChanged);
        public string UsePropertyAndDrawStringArray(string propertyName, string label, string value, string[] array, out int index, out bool usePropertyChanged) => usePropertyAndDrawStringArray(propertyName, label, value, array, true, out index, out usePropertyChanged);
        string usePropertyAndDrawStringArray(string propertyName, string label, string value, string[] array, bool labelShow, out int index, out bool usePropertyChanged)
        {
            if (propertyName == null)
                propertyName = "";
            if (label == null)
                label = "";
            if (value == null)
                value = "";
            if (array == null)
                throw new NullReferenceException();

            EditorGUILayout.BeginHorizontal();

            if (labelShow)
                EditorGUILayout.PrefixLabel(label);

            SerializedProperty serializedProperty = UseProperty(propertyName, "");
            usePropertyChanged = GUI.changed;
            
            index = EditorGUILayout.Popup(Array.IndexOf(array, value), array, GUILayout.MinWidth(0));

            EditorGUILayout.EndHorizontal();

            if (!usePropertyChanged)
            {
                if (index >= 0)
                    return array[index];
                else
                    return value;
            }
            else
                return serializedProperty.stringValue;
        }



        public static void DrawLine(int thickness = 1, int padding = 10) => DrawLine(new Color(0.4980392f, 0.4980392f, 0.4980392f), thickness, padding);

        public static void DrawLine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 18;
            r.width += 22;
            EditorGUI.DrawRect(r, color);
        }



        public static void Space() => EditorGUILayout.Space();



        public delegate T DrawListFunc<T>(T value);

        public static void DrawList(List<int> list, string label, int tab = 0, int tab2 = 0, bool deleteSafety = true) => drawList(list, label, false, Vector2.zero, tab, tab2, deleteSafety);
        public static Vector2 DrawList(List<int> list, string label, Vector2 scrollViewPos, int tab = 0, int tab2 = 0, bool deleteSafety = true) => drawList(list, label, true, scrollViewPos, tab, tab2, deleteSafety);
        static Vector2 drawList(List<int> list, string label, bool scrollView, Vector2 scrollViewPos, int tab, int tab2, bool deleteSafety) => drawList(list, label, (int value) => EditorGUILayout.IntField(value), scrollView, scrollViewPos, tab, tab2, deleteSafety);

        public static void DrawList(List<float> list, string label, int tab = 0, int tab2 = 0, bool deleteSafety = true) => drawList(list, label, false, Vector2.zero, tab, tab2, deleteSafety);
        public static Vector2 DrawList(List<float> list, string label, Vector2 scrollViewPos, int tab = 0, int tab2 = 0, bool deleteSafety = true) => drawList(list, label, true, scrollViewPos, tab, tab2, deleteSafety);
        static Vector2 drawList(List<float> list, string label, bool scrollView, Vector2 scrollViewPos, int tab, int tab2, bool deleteSafety) => drawList(list, label, (float value) => EditorGUILayout.FloatField(value), scrollView, scrollViewPos, tab, tab2, deleteSafety);

        public static void DrawList(List<double> list, string label, int tab = 0, int tab2 = 0, bool deleteSafety = true) => drawList(list, label, false, Vector2.zero, tab, tab2, deleteSafety);
        public static Vector2 DrawList(List<double> list, string label, Vector2 scrollViewPos, int tab = 0, int tab2 = 0, bool deleteSafety = true) => drawList(list, label, true, scrollViewPos, tab, tab2, deleteSafety);
        static Vector2 drawList(List<double> list, string label, bool scrollView, Vector2 scrollViewPos, int tab, int tab2, bool deleteSafety) => drawList(list, label, (double value) => EditorGUILayout.DoubleField(value), scrollView, scrollViewPos, tab, tab2, deleteSafety);

        public static void DrawList(List<string> list, string label, int tab = 0, int tab2 = 0, bool deleteSafety = true) => drawList(list, label, false, Vector2.zero, tab, tab2, deleteSafety);
        public static Vector2 DrawList(List<string> list, string label, Vector2 scrollViewPos, int tab = 0, int tab2 = 0, bool deleteSafety = true) => drawList(list, label, true, scrollViewPos, tab, tab2, deleteSafety);
        static Vector2 drawList(List<string> list, string label, bool scrollView, Vector2 scrollViewPos, int tab, int tab2, bool deleteSafety) => drawList(list, label, (string value) => EditorGUILayout.TextField(value), scrollView, scrollViewPos, tab, tab2, deleteSafety);

        public static void DrawList<T>(List<T> list, string label, DrawListFunc<T> func, int tab = 0, int tab2 = 0, bool deleteSafety = true) => drawList(list, label, func, false, Vector2.zero, tab, tab2, deleteSafety);
        public static Vector2 DrawList<T>(List<T> list, string label, DrawListFunc<T> func, Vector2 scrollViewPos, int tab = 0, int tab2 = 0, bool deleteSafety = true) => drawList(list, label, func, true, scrollViewPos, tab, tab2, deleteSafety);
        static Vector2 drawList<T>(List<T> list, string label, DrawListFunc<T> func, bool scrollView, Vector2 scrollViewPos, int tab, int tab2, bool deleteSafety)
        {
            if (label == null)
                label = "";

            //GUI
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(30 * tab);

                {
                    if (GUILayout.Button("추가", GUILayout.ExpandWidth(false)))
                        list.Add(default);
                }

                {
                    if (deleteSafety && list.Count <= 0 || (list[list.Count - 1] != null && !list[list.Count - 1].Equals(default(T))))
                        GUI.enabled = false;

                    if (GUILayout.Button("삭제", GUILayout.ExpandWidth(false)) && list.Count > 0)
                        list.RemoveAt(list.Count - 1);

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
                            list.Add(default);
                    }
                    else if (count < list.Count)
                    {
                        for (int i = list.Count - 1; i >= count; i--)
                        {
                            if (!deleteSafety || list.Count > 0 && (list[list.Count - 1] == null || list[list.Count - 1].Equals(default(T))))
                                list.RemoveAt(list.Count - 1);
                            else
                                count++;
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            if (scrollView)
                scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos);

            for (int i = 0; i < list.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(30 * (tab + tab2));

                GUILayout.Label(label, GUILayout.ExpandWidth(false));
                list[i] = func.Invoke(list[i]);

                {
                    if (i - 1 < 0)
                        GUI.enabled = false;

                    if (GUILayout.Button("위로", GUILayout.ExpandWidth(false)))
                        list.Move(i, i - 1);

                    GUI.enabled = true;
                }

                {
                    if (i + 1 >= list.Count)
                        GUI.enabled = false;

                    if (GUILayout.Button("아래로", GUILayout.ExpandWidth(false)))
                        list.Move(i, i + 1);

                    GUI.enabled = true;
                }

                {
                    if (deleteSafety && list[i] != null && !list[i].Equals(default(T)))
                        GUI.enabled = false;

                    if (GUILayout.Button("삭제", GUILayout.ExpandWidth(false)))
                        list.RemoveAt(i);

                    GUI.enabled = true;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (scrollView)
                EditorGUILayout.EndScrollView();

            return scrollViewPos;
        }



        public static string DrawNameSpace(string nameSpace) => DrawStringArray(nameSpace, ResourceManager.nameSpaces.ToArray());
        public static string DrawNameSpace(string label, string nameSpace) => DrawStringArray(label, nameSpace, ResourceManager.nameSpaces.ToArray());

        public string UsePropertyAndDrawNameSpace(string propertyName, string nameSpace) => UsePropertyAndDrawNameSpace(propertyName, nameSpace, "");
        public string UsePropertyAndDrawNameSpace(string propertyName, string label, string nameSpace) => UsePropertyAndDrawStringArray(propertyName, label, nameSpace, ResourceManager.nameSpaces.ToArray());



        public static string DrawStringArray(string value, string[] array) => drawStringArray("", value, array, false, out _);
        public static string DrawStringArray(string label, string value, string[] array) => drawStringArray(label, value, array, true, out _);
        public static string DrawStringArray(string value, string[] array, out int index) => drawStringArray("", value, array, false, out index);
        public static string DrawStringArray(string label, string value, string[] array, out int index) => drawStringArray(label, value, array, true, out index);
        static string drawStringArray(string label, string value, string[] array, bool labelShow, out int index)
        {
            if (label == null)
                label = "";
            if (value == null)
                value = "";
            if (array == null)
                throw new NullReferenceException();

            if (!labelShow)
                index = EditorGUILayout.Popup(Array.IndexOf(array, value), array, GUILayout.MinWidth(0));
            else
                index = EditorGUILayout.Popup(label, Array.IndexOf(array, value), array, GUILayout.MinWidth(0));

            if (index >= 0)
                return array[index];
            else
                return value;
        }
    }
}