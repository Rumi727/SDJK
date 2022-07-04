using SCKRM.UI;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RectTransformTool))]
    public class RectTransformToolEditor : UIEditor
    {
        [System.NonSerialized] RectTransformTool editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (RectTransformTool)target;
        }

        /// <summary>
        /// Please put base.OnInspectorGUI() when overriding
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("Anchored Position: " + editor.rectTransform.anchoredPosition);
            EditorGUILayout.LabelField("Size Delta: " + editor.rectTransform.sizeDelta);

            Space();

            EditorGUILayout.LabelField("Offset Min: " + editor.rectTransform.offsetMin);
            EditorGUILayout.LabelField("Offset Max: " + editor.rectTransform.offsetMax);

            Space();

            EditorGUILayout.LabelField("Rect: " + editor.rectTransform.rect);

            Space();

            EditorGUILayout.LabelField("World Rect: " + editor.worldCorners.rect);
            /*
            DrawLine();

            UseProperty("onBeforeTransformParentChangedUnityEvent", "상위 트랜스폼이 변경되기 전에 호출됩니다");
            UseProperty("onTransformParentChangedUnityEvent", "상위 트랜스폼이 변경되면 호출됩니다");

            DrawLine();

            UseProperty("onRectTransformDimensionsChangeUnityEvent", "RectTransform의 월드 값이 변경되면 호출됩니다");
            UseProperty("onDidApplyAnimationPropertiesUnityEvent", "애니메이션으로 인해 속성이 변경된 경우를 위한 콜백입니다");

            DrawLine();

            UseProperty("onCanvasHierarchyChangedUnityEvent", "부모 캔버스의 상태가 변경되면 호출됩니다");
            UseProperty("onCanvasGroupChangedUnityEvent", "캔버스 그룹의 상태가 변경되면 호출됩니다");*/
        }
    }
}