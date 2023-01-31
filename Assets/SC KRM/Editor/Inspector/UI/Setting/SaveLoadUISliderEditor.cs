using SCKRM.SaveLoad.UI;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SaveLoadUISlider))]
    public class SaveLoadUISliderEditor : SaveLoadUIInputFieldEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawLine();

            UseProperty("_onValueChanged");
        }
    }
}