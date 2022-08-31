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

            UseProperty("_padding");

            Space();

            UseProperty("_spacing");

            Space();

            UseProperty("_onlyPos", "좌표만 변경");

            Space();

            UseProperty("_lerp", "애니메이션 사용");

            if (editor.lerp)
            {
                UseProperty("_lerpValue", "애니메이션 속도");

                Space();

                UseProperty("_awakeNoAni", "시작할 때 애니메이션 무시");
            }

            Space();

            UseProperty("_ignore", "무시");
        }
    }
}