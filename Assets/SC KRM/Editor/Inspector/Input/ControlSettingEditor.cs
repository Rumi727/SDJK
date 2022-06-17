using SCKRM.Input.UI;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ControlSetting))]
    public class ControlSettingEditor : UIEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawLine();

            UseProperty("_targetKey", "변경할 키");

            DrawLine();

            UseProperty("controlPanelRectTransform");
            UseProperty("resetButton");
            UseProperty("nameRectTransform");
            UseProperty("controlButtonImage");
            UseProperty("controlButtonText");
        }
    }
}