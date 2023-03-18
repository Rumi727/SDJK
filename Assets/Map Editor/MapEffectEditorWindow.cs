using ExtendedNumerics;
using Newtonsoft.Json;
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

                Recursion(targetObject, propertyInfo);

                void Recursion(object targetObject, PropertyInfo propertyInfo)
                {
                    GUILayout.Label(propertyInfo.Name, EditorStyles.boldLabel);

                    if (propertyInfo.GetCustomAttributes<JsonIgnoreAttribute>().Count() > 0 || !propertyInfo.CanWrite)
                        GUI.enabled = false;

                    if (typeof(IList).IsAssignableFrom(propertyInfo.PropertyType) && typeof(ITypeList).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        object value = propertyInfo.GetValue(targetObject);

                        IList list = (IList)value;
                        ITypeList listType = (ITypeList)value;

                        displayRestrictionsIndex = CustomInspectorEditor.DrawList(listType.listType, list, "", Top, Bottom, StringDefault, 0, 0, true, 25, displayRestrictionsIndex);

                        object Top(object currentObject)
                        {
                            Field("", currentObject, out object result, true, true);
                            return result;
                        }

                        object Bottom(object currentObject)
                        {
                            Field("", currentObject, out object result, false, true);
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

                        if (propertyInfo.CanWrite)
                            propertyInfo.SetValue(targetObject, list);
                    }
                    else if (!typeof(ICollection).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        if (Field(propertyInfo.Name, propertyInfo.GetValue(targetObject), out object value, true, false))
                        {
                            if (propertyInfo.CanWrite)
                                propertyInfo.SetValue(targetObject, value);
                        }
                    }

                    GUI.enabled = true;

                    bool Field(string fieldName, object currentObject, out object outObject, bool top, bool isList)
                    {
                        Type type = currentObject.GetType();
                        
                        if (type == typeof(bool))
                        {
                            outObject = EditorGUILayout.Toggle(fieldName, (bool)currentObject);
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
                            if (!string.IsNullOrEmpty(fieldName))
                                GUILayout.Label(fieldName, GUILayout.ExpandWidth(false));

                            outObject = EditorGUILayout.TextArea((string)currentObject);
                            return true;
                        }
                        else if (type == typeof(JVector2))
                        {
                            outObject = (JVector2)EditorGUILayout.Vector2Field(fieldName, (JVector2)currentObject);
                            return true;
                        }
                        else if (type == typeof(JVector3))
                        {
                            outObject = (JVector3)EditorGUILayout.Vector3Field(fieldName, (JVector3)currentObject);
                            return true;
                        }
                        else if (type == typeof(JVector4))
                        {
                            outObject = (JVector4)EditorGUILayout.Vector4Field(fieldName, (JVector4)currentObject);
                            return true;
                        }
                        else if (type == typeof(JRect))
                        {
                            outObject = (JRect)EditorGUILayout.RectField(fieldName, (JRect)currentObject);
                            return true;
                        }
                        else if (type == typeof(JColor))
                        {
                            outObject = (JColor)EditorGUILayout.ColorField(fieldName, (JColor)currentObject);
                            return true;
                        }
                        else if (type == typeof(JColor32))
                        {
                            outObject = (JColor32)EditorGUILayout.ColorField(fieldName, (JColor32)currentObject);
                            return true;
                        }
                        else if (type == typeof(AnimationCurve))
                        {
                            outObject = EditorGUILayout.CurveField(fieldName, (AnimationCurve)currentObject);
                            return true;
                        }
                        else if (typeof(IBeatValuePairAni).IsAssignableFrom(type))
                        {
                            IBeatValuePairAni pair = (IBeatValuePairAni)currentObject;

                            if (top)
                            {
                                pair.beat = EditorGUILayout.DoubleField(nameof(pair.beat), pair.beat);

                                GUILayout.Label(nameof(pair.disturbance), GUILayout.ExpandWidth(false));
                                pair.disturbance = EditorGUILayout.Toggle(pair.disturbance, GUILayout.ExpandWidth(false));

                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();

                                pair.length = EditorGUILayout.DoubleField(nameof(pair.length), pair.length);

                                GUILayout.Label(nameof(pair.easingFunction), GUILayout.ExpandWidth(false));
                                pair.easingFunction = (EasingFunction.Ease)EditorGUILayout.EnumPopup(pair.easingFunction);
                            }
                            else
                            {
                                Field(nameof(pair.value), pair.value, out object outValue, top, isList);
                                pair.value = outValue;
                            }

                            outObject = pair;
                            return true;
                        }
                        else if (typeof(IBeatValuePair).IsAssignableFrom(type))
                        {
                            IBeatValuePair pair = (IBeatValuePair)currentObject;

                            if (top)
                            {
                                pair.beat = EditorGUILayout.DoubleField(nameof(pair.beat), pair.beat);

                                GUILayout.Label(nameof(pair.disturbance), GUILayout.ExpandWidth(false));
                                pair.disturbance = EditorGUILayout.Toggle(pair.disturbance, GUILayout.ExpandWidth(false));
                            }
                            else
                            {
                                Field(nameof(pair.value), pair.value, out object outValue, top, isList);
                                pair.value = outValue;
                            }

                            outObject = pair;
                            return true;
                        }
                        else
                        {
                            PropertyInfo[] propertyInfos = type.GetProperties();
                            for (int i = 0; i < propertyInfos.Length; i++)
                            {
                                PropertyInfo propertyInfo2 = propertyInfos[i];
                                Recursion(currentObject, propertyInfo2);

                                if (i < propertyInfos.Length - 1 && !isList)
                                    CustomInspectorEditor.DrawLine(2);
                            }

                            outObject = currentObject;
                            return true;
                        }
                        
                        object IntField()
                        {
                            int value = (int)Convert.ChangeType(currentObject, typeof(int));

                            if (!string.IsNullOrEmpty(fieldName))
                                return Convert.ChangeType(EditorGUILayout.IntField(fieldName, value), type);
                            else
                                return Convert.ChangeType(EditorGUILayout.IntField(value), type);
                        }

                        object DecimalField()
                        {
                            double value = (double)Convert.ChangeType(currentObject, typeof(double));

                            if (!string.IsNullOrEmpty(fieldName))
                                return Convert.ChangeType(EditorGUILayout.DoubleField(fieldName, value), type);
                            else
                                return Convert.ChangeType(EditorGUILayout.DoubleField(value), type);
                        }
                    }
                }
            }

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
