using SCKRM.UI;
using UnityEditor;
using UnityEngine;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AlphaHitTestMinimumThreshold))]
    public class AlphaHitTestMinimumThresholdEditor : UIEditor
    {
        [System.NonSerialized] AlphaHitTestMinimumThreshold editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (AlphaHitTestMinimumThreshold)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UseProperty("_alphaHitTestMinimumThreshold");
            if (GUI.changed)
                editor.alphaHitTestMinimumThreshold = editor.alphaHitTestMinimumThreshold;
        }
    }
}