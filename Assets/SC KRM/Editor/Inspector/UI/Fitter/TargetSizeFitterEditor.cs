using SCKRM.UI;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TargetSizeFitter))]
    public class TargetSizeFitterEditor : UIEditor
    {
        [System.NonSerialized] TargetSizeFitter editor;
        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (TargetSizeFitter)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UseProperty("_targetRectTransforms", "대상");

            Space();

            UseProperty("_xSize", "X 크기 변경");
            UseProperty("_ySize", "Y 크기 변경");

            Space();

            UseProperty("_offset");
            UseProperty("_min");
            UseProperty("_max");

            Space();

            UseProperty("_reversal", "반전");

            Space();

            UseProperty("_lerp", "애니메이션 사용");

            if (editor.lerp)
            {
                UseProperty("_lerpValue", "애니메이션 속도");

                Space();

                UseProperty("_awakeNoAni", "시작할 때 애니메이션 무시");
            }

            Space();

            UseProperty("_disabledObjectIgnore", "비활성화 된 오브젝트 무시");
        }
    }
}