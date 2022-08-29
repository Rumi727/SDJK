using SCKRM.UI;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ContentAspectRatioFitter))]
    public class ContentAspectRatioFitterEditor : UIEditor
    {
        protected override void OnEnable() => base.OnEnable();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UseProperty("m_AspectMode");
        }
    }
}