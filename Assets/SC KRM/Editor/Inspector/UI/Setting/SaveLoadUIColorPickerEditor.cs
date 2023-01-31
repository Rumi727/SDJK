using SCKRM.SaveLoad.UI;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SaveLoadUIColorPicker))]
    public class SaveLoadUIColorPickerEditor : SaveLoadUIBaseEditor
    {
        [System.NonSerialized] SaveLoadUIColorPicker editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (SaveLoadUIColorPicker)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawLine();

            UseProperty("_colorPicker");

            DrawLine();

            UseProperty("_onValueChanged");
        }
    }
}