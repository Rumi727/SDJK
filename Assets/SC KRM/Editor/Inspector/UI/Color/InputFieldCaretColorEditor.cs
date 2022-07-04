using SCKRM.UI;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(InputFieldCaretColor))]
    public class InputFieldCaretColorEditor : UIEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UseProperty("_inputField");
        }
    }
}