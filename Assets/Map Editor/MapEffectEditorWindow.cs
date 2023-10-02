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
using Object = UnityEngine.Object;

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
            timelineWindow = new MapEffectEditorTimelineWindow(this);
        }

        public MapEditorWindow editor;
        public MapFile mapFile;
        public MapEffectTreeView treeView;
        public MapEffectEditorTimelineWindow timelineWindow;

        public EditorGUISplitView splitView;


        Vector2 scrollPosition;
        int displayRestrictionsIndex;
        public void OnGUI()
        {
            splitView.BeginSplitView();
            treeView.OnGUI(EditorGUILayout.GetControlRect(GUILayout.Width(splitView.availableRect.width * splitView.splitNormalizedPosition - 7), GUILayout.ExpandHeight(true)));

            splitView.Split();

            bool isMapChanged = false;
            bool isListChanged = false;

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (!timelineWindow.OnGUI(treeView.selectedItems) && treeView.selectedItem != null)
            {
                object targetObject = treeView.selectedItem.targetObject;
                PropertyInfo propertyInfo = treeView.selectedItem.propertyInfo;

                DrawClass(targetObject, propertyInfo, 0, ref isMapChanged, ref isListChanged);
            }

            if (isMapChanged)
                RhythmManager.MapChange(mapFile.globalEffect.bpm, mapFile.info.songOffset, mapFile.globalEffect.yukiMode, mapFile.info.clearBeat);
            if (isListChanged)
                treeView = new MapEffectTreeView(treeView.state, mapFile);

            EditorGUILayout.EndScrollView();
            splitView.EndSplitView();
        }

        public void DrawClass(object targetObject, PropertyInfo propertyInfo, int tab, ref bool isMapChanged, ref bool isListChanged)
        {
            InfiniteLoopDetector.Run();

            EditorGUILayout.BeginHorizontal();
            CustomInspectorEditor.TabSpace(tab);

            GUILayout.Label(propertyInfo.Name, EditorStyles.boldLabel);

            EditorGUILayout.EndHorizontal();

            if (propertyInfo.GetCustomAttributes<JsonIgnoreAttribute>().Count() > 0 || !propertyInfo.CanWrite)
                GUI.enabled = false;

            EditorGUI.BeginChangeCheck();

            (isMapChanged, isListChanged) = DrawField(propertyInfo.Name, propertyInfo.PropertyType, propertyInfo.GetValue(targetObject), out object value, false, tab + 1, isMapChanged, isListChanged);

            if (EditorGUI.EndChangeCheck())
            {
                isMapChanged = true;
                if (propertyInfo.CanWrite)
                    propertyInfo.SetValue(targetObject, value);
            }

            GUI.enabled = true;
        }

        public (bool isMapChanged, bool isListChanged) DrawBeatValuePair(string fieldName, Type type, object currentObject, out object outObject, bool isList, int tab, bool isMapChanged, bool isListChanged, bool beatMixed, out bool beatChange, bool valueMixed, out bool valueChange, bool disturbanceMixed, out bool disturbanceChange, bool lengthMixed, out bool lengthChange, bool easingFunctionMixed, out bool easingFuctionChange)
        {
            beatChange = false;
            valueChange = false;
            disturbanceChange = false;
            lengthChange = false;
            easingFuctionChange = false;

            if (typeof(IBeatValuePairAni).IsAssignableFrom(type))
            {
                IBeatValuePairAni pair = (IBeatValuePairAni)Activator.CreateInstance(type);
                {
                    IBeatValuePairAni orgPair = (IBeatValuePairAni)currentObject;
                    pair.beat = orgPair.beat;
                    pair.value = orgPair.value;
                    pair.length = orgPair.length;
                    pair.easingFunction = orgPair.easingFunction;
                    pair.disturbance = orgPair.disturbance;
                }

                EditorGUI.showMixedValue = beatMixed;
                EditorGUILayout.BeginHorizontal();

                CustomInspectorEditor.TabSpace(tab);

                EditorGUI.BeginChangeCheck();
                pair.beat = EditorGUILayout.DoubleField(nameof(pair.beat), pair.beat);
                beatChange = EditorGUI.EndChangeCheck();
                isMapChanged = isMapChanged || beatChange;

                EditorGUILayout.EndHorizontal();
                EditorGUI.showMixedValue = false;

                EditorGUI.showMixedValue = valueMixed;
                (bool isMapChanged, bool isListChanged) result = DrawField(nameof(pair.value), pair.type, pair.value, out object outValue, isList, tab, false, false);
                EditorGUI.showMixedValue = false;

                valueChange = result.isMapChanged;
                isMapChanged = isMapChanged || valueChange;
                isListChanged = isListChanged || result.isListChanged;

                pair.value = outValue;

                EditorGUI.showMixedValue = lengthMixed;
                EditorGUILayout.BeginHorizontal();

                CustomInspectorEditor.TabSpace(tab);

                EditorGUI.BeginChangeCheck();
                pair.length = EditorGUILayout.DoubleField(nameof(pair.length), pair.length);
                lengthChange = EditorGUI.EndChangeCheck();
                isMapChanged = isMapChanged || lengthChange;

                EditorGUILayout.EndHorizontal();
                EditorGUI.showMixedValue = false;

                EditorGUI.showMixedValue = easingFunctionMixed;
                EditorGUILayout.BeginHorizontal();

                CustomInspectorEditor.TabSpace(tab);

                GUILayout.Label(nameof(pair.easingFunction), GUILayout.ExpandWidth(false));

                EditorGUI.BeginChangeCheck();
                pair.easingFunction = (EasingFunction.Ease)EditorGUILayout.EnumPopup(pair.easingFunction);
                easingFuctionChange = EditorGUI.EndChangeCheck();
                isMapChanged = isMapChanged || easingFuctionChange;

                EditorGUILayout.EndHorizontal();
                EditorGUI.showMixedValue = false;

                EditorGUI.showMixedValue = disturbanceMixed;
                EditorGUILayout.BeginHorizontal();

                CustomInspectorEditor.TabSpace(tab);

                GUILayout.Label(nameof(pair.disturbance), GUILayout.ExpandWidth(false));

                EditorGUI.BeginChangeCheck();
                pair.disturbance = EditorGUILayout.Toggle(pair.disturbance, GUILayout.ExpandWidth(false));
                disturbanceChange = EditorGUI.EndChangeCheck();
                isMapChanged = isMapChanged || disturbanceChange;

                EditorGUILayout.EndHorizontal();
                EditorGUI.showMixedValue = false;

                outObject = pair;
                return (isMapChanged, isListChanged);
            }
            else if (typeof(IBeatValuePair).IsAssignableFrom(type))
            {
                IBeatValuePair pair = (IBeatValuePair)Activator.CreateInstance(type);
                {
                    IBeatValuePair orgPair = (IBeatValuePair)currentObject;
                    pair.beat = orgPair.beat;
                    pair.value = orgPair.value;
                    pair.disturbance = orgPair.disturbance;
                }

                EditorGUI.showMixedValue = beatMixed;
                EditorGUILayout.BeginHorizontal();

                CustomInspectorEditor.TabSpace(tab);

                EditorGUI.BeginChangeCheck();
                pair.beat = EditorGUILayout.DoubleField(nameof(pair.beat), pair.beat);
                beatChange = EditorGUI.EndChangeCheck();
                isMapChanged = isMapChanged || beatChange;

                EditorGUILayout.EndHorizontal();
                EditorGUI.showMixedValue = false;

                EditorGUI.showMixedValue = valueMixed;
                (bool isMapChanged, bool isListChanged) result = DrawField(nameof(pair.value), pair.type, pair.value, out object outValue, isList, tab, false, false);
                EditorGUI.showMixedValue = false;

                valueChange = result.isMapChanged;
                isMapChanged = isMapChanged || valueChange;
                isListChanged = isListChanged || result.isListChanged;

                pair.value = outValue;

                EditorGUI.showMixedValue = disturbanceMixed;
                EditorGUILayout.BeginHorizontal();

                CustomInspectorEditor.TabSpace(tab);

                GUILayout.Label(nameof(pair.disturbance), GUILayout.ExpandWidth(false));

                EditorGUI.BeginChangeCheck();
                pair.disturbance = EditorGUILayout.Toggle(pair.disturbance, GUILayout.ExpandWidth(false));
                disturbanceChange = EditorGUI.EndChangeCheck();
                isMapChanged = isMapChanged || disturbanceChange;

                EditorGUILayout.EndHorizontal();
                EditorGUI.showMixedValue = false;

                outObject = pair;
                return (isMapChanged, isListChanged);
            }
            else
            {
                outObject = currentObject;
                return (isMapChanged, isListChanged);
            }
        }
        public (bool isMapChanged, bool isListChanged) DrawField(string fieldName, Type type, object currentObject, out object outObject, bool isList, int tab, bool isMapChanged, bool isListChanged)
        {
            InfiniteLoopDetector.Run();
            EditorGUI.BeginChangeCheck();

            if (type == typeof(bool))
            {
                EditorGUILayout.BeginHorizontal();
                CustomInspectorEditor.TabSpace(tab);

                outObject = EditorGUILayout.Toggle(fieldName, (bool)currentObject);

                EditorGUILayout.EndHorizontal();
                return (EditorGUI.EndChangeCheck() || isMapChanged, isListChanged);
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
                outObject = LongField();
                return (EditorGUI.EndChangeCheck() || isMapChanged, isListChanged);
            }
            else if (type == typeof(float) || type == typeof(double) || type == typeof(decimal) || type == typeof(BigDecimal))
            {
                outObject = DecimalField();
                return (EditorGUI.EndChangeCheck() || isMapChanged, isListChanged);
            }
            else if (type == typeof(string))
            {
                EditorGUILayout.BeginHorizontal();
                CustomInspectorEditor.TabSpace(tab);

                if (!string.IsNullOrEmpty(fieldName))
                    GUILayout.Label(fieldName, GUILayout.ExpandWidth(false));

                outObject = EditorGUILayout.TextArea((string)currentObject);

                EditorGUILayout.EndHorizontal();
                return (EditorGUI.EndChangeCheck() || isMapChanged, isListChanged);
            }
            else if (type == typeof(JVector2))
            {
                EditorGUILayout.BeginHorizontal();
                CustomInspectorEditor.TabSpace(tab);

                outObject = (JVector2)EditorGUILayout.Vector2Field(fieldName, (JVector2)currentObject);

                EditorGUILayout.EndHorizontal();
                return (EditorGUI.EndChangeCheck() || isMapChanged, isListChanged);
            }
            else if (type == typeof(JVector3))
            {
                EditorGUILayout.BeginHorizontal();
                CustomInspectorEditor.TabSpace(tab);

                outObject = (JVector3)EditorGUILayout.Vector3Field(fieldName, (JVector3)currentObject);

                EditorGUILayout.EndHorizontal();
                return (EditorGUI.EndChangeCheck() || isMapChanged, isListChanged);
            }
            else if (type == typeof(JVector4))
            {
                EditorGUILayout.BeginHorizontal();
                CustomInspectorEditor.TabSpace(tab);

                outObject = (JVector4)EditorGUILayout.Vector4Field(fieldName, (JVector4)currentObject);

                EditorGUILayout.EndHorizontal();
                return (EditorGUI.EndChangeCheck() || isMapChanged, isListChanged);
            }
            else if (type == typeof(JRect))
            {
                EditorGUILayout.BeginHorizontal();
                CustomInspectorEditor.TabSpace(tab);

                outObject = (JRect)EditorGUILayout.RectField(fieldName, (JRect)currentObject);

                EditorGUILayout.EndHorizontal();
                return (EditorGUI.EndChangeCheck() || isMapChanged, isListChanged);
            }
            else if (type == typeof(JColor))
            {
                EditorGUILayout.BeginHorizontal();
                CustomInspectorEditor.TabSpace(tab);

                outObject = (JColor)EditorGUILayout.ColorField(fieldName, (JColor)currentObject);

                EditorGUILayout.EndHorizontal();
                return (EditorGUI.EndChangeCheck() || isMapChanged, isListChanged);
            }
            else if (type == typeof(JColor32))
            {
                EditorGUILayout.BeginHorizontal();
                CustomInspectorEditor.TabSpace(tab);

                outObject = (JColor32)EditorGUILayout.ColorField(fieldName, (JColor32)currentObject);

                EditorGUILayout.EndHorizontal();
                return (EditorGUI.EndChangeCheck() || isMapChanged, isListChanged);
            }
            else if (type == typeof(AnimationCurve))
            {
                EditorGUILayout.BeginHorizontal();
                CustomInspectorEditor.TabSpace(tab);

                outObject = EditorGUILayout.CurveField(fieldName, (AnimationCurve)currentObject);

                EditorGUILayout.EndHorizontal();
                return (EditorGUI.EndChangeCheck() || isMapChanged, isListChanged);
            }
            else if (typeof(Enum).IsAssignableFrom(type))
            {
                EditorGUILayout.BeginHorizontal();
                CustomInspectorEditor.TabSpace(tab);

                outObject = EditorGUILayout.EnumPopup(fieldName, (Enum)currentObject);

                EditorGUILayout.EndHorizontal();
                return (EditorGUI.EndChangeCheck() || isMapChanged, isListChanged);
            }
            else if (typeof(ITypeList).IsAssignableFrom(type))
            {
                EditorGUI.EndChangeCheck();

                object value = currentObject;
                ITypeList list = (ITypeList)value;

                int lastCount = list.Count;
                displayRestrictionsIndex = CustomInspectorEditor.DrawList(list.listType, list, "", Top, StringDefault, tab, tab + 1, true, 30, displayRestrictionsIndex);

                if (list.listType.IsClass && lastCount != list.Count)
                    isListChanged = true;

                object Top(object currentObject)
                {
                    (isMapChanged, isListChanged) = DrawField("", list.listType, currentObject, out object result, true, tab + 1, isMapChanged, isListChanged);
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
                return (isMapChanged, isListChanged);
            }
            else if (typeof(ICollection).IsAssignableFrom(type))
            {
                outObject = currentObject;
                return (isMapChanged, isListChanged);
            }
            else if (typeof(IBeatValuePair).IsAssignableFrom(type))
            {
                DrawBeatValuePair(fieldName, type, currentObject, out outObject, isList, tab, isMapChanged, isListChanged, false, out bool beatChange, false, out bool valueChange, false, out bool disturbanceChange, false, out bool lengthChange, false, out bool easingFunctionChange);
                return (isMapChanged || beatChange || valueChange || disturbanceChange || lengthChange || disturbanceChange, isListChanged);
            }
            else
            {
                PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    PropertyInfo propertyInfo2 = propertyInfos[i];
                    DrawClass(currentObject, propertyInfo2, tab, ref isMapChanged, ref isListChanged);

                    if (i < propertyInfos.Length - 1 && !isList)
                        CustomInspectorEditor.DrawLine(2);
                }

                outObject = currentObject;
                return (isMapChanged, isListChanged);
            }

            object LongField()
            {
                EditorGUILayout.BeginHorizontal();
                CustomInspectorEditor.TabSpace(tab);

                unchecked
                {
                    long value = (long)Convert.ChangeType(currentObject, typeof(long));

                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        long value2 = EditorGUILayout.LongField(fieldName, value);

                        EditorGUILayout.EndHorizontal();
                        return Convert.ChangeType(value2, type);
                    }
                    else
                    {
                        long value2 = EditorGUILayout.LongField(value);

                        EditorGUILayout.EndHorizontal();
                        return Convert.ChangeType(value2, type);
                    }
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
