using ExtendedNumerics;
using Newtonsoft.Json;
using SCKRM;
using SCKRM.Easing;
using SCKRM.Editor;
using SCKRM.Json;
using SCKRM.Rhythm;
using SDJK.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace SDJK.MapEditor
{
    public sealed class MapEffectEditorWindow
    {
        public MapEffectEditorWindow(MapEditorWindow editor)
        {
            this.editor = editor;
            mapFile = editor.mapFile;

            treeView = new MapEffectTreeView(new TreeViewState(), mapFile);
            splitView = new EditorGUISplitView(EditorGUISplitView.Direction.Horizontal, editor);
        }

        public MapEditorWindow editor;
        public MapFile mapFile;
        public MapEffectTreeView treeView;
        EditorGUISplitView splitView;


        Vector2 scrollPosition;
        int displayRestrictionsIndex;
        public void OnGUI()
        {
            splitView.BeginSplitView();
            treeView.OnGUI(EditorGUILayout.GetControlRect(GUILayout.Width(splitView.availableRect.width * splitView.splitNormalizedPosition - 7), GUILayout.ExpandHeight(true)));

            splitView.Split();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (treeView.selectedItem != null)
            {
                object targetObject = treeView.selectedItem.targetObject;
                PropertyInfo propertyInfo = treeView.selectedItem.propertyInfo;
                Recursion(targetObject, propertyInfo, 0);

                void Recursion(object targetObject, PropertyInfo propertyInfo, int tab)
                {
                    EditorGUILayout.BeginHorizontal();
                    CustomInspectorEditor.TabSpace(tab);

                    GUILayout.Label(propertyInfo.Name, EditorStyles.boldLabel);

                    EditorGUILayout.EndHorizontal();

                    if (propertyInfo.GetCustomAttributes<JsonIgnoreAttribute>().Count() > 0 || !propertyInfo.CanWrite)
                        GUI.enabled = false;

                    if (Field(propertyInfo.Name, propertyInfo.GetValue(targetObject), out object value, false, tab + 1))
                    {
                        if (propertyInfo.CanWrite)
                            propertyInfo.SetValue(targetObject, value);
                    }

                    GUI.enabled = true;

                    bool Field(string fieldName, object currentObject, out object outObject, bool isList, int tab)
                    {
                        Type type = currentObject.GetType();

                        if (type == typeof(bool))
                        {
                            EditorGUILayout.BeginHorizontal();
                            CustomInspectorEditor.TabSpace(tab);

                            outObject = EditorGUILayout.Toggle(fieldName, (bool)currentObject);

                            EditorGUILayout.EndHorizontal();
                            return true;
                        }
                        else if (type == typeof(byte)
                            || type == typeof(sbyte)
                            || type == typeof(short)
                            || type == typeof(ushort)
                            || type == typeof(int)
                            || type == typeof(uint)
                            || type == typeof(long)
                            || type == typeof(ulong)
                            || type == typeof(BigInteger)
                            || type == typeof(nint)
                            || type == typeof(nuint))
                        {
                            outObject = IntField();
                            return true;
                        }
                        else if (type == typeof(float) || type == typeof(double) || type == typeof(decimal) || type == typeof(BigDecimal))
                        {
                            outObject = DecimalField();
                            return true;
                        }
                        else if (type == typeof(string))
                        {
                            EditorGUILayout.BeginHorizontal();
                            CustomInspectorEditor.TabSpace(tab);

                            if (!string.IsNullOrEmpty(fieldName))
                                GUILayout.Label(fieldName, GUILayout.ExpandWidth(false));

                            outObject = EditorGUILayout.TextArea((string)currentObject);

                            EditorGUILayout.EndHorizontal();
                            return true;
                        }
                        else if (type == typeof(JVector2))
                        {
                            EditorGUILayout.BeginHorizontal();
                            CustomInspectorEditor.TabSpace(tab);

                            outObject = (JVector2)EditorGUILayout.Vector2Field(fieldName, (JVector2)currentObject);

                            EditorGUILayout.EndHorizontal();
                            return true;
                        }
                        else if (type == typeof(JVector3))
                        {
                            EditorGUILayout.BeginHorizontal();
                            CustomInspectorEditor.TabSpace(tab);

                            outObject = (JVector3)EditorGUILayout.Vector3Field(fieldName, (JVector3)currentObject);

                            EditorGUILayout.EndHorizontal();
                            return true;
                        }
                        else if (type == typeof(JVector4))
                        {
                            EditorGUILayout.BeginHorizontal();
                            CustomInspectorEditor.TabSpace(tab);

                            outObject = (JVector4)EditorGUILayout.Vector4Field(fieldName, (JVector4)currentObject);

                            EditorGUILayout.EndHorizontal();
                            return true;
                        }
                        else if (type == typeof(JRect))
                        {
                            EditorGUILayout.BeginHorizontal();
                            CustomInspectorEditor.TabSpace(tab);

                            outObject = (JRect)EditorGUILayout.RectField(fieldName, (JRect)currentObject);

                            EditorGUILayout.EndHorizontal();
                            return true;
                        }
                        else if (type == typeof(JColor))
                        {
                            EditorGUILayout.BeginHorizontal();
                            CustomInspectorEditor.TabSpace(tab);

                            outObject = (JColor)EditorGUILayout.ColorField(fieldName, (JColor)currentObject);

                            EditorGUILayout.EndHorizontal();
                            return true;
                        }
                        else if (type == typeof(JColor32))
                        {
                            EditorGUILayout.BeginHorizontal();
                            CustomInspectorEditor.TabSpace(tab);

                            outObject = (JColor32)EditorGUILayout.ColorField(fieldName, (JColor32)currentObject);

                            EditorGUILayout.EndHorizontal();
                            return true;
                        }
                        else if (type == typeof(AnimationCurve))
                        {
                            EditorGUILayout.BeginHorizontal();
                            CustomInspectorEditor.TabSpace(tab);

                            outObject = EditorGUILayout.CurveField(fieldName, (AnimationCurve)currentObject);

                            EditorGUILayout.EndHorizontal();
                            return true;
                        }
                        else if (typeof(Enum).IsAssignableFrom(type))
                        {
                            EditorGUILayout.BeginHorizontal();
                            CustomInspectorEditor.TabSpace(tab);

                            outObject = EditorGUILayout.EnumPopup(fieldName, (Enum)currentObject);

                            EditorGUILayout.EndHorizontal();
                            return true;
                        }
                        else if (typeof(ITypeList).IsAssignableFrom(type))
                        {
                            object value = currentObject;
                            ITypeList list = (ITypeList)value;
                            
                            displayRestrictionsIndex = CustomInspectorEditor.DrawList(list.listType, list, "", Top, StringDefault, tab, tab + 1, true, 30, displayRestrictionsIndex);

                            object Top(object currentObject)
                            {
                                Field("", currentObject, out object result, true, tab + 1);
                                return result;
                            }

                            bool StringDefault(int index)
                            {
                                object value = list[index];

                                if (value.GetType() == typeof(string))
                                    return string.IsNullOrWhiteSpace((string)value);
                                else
                                    return true;
                            }

                            outObject = value;
                            return true;
                        }
                        else if (typeof(ICollection).IsAssignableFrom(type))
                        {
                            outObject = currentObject;
                            return false;
                        }
                        else if (typeof(IBeatValuePairAni).IsAssignableFrom(type))
                        {
                            IBeatValuePairAni pair = (IBeatValuePairAni)currentObject;

                            EditorGUILayout.BeginHorizontal();
                            CustomInspectorEditor.TabSpace(tab);

                            pair.beat = EditorGUILayout.DoubleField(nameof(pair.beat), pair.beat);

                            EditorGUILayout.EndHorizontal();

                            Field(nameof(pair.value), pair.value, out object outValue, isList, tab);
                            pair.value = outValue;

                            EditorGUILayout.BeginHorizontal();
                            CustomInspectorEditor.TabSpace(tab);

                            pair.length = EditorGUILayout.DoubleField(nameof(pair.length), pair.length);

                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            CustomInspectorEditor.TabSpace(tab);

                            GUILayout.Label(nameof(pair.easingFunction), GUILayout.ExpandWidth(false));
                            pair.easingFunction = (EasingFunction.Ease)EditorGUILayout.EnumPopup(pair.easingFunction);

                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            CustomInspectorEditor.TabSpace(tab);

                            GUILayout.Label(nameof(pair.disturbance), GUILayout.ExpandWidth(false));
                            pair.disturbance = EditorGUILayout.Toggle(pair.disturbance, GUILayout.ExpandWidth(false));

                            EditorGUILayout.EndHorizontal();

                            outObject = pair;
                            return true;
                        }
                        else if (typeof(IBeatValuePair).IsAssignableFrom(type))
                        {
                            IBeatValuePair pair = (IBeatValuePair)currentObject;

                            EditorGUILayout.BeginHorizontal();
                            CustomInspectorEditor.TabSpace(tab);

                            pair.beat = EditorGUILayout.DoubleField(nameof(pair.beat), pair.beat);

                            EditorGUILayout.EndHorizontal();

                            Field(nameof(pair.value), pair.value, out object outValue, isList, tab);
                            pair.value = outValue;

                            EditorGUILayout.BeginHorizontal();
                            CustomInspectorEditor.TabSpace(tab);

                            GUILayout.Label(nameof(pair.disturbance), GUILayout.ExpandWidth(false));
                            pair.disturbance = EditorGUILayout.Toggle(pair.disturbance, GUILayout.ExpandWidth(false));

                            EditorGUILayout.EndHorizontal();

                            outObject = pair;
                            return true;
                        }
                        else
                        {
                            PropertyInfo[] propertyInfos = type.GetProperties();
                            for (int i = 0; i < propertyInfos.Length; i++)
                            {
                                PropertyInfo propertyInfo2 = propertyInfos[i];
                                Recursion(currentObject, propertyInfo2, tab);

                                if (i < propertyInfos.Length - 1 && !isList)
                                    CustomInspectorEditor.DrawLine(2);
                            }

                            outObject = currentObject;
                            return true;
                        }
                        
                        object IntField()
                        {
                            EditorGUILayout.BeginHorizontal();
                            CustomInspectorEditor.TabSpace(tab);

                            int value = (int)Convert.ChangeType(currentObject, typeof(int));

                            if (!string.IsNullOrEmpty(fieldName))
                            {
                                int value2 = EditorGUILayout.IntField(fieldName, value);

                                EditorGUILayout.EndHorizontal();
                                return Convert.ChangeType(value2, type);
                            }
                            else
                            {
                                int value2 = EditorGUILayout.IntField(value);

                                EditorGUILayout.EndHorizontal();
                                return Convert.ChangeType(value2, type);
                            }
                        }

                        object DecimalField()
                        {
                            EditorGUILayout.BeginHorizontal();
                            CustomInspectorEditor.TabSpace(tab);

                            double value = (double)Convert.ChangeType(currentObject, typeof(double));

                            if (!string.IsNullOrEmpty(fieldName))
                            {
                                double value2 = EditorGUILayout.DoubleField(fieldName, value);

                                EditorGUILayout.EndHorizontal();
                                return Convert.ChangeType(value2, type);
                            }
                            else
                            {
                                double value2 = EditorGUILayout.DoubleField(value);

                                EditorGUILayout.EndHorizontal();
                                return Convert.ChangeType(value2, type);
                            }
                        }
                    }
                }
            }

            if (GUI.changed)
                RhythmManager.MapChange(mapFile.globalEffect.bpm, mapFile.info.songOffset, mapFile.globalEffect.yukiMode);

            EditorGUILayout.EndScrollView();

            splitView.EndSplitView();
        }
    }

    public sealed class MapEffectTreeView : TreeView
    {
        public MapEffectTreeView(TreeViewState treeViewState, MapFile mapFile) : base(treeViewState)
        {
            this.mapFile = mapFile;
            Reload();
        }

        public MapFile mapFile;
        public MapEffectTreeViewItem selectedItem;

        protected override TreeViewItem BuildRoot()
        {
            MapEffectTreeViewItem root = new MapEffectTreeViewItem(0, -1);
            Type type = mapFile.GetType();

            {
                int id = 0;
                ReflectionRecursion(type, root, mapFile, ref id);
            }

            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

        void ReflectionRecursion(Type type, TreeViewItem item, object targetObject, ref int id)
        {
            PropertyInfo[] propertyInfos = type.GetProperties();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                PropertyInfo propertyInfo = propertyInfos[i];
                Type propertyType = propertyInfo.PropertyType;

                if (!propertyInfo.CanRead)
                    continue;

                if (typeof(IBeatValuePairAniList).IsAssignableFrom(propertyType))
                    AddChild(ref id);
                else if (typeof(IBeatValuePairList).IsAssignableFrom(propertyType))
                    AddChild(ref id);
                else
                {
                    if (propertyType == typeof(bool)
                        || propertyType == typeof(byte)
                        || propertyType == typeof(sbyte)
                        || propertyType == typeof(short)
                        || propertyType == typeof(ushort)
                        || propertyType == typeof(int)
                        || propertyType == typeof(uint)
                        || propertyType == typeof(long)
                        || propertyType == typeof(ulong)
                        || propertyType == typeof(float)
                        || propertyType == typeof(double)
                        || propertyType == typeof(decimal)
                        || propertyType == typeof(nint)
                        || propertyType == typeof(nuint)
                        || propertyType == typeof(char)
                        || propertyType == typeof(string)
                        || propertyType == typeof(BigInteger)
                        || propertyType == typeof(BigDecimal)
                        || propertyType == typeof(JVector2)
                        || propertyType == typeof(JVector3)
                        || propertyType == typeof(JVector4)
                        || propertyType == typeof(JRect)
                        || propertyType == typeof(JColor)
                        || propertyType == typeof(JColor32)
                        || propertyType == typeof(AnimationCurve)
                        || typeof(IList).IsAssignableFrom(propertyType))
                        AddChild(ref id);
                    else if (typeof(ICollection).IsAssignableFrom(propertyType))
                        continue;
                    else
                        ReflectionRecursion(propertyType, AddChild(ref id), propertyInfo.GetValue(targetObject), ref id);
                }

                MapEffectTreeViewItem AddChild(ref int id)
                {
                    id++;

                    MapEffectTreeViewItem childItem = new MapEffectTreeViewItem(id, 0, propertyInfo.Name);

                    childItem.targetObject = targetObject;
                    childItem.propertyInfo = propertyInfo;

                    item.AddChild(childItem);
                    return childItem;
                }
            }
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            if (selectedIds.Count == 1)
                selectedItem = (MapEffectTreeViewItem)FindItem(selectedIds[0], rootItem);
            else
                selectedItem = null;
        }
    }

    public class MapEffectTreeViewItem : TreeViewItem
    {
        public MapEffectTreeViewItem(int id) : base(id) { }
        public MapEffectTreeViewItem(int id, int depth) : base(id, depth) { }
        public MapEffectTreeViewItem(int id, int depth, string displayName) : base(id, depth, displayName) { }

        public object targetObject;
        public PropertyInfo propertyInfo;
    }
}
