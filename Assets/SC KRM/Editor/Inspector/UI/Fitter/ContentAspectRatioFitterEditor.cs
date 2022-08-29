using SCKRM.UI;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ContentAspectRatioFitter))]
    public class ContentAspectRatioFitterEditor : UIEditor
    {
        ContentAspectRatioFitter editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (ContentAspectRatioFitter)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UseProperty("m_AspectMode");

            if (editor)
            {
                if (!editor.IsAspectModeValid())
                    ShowNoParentWarning();
                if (!editor.IsComponentValidOnObject())
                    ShowCanvasRenderModeInvalidWarning();
            }
        }

        static void ShowNoParentWarning()
        {
            string text = L10n.Tr("You cannot use this Aspect Mode because this Component's GameObject does not have a parent object.");
            EditorGUILayout.HelpBox(text, MessageType.Warning);
        }

        static void ShowCanvasRenderModeInvalidWarning()
        {
            string text = L10n.Tr("You cannot use this Aspect Mode because this Component is attached to a Canvas with a fixed width and height.");
            EditorGUILayout.HelpBox(text, MessageType.Warning);
        }
    }
}