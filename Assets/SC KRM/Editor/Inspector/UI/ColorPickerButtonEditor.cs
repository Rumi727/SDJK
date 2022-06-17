using SCKRM.UI;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ColorPickerButton))]
    public class ColorPickerButtonEditor : UIAniEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawLine();

            UseProperty("colorPickerMask");
            UseProperty("colorPickerRectTransform");
            UseProperty("colorPicker");
        }
    }
}