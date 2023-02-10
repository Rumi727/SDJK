using SCKRM.UI;
using UnityEditor;
using UnityEngine;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UI.UIBase))]
    public class UIEditor : CustomInspectorEditor
    {
        [System.NonSerialized] IUI editor;
        [System.NonSerialized] GameObject editorGameObject;

        protected override void OnEnable()
        {
            base.OnEnable();
            editorGameObject = ((Component)target).gameObject;
            editor = target as IUI;
        }

        /// <summary>
        /// Please put base.OnInspectorGUI() when overriding
        /// </summary>
        public override void OnInspectorGUI()
        {
            bool lineShow = false;

            if (editor == null)
            {
                EditorGUILayout.HelpBox("컴포넌트가 IUI 인터페이스를 상속하지 않지만\n커스텀 인스펙터 에디터 스크립트는 UIEditor 클래스를 상속합니다", MessageType.Warning);
                DrawLine();

                return;
            }

            if (editor.rectTransform == null)
                return;

            if (editor.rectTransform.gameObject != editorGameObject)
            {
                EditorGUILayout.HelpBox("이 게임 오브젝트에 있는 RectTramsform 컴포넌트를 넣어야합니다!", MessageType.Error);
                UseProperty("_rectTransform");

                lineShow = true;
            }

            if (editor.graphic != null && editor.graphic.gameObject != editorGameObject)
            {
                EditorGUILayout.HelpBox("이 게임 오브젝트에 있는 그래픽 컴포넌트를 넣어야합니다!", MessageType.Error);
                UseProperty("_graphic");

                lineShow = true;
            }

            if (lineShow)
                DrawLine();
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIObjectPoolingBase))]
    public class ObjectPoolingUIEditor : UIEditor
    {
        public override void OnInspectorGUI() => base.OnInspectorGUI();
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIAniBase))]
    public class UIAniEditor : UIEditor
    {
        UIAniBase editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (UIAniBase)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UseProperty("_lerp", "애니메이션 사용");

            if (editor.lerp)
            {
                UseProperty("_lerpValue", "애니메이션 속도");

                Space();

                UseProperty("_awakeNoAni", "시작할 때 애니메이션 무시");
            }
        }
    }
}