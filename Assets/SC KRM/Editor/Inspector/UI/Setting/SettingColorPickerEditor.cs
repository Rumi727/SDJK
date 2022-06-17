using SCKRM.UI.Setting;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SettingColorPicker))]
    public class SettingColorPickerEditor : SettingEditor
    {
        [System.NonSerialized] SettingColorPicker editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (SettingColorPicker)target;
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