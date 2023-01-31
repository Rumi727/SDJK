using SCKRM.SaveLoad.UI;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SaveLoadUIToggle))]
    public class SaveLoadUIToggleEditor : SaveLoadUIBaseEditor
    {
        [System.NonSerialized] SaveLoadUIToggle editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (SaveLoadUIToggle)target;
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