using SCKRM.Editor;
using SDJK.Effect;
using UnityEditor;
using UnityEngine;

namespace SDJK.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Visualizer))]
    public class VisualizerEditor : CustomInspectorEditor
    {
        Visualizer editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (Visualizer)target;
        }

        public override void OnInspectorGUI()
        {
            UseProperty("_effectManager");
            UseProperty("barPrefab", "바 프리팹");

            Space();

            UseProperty("_circle");

            Space();

            editor.left = EditorGUILayout.Toggle("왼쪽으로 애니메이션", editor.left);
            editor.divide = EditorGUILayout.IntField("분할", editor.divide);
            editor.speed = EditorGUILayout.FloatField("속도", editor.speed);

            Space();

            editor.offset = EditorGUILayout.IntField("오프셋", editor.offset);
            editor.size = EditorGUILayout.FloatField("크기", editor.size);

            Space();

            UseProperty("_length");

            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }
    }
}
