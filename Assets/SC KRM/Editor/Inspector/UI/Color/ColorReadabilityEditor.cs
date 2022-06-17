using SCKRM.UI;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ColorReadability))]
    public class ColorReadabilityEditor : UIAniEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawLine();

            UseProperty("_targetCanvasRenderer");
            UseProperty("_targetGraphic");
        }
    }
}