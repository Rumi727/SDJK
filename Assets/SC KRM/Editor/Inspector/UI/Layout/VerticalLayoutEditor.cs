using SCKRM.UI.Layout;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(VerticalLayout))]
    public class VerticalLayoutEditor : UIEditor
    {
        [System.NonSerialized] VerticalLayout editor;
        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (VerticalLayout)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawLine();

            UseProperty("_padding");

            EditorGUILayout.Space();

            UseProperty("_spacing");

            EditorGUILayout.Space();

            UseProperty("_onlyPos", "좌표만 변경");
            UseProperty("_lerp", "애니메이션 사용");
            if (editor.lerp)
            {
                UseProperty("_lerpValue", "애니메이션 속도");

                EditorGUILayout.Space();

                UseProperty("_allLerp", "X 좌표도 애니메이션 사용");
            }

            EditorGUILayout.Space();

            UseProperty("_ignore", "무시");
        }
    }
}