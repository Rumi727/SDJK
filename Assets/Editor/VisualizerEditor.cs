using SCKRM.Editor;
using SDJK.MainMenu;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SDJK.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Visualizer))]
    public class VisualizerEditor : CustomInspectorEditor
    {
        Visualizer editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (Visualizer)target;
        }

        public override void OnInspectorGUI()
        {
            UseProperty("barPrefab", "바 프리팹");

            Space();

            UseProperty("_circle");

            Space();

            UseProperty("_all", "한번에 변경");

            Space();

            if (!editor.all)
            {
                UseProperty("_left", "왼쪽으로 애니메이션");
                UseProperty("_divide", "분할");

                Space();
            }

            UseProperty("_offset");
            UseProperty("_size");

            Space();

            UseProperty("_length");
        }
    }
}
