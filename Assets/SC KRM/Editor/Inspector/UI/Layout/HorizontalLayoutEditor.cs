using SCKRM.UI.Layout;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HorizontalLayout))]
    public class HorizontalLayoutEditor : UIEditor
    {
        [System.NonSerialized] HorizontalLayout editor;
        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (HorizontalLayout)target;
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

                UseProperty("_allLerp", "Y 좌표도 애니메이션 사용");
            }

            Space();

            UseProperty("_ignore", "무시");
        }
    }
}