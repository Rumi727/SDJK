using SCKRM.SaveLoad.UI;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SaveLoadUIDropdown))]
    public class SaveLoadUIDropdownEditor : SaveLoadUIBaseEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawLine();

            UseProperty("_dropdown");

            DrawLine();

            UseProperty("_onValueChanged");
        }
    }
}