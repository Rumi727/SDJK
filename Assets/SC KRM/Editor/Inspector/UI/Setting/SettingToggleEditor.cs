using SCKRM.UI.Setting;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SettingToggle))]
    public class SettingToggleEditor : SettingEditor
    {
        [System.NonSerialized] SettingToggle editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (SettingToggle)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawLine();

            UseProperty("_toggle");

            DrawLine();

            UseProperty("_onValueChanged");
        }
    }
}