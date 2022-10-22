using SCKRM.UI.Layout;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ChildSizeFitter))]
    public class ChildSizeFitterEditor : UIEditor
    {
        [System.NonSerialized] ChildSizeFitter editor;
        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (ChildSizeFitter)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UseProperty("_xSize", "X 크기 변경");
            UseProperty("_ySize", "Y 크기 변경");

            EditorGUILayout.Space();

            UseProperty("_spacing");
            UseProperty("_offset");
            UseProperty("_min");
            UseProperty("_max");

            EditorGUILayout.Space();

            UseProperty("_lerp", "애니메이션 사용");

            if (editor.lerp)
            {
                UseProperty("_lerpValue", "애니메이션 속도");

                Space();

                UseProperty("_awakeNoAni", "시작할 때 애니메이션 무시");
            }

            EditorGUILayout.Space();

            UseProperty("_disabledObjectIgnore", "비활성화 된 오브젝트 무시");
            UseProperty("_ignore", "무시");
        }
    }
}