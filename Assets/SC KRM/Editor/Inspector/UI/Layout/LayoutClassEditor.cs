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

            UseProperty("_spacing");

            EditorGUILayout.Space();

            UseProperty("_disabledObjectIgnore", "비활성화 된 오브젝트 무시");
            UseProperty("_ignore", "무시");
        }
    }
}