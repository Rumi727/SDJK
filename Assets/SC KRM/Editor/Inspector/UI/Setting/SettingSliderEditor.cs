using SCKRM.UI.Setting;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SettingSlider))]
    public class SettingSliderEditor : SettingInputFieldEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawLine();

            UseProperty("_onValueChanged");
        }
    }
}