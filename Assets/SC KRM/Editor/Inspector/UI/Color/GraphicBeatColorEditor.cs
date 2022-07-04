using SCKRM.UI;
using UnityEditor;
using UnityEngine;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GraphicBeatColor))]
    public class GraphicBeatColorEditor : UIEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UseProperty("_alpha");

            if (Kernel.isPlaying)
                GUI.enabled = false;

            UseProperty("_dropPartMode");

            GUI.enabled = true;
        }
    }
}