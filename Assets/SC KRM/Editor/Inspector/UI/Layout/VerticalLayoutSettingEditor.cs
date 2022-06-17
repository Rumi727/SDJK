using SCKRM.UI;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(VerticalLayoutSetting))]
    public class VerticalLayoutSettingEditor : CustomInspectorEditor
    {
        [System.NonSerialized] VerticalLayoutSetting editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (VerticalLayoutSetting)target;
        }

        public override void OnInspectorGUI() => UseProperty("_mode");
    }
}