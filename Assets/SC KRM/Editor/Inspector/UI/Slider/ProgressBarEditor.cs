using SCKRM.UI;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ProgressBar))]
    public class ProgressBarEditor : UIAniEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawLine();

            UseProperty("_progress");
            UseProperty("_maxProgress", "최대 진행도");

            DrawLine();

            UseProperty("_slider");
            UseProperty("_fillShow");
        }
    }
}