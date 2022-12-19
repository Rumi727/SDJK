using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Kernel), true)]
    public class KernelEditor : CustomInspectorEditor
    {
        static SCKRMWindowTabDefault window = new();
        public override void OnInspectorGUI() => SCKRMWindowTabDefault.Render(window);
    }
}