using SCKRM.UI;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DropdownItem))]
    public class DropdownItemEditor : ObjectPoolingUIEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UseProperty("_label");
            UseProperty("_toggle");
        }
    }
}