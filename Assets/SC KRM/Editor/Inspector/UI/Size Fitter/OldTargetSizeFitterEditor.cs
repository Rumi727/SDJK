#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
using SCKRM.UI;
using UnityEditor;
using UnityEngine;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(OldTargetSizeFitter))]
    public class OldTargetSizeFitterEditor : UIEditor
    {
        [System.NonSerialized] OldTargetSizeFitter editor;
        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (OldTargetSizeFitter)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawLine();

            UseProperty("_targetRectTransform", "대상");

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
                UseProperty("_lerpValue", "애니메이션 속도");

            Space();

            if (GUILayout.Button("Target Size Fitter Change"))
            {
                TargetSizeFitter targetSizeFitter = editor.gameObject.AddComponent<TargetSizeFitter>();
                targetSizeFitter.hideFlags = editor.hideFlags;
                targetSizeFitter.enabled = editor.enabled;
                targetSizeFitter.useGUILayout = editor.useGUILayout;
                targetSizeFitter.runInEditMode = editor.runInEditMode;


                targetSizeFitter.targetRectTransforms = new RectTransform[] { editor.targetRectTransform };

                targetSizeFitter.xSize = editor.xSize;
                targetSizeFitter.ySize = editor.ySize;

                targetSizeFitter.offset = editor.offset;
                targetSizeFitter.min = editor.min;
                targetSizeFitter.max = editor.max;

                targetSizeFitter.reversal = editor.reversal;

                targetSizeFitter.lerp = editor.lerp;
                targetSizeFitter.lerpValue = editor.lerpValue;

                EditorUtility.SetDirty(target);
                DestroyImmediate(editor);
            }
        }
    }
}
#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.