using SCKRM.UI;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SystemColorApply))]
    public class SystemColorApplyEditor : UIEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UseProperty("_offset");
        }
    }
}