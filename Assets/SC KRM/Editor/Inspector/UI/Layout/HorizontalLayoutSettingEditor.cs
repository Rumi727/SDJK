using SCKRM.UI;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HorizontalLayoutSetting))]
    public class HorizontalLayoutSettingEditor : CustomInspectorEditor
    {
        [System.NonSerialized] HorizontalLayoutSetting editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (HorizontalLayoutSetting)target;
        }

        public override void OnInspectorGUI() => UseProperty("_mode");
    }
}