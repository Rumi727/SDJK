using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using SCKRM.Resource;
using System.Collections;
using SCKRM.Rhythm;

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
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding - 2));
            r.height = thickness;
            r.y += padding / 2 - 2;
            r.x -= 18;
            r.width += 22;
            EditorGUI.DrawRect(r, color);
        }

        public static void DrawLineVertical(int thickness = 1, int padding = 10) => DrawLineVertical(new Color(0.4980392f, 0.4980392f, 0.4980392f), thickness, padding);

        public static void DrawLineVertical(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(padding - 2));
            r.width = thickness;
            r.x += padding / 2 - 2;
            r.y -= 18;
            r.height += 22;
            EditorGUI.DrawRect(r, color);
        }



        public static void Space() => EditorGUILayout.Space();
        public static void TabSpace(int tab)
        {
            if (tab > 0)
                GUILayout.Space(30 * tab);
        }

        public delegate object DrawListFunc(object value);
        public delegate bool DrawListDefaultValueFunc(int index);

        public static int DrawList(List<int> list, string label, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => drawList(list, label, false, Vector2.zero, tab, tab2, deleteSafety, displayRestrictions, displayRestrictionsIndex).displayRestrictionsIndex;
        public static (Vector2 scrollViewPos, int displayRestrictionsIndex) DrawList(List<int> list, string label, Vector2 scrollViewPos, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => drawList(list, label, true, scrollViewPos, tab, tab2, deleteSafety, displayRestrictions, displayRestrictionsIndex);
        static (Vector2 scrollViewPos, int displayRestrictionsIndex) drawList(List<int> list, string label, bool scrollView, Vector2 scrollViewPos, int tab, int tab2, bool deleteSafety, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => drawList(typeof(int), list, label, (object value) =>
        {
            EditorGUILayout.BeginHorizontal();
            TabSpace(tab + tab2);

            string value2 = EditorGUILayout.TextField((string)value);

            EditorGUILayout.EndHorizontal();
            return value2;
        }, scrollView, scrollViewPos, tab, tab2, deleteSafety, (int index) => list[index].Equals(0), displayRestrictions, displayRestrictionsIndex);

        public static int DrawList(List<float> list, string label, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => drawList(list, label, false, Vector2.zero, tab, tab2, deleteSafety, displayRestrictions, displayRestrictionsIndex).displayRestrictionsIndex;
        public static (Vector2 scrollViewPos, int displayRestrictionsIndex) DrawList(List<float> list, string label, Vector2 scrollViewPos, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => drawList(list, label, true, scrollViewPos, tab, tab2, deleteSafety, displayRestrictions, displayRestrictionsIndex);
        static (Vector2 scrollViewPos, int displayRestrictionsIndex) drawList(List<float> list, string label, bool scrollView, Vector2 scrollViewPos, int tab, int tab2, bool deleteSafety, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => drawList(typeof(float), list, label, (object value) =>
        {
            EditorGUILayout.BeginHorizontal();
            TabSpace(tab + tab2);

            float value2 = EditorGUILayout.FloatField((float)value);

            EditorGUILayout.EndHorizontal();
            return value2;
        }, scrollView, scrollViewPos, tab, tab2, deleteSafety, (int index) => list[index].Equals(0), displayRestrictions, displayRestrictionsIndex);

        public static int DrawList(List<double> list, string label, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => drawList(list, label, false, Vector2.zero, tab, tab2, deleteSafety, displayRestrictions, displayRestrictionsIndex).displayRestrictionsIndex;
        public static (Vector2 scrollViewPos, int displayRestrictionsIndex) DrawList(List<double> list, string label, Vector2 scrollViewPos, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => drawList(list, label, true, scrollViewPos, tab, tab2, deleteSafety, displayRestrictions, displayRestrictionsIndex);
        static (Vector2 scrollViewPos, int displayRestrictionsIndex) drawList(List<double> list, string label, bool scrollView, Vector2 scrollViewPos, int tab, int tab2, bool deleteSafety, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => drawList(typeof(double), list, label, (object value) =>
        {
            EditorGUILayout.BeginHorizontal();
            TabSpace(tab + tab2);

            double value2 = EditorGUILayout.DoubleField((double)value);

            EditorGUILayout.EndHorizontal();
            return value2;
        }, scrollView, scrollViewPos, tab, tab2, deleteSafety, (int index) => list[index].Equals(0), displayRestrictions, displayRestrictionsIndex);

        public static int DrawList(List<string> list, string label, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => drawList(list, label, false, Vector2.zero, tab, tab2, deleteSafety, displayRestrictions, displayRestrictionsIndex).displayRestrictionsIndex;
        public static (Vector2 scrollViewPos, int displayRestrictionsIndex) DrawList(List<string> list, string label, Vector2 scrollViewPos, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => drawList(list, label, true, scrollViewPos, tab, tab2, deleteSafety, displayRestrictions, displayRestrictionsIndex);
        static (Vector2 scrollViewPos, int displayRestrictionsIndex) drawList(List<string> list, string label, bool scrollView, Vector2 scrollViewPos, int tab, int tab2, bool deleteSafety, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => drawList(typeof(string), list, label, (object value) =>
        {
            EditorGUILayout.BeginHorizontal();
            TabSpace(tab + tab2);

            string value2 = EditorGUILayout.TextField((string)value);

            EditorGUILayout.EndHorizontal();
            return value2;
        }, scrollView, scrollViewPos, tab, tab2, deleteSafety, (int index) => string.IsNullOrWhiteSpace(list[index]), displayRestrictions, displayRestrictionsIndex);

        public static int DrawList<T>(IList list, string label, DrawListFunc topFunc, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => drawList(typeof(T), list, label, topFunc, false, Vector2.zero, tab, tab2, deleteSafety, (int index) => list[index].Equals(default(T)), displayRestrictions, displayRestrictionsIndex).displayRestrictionsIndex;
        public static (Vector2 scrollViewPos, int displayRestrictionsIndex) DrawList<T>(IList list, string label, DrawListFunc topFunc, Vector2 scrollViewPos, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => drawList(typeof(T), list, label, topFunc, true, scrollViewPos, tab, tab2, deleteSafety, (int index) => list[index].Equals(default(T)), displayRestrictions, displayRestrictionsIndex);
        public static int DrawList<T>(IList list, string label, DrawListFunc topFunc, DrawListDefaultValueFunc defaultValueFunc, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => drawList(typeof(T), list, label, topFunc, false, Vector2.zero, tab, tab2, deleteSafety, defaultValueFunc, displayRestrictions, displayRestrictionsIndex).displayRestrictionsIndex;
        public static (Vector2 scrollViewPos, int displayRestrictionsIndex) DrawList<T>(IList list, string label, DrawListFunc topFunc, DrawListDefaultValueFunc defaultValueFunc, Vector2 scrollViewPos, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => drawList(typeof(T), list, label, topFunc, true, scrollViewPos, tab, tab2, deleteSafety, defaultValueFunc, displayRestrictions, displayRestrictionsIndex);

        public static int DrawList(Type type, IList list, string label, DrawListFunc topFunc, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => drawList(type, list, label, topFunc, false, Vector2.zero, tab, tab2, deleteSafety, (int index) => list[index].Equals(type.GetDefaultValue()), displayRestrictions, displayRestrictionsIndex).displayRestrictionsIndex;
        public static (Vector2 scrollViewPos, int displayRestrictionsIndex) DrawList(Type type, IList list, string label, DrawListFunc topFunc, Vector2 scrollViewPos, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => drawList(type, list, label, topFunc, true, scrollViewPos, tab, tab2, deleteSafety, (int index) => list[index].Equals(type.GetDefaultValue()), displayRestrictions, displayRestrictionsIndex);
        public static int DrawList(Type type, IList list, string label, DrawListFunc topFunc, DrawListDefaultValueFunc defaultValueFunc, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => drawList(type, list, label, topFunc, false, Vector2.zero, tab, tab2, deleteSafety, defaultValueFunc, displayRestrictions, displayRestrictionsIndex).displayRestrictionsIndex;
        public static (Vector2 scrollViewPos, int displayRestrictionsIndex) DrawList(Type type, IList list, string label, DrawListFunc topFunc, DrawListDefaultValueFunc defaultValueFunc, Vector2 scrollViewPos, int tab = 0, int tab2 = 0, bool deleteSafety = true, int displayRestrictions = int.MaxValue, int displayRestrictionsIndex = 0) => drawList(type, list, label, topFunc, true, scrollViewPos, tab, tab2, deleteSafety, defaultValueFunc, displayRestrictions, displayRestrictionsIndex);
        static (Vector2 scrollViewPos, int displayRestrictionsIndex) drawList(Type type, IList list, string label, DrawListFunc topFunc, bool scrollView, Vector2 scrollViewPos, int tab, int tab2, bool deleteSafety, DrawListDefaultValueFunc defaultValueFunc, int displayRestrictions, int displayRestrictionsIndex)
        {
            if (label == null)
                label = "";

            //GUI
            {
                EditorGUILayout.BeginHorizontal();

                TabSpace(tab);

                {
                    int count = EditorGUILayout.IntField("리스트 길이", list.Count, GUILayout.Height(21));
                    //변수 설정
                    if (count < 0)
                        count = 0;

                    if (count > list.Count)
                    {
                        for (int i = list.Count; i < count; i++)
                        {
                            if (typeof(IBeatValuePairList).IsAssignableFrom(list.GetType()))
                                ((IBeatValuePairList)list).Add();
                            else
                                list.Add(type.GetDefaultValueNotNull());
                        }
                    }
                    else if (count < list.Count)
                    {
                        for (int i = list.Count - 1; i >= count; i--)
                        {
                            if (!deleteSafety || list.Count > 0 && (list[list.Count - 1] == null || defaultValueFunc.Invoke(list.Count - 1)))
                                list.RemoveAt(list.Count - 1);
                            else
                                count++;
                        }
                    }
                }

                {
                    if (GUILayout.Button("추가", GUILayout.ExpandWidth(false)))
                    {
                        if (typeof(IBeatValuePairList).IsAssignableFrom(list.GetType()))
                            ((IBeatValuePairList)list).Add();
                        else
                            list.Add(type.GetDefaultValueNotNull());
                    }
                }

                {
                    EditorGUI.BeginDisabledGroup(deleteSafety && (list.Count <= 0 || (list[list.Count - 1] != null && !defaultValueFunc.Invoke(list.Count - 1))));

                    if (GUILayout.Button("삭제", GUILayout.ExpandWidth(false)) && list.Count > 0)
                        list.RemoveAt(list.Count - 1);

                    EditorGUI.EndDisabledGroup();
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();

                TabSpace(tab);

                {
                    int count = EditorGUILayout.IntField("화면 인덱스", displayRestrictionsIndex, GUILayout.Height(21));
                    //변수 설정
                    if (count < 0)
                        count = 0;

                    if (count > displayRestrictionsIndex)
                    {
                        for (int i = displayRestrictionsIndex; i < count && (list.Count - (displayRestrictions * displayRestrictionsIndex)) > displayRestrictions; i++)
                            displayRestrictionsIndex++;
                    }
                    else if (count < displayRestrictionsIndex)
                    {
                        for (int i = displayRestrictionsIndex; i >= count; i--)
                        {
                            if (displayRestrictionsIndex > 0)
                                displayRestrictionsIndex--;
                            else
                                count++;
                        }
                    }
                }

                {
                    EditorGUI.BeginDisabledGroup(displayRestrictionsIndex <= 0);

                    if (GUILayout.Button("이전", GUILayout.ExpandWidth(false)))
                        displayRestrictionsIndex--;

                    EditorGUI.EndDisabledGroup();
                }

                {
                    EditorGUI.BeginDisabledGroup((list.Count - (displayRestrictions * displayRestrictionsIndex)) <= displayRestrictions);

                    if (GUILayout.Button("다음", GUILayout.ExpandWidth(false)))
                        displayRestrictionsIndex++;

                    EditorGUI.EndDisabledGroup();
                }

                EditorGUILayout.EndHorizontal();
            }

            DrawLine();

            if (scrollView)
                scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos);

            int loopCount = 0;
            for (int i = displayRestrictions * displayRestrictionsIndex; i < list.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                TabSpace(tab + tab2);

                if (!string.IsNullOrEmpty(label))
                    GUILayout.Label(label, GUILayout.ExpandWidth(false));

                EditorGUILayout.EndHorizontal();

                if (topFunc != null)
                    list[i] = topFunc.Invoke(list[i]);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();

                {
                    if (GUILayout.Button("추가", GUILayout.ExpandWidth(false)))
                    {
                        if (typeof(IBeatValuePairList).IsAssignableFrom(list.GetType()))
                            ((IBeatValuePairList)list).Insert(i + 1);
                        else
                            list.Insert(i + 1, type.GetDefaultValueNotNull());
                    }
                }

                {
                    EditorGUI.BeginDisabledGroup(deleteSafety && list[i] != null && !defaultValueFunc.Invoke(i));

                    if (GUILayout.Button("삭제", GUILayout.ExpandWidth(false)))
                        list.RemoveAt(i);

                    EditorGUI.EndDisabledGroup();
                }

                {
                    EditorGUI.BeginDisabledGroup(i - 1 < 0);

                    if (GUILayout.Button("이전", GUILayout.ExpandWidth(false)))
                        list.Move(i, i - 1);

                    EditorGUI.EndDisabledGroup();
                }

                {
                    EditorGUI.BeginDisabledGroup(i + 1 >= list.Count);

                    if (GUILayout.Button("다음", GUILayout.ExpandWidth(false)))
                        list.Move(i, i + 1);

                    EditorGUI.EndDisabledGroup();
                }

                EditorGUILayout.EndHorizontal();
                loopCount++;

                if (loopCount >= displayRestrictions)
                    break;

                if (i < list.Count - 1)
                    DrawLine();
            }

            if (scrollView)
                EditorGUILayout.EndScrollView();

            return (scrollViewPos, displayRestrictionsIndex);
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

        public static void FileObjectField<T>(string label, ref string path, out bool isChanged) where T : UnityEngine.Object
        {
            T oldAssets = AssetDatabase.LoadAssetAtPath<T>(path);
            T assets = (T)EditorGUILayout.ObjectField(label, oldAssets, typeof(T), false);

            path = AssetDatabase.GetAssetPath(assets);

            EditorGUILayout.LabelField("경로: " + path);
            isChanged = oldAssets != assets;
        }

        public static void DeleteSafety(ref bool value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("안전 삭제 모드 (삭제 할 리스트가 빈 값이 아니면 삭제 금지)", GUILayout.Width(330));
            value = EditorGUILayout.Toggle(value);
            EditorGUILayout.EndHorizontal();
        }
    }
}