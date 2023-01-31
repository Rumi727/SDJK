using SCKRM.SaveLoad.UI;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SaveLoadUITitle))]
    public class SaveLoadUITitleEditor : UIEditor
    {
        public override void OnInspectorGUI() => UseProperty("_customTextMeshProRenderer");
    }
}