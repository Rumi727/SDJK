using UnityEngine;
using UnityEditor;
using SCKRM.Renderer;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CustomSpriteRenderer), true)]
    public class CustomSpriteRendererEditor : CustomAllSpriteRendererEditor
    {
        CustomSpriteRenderer editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (CustomSpriteRenderer)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            UseProperty("_drawMode");
            if (editor.drawMode != SpriteDrawMode.Simple)
                UseProperty("_size");
        }
    }
}