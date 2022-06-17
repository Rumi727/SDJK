using SCKRM.UI.Layout;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LayoutChild))]
    public class LayoutChildEditor : UIAniEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawLine();

            UseProperty("_spacing");

            EditorGUILayout.Space();

            UseProperty("_ignore", "무시");
        }
    }
}