//MIT License
//Copyright (c) 2020 Mohammed Iqubal Hussain
//Website : Polyandcode.com 

using UnityEngine.UI;
using UnityEditor.AnimatedValues;
using UnityEditor;
using UnityEngine;
using UnityEditor.UI;

namespace PolyAndCode.UI
{
    [CustomEditor(typeof(RecyclableScrollRectScrollBar), true)]
    [CanEditMultipleObjects]
    /// <summary>
    /// Custom Editor for the Recyclable Scroll Rect Component which is derived from Scroll Rect.
    /// </summary>

    public class RecyclableScrollRectScrollBarEditor : ScrollbarEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            SerializedProperty tps = serializedObject.FindProperty("recyclableScrollRect");
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(tps, true);

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}