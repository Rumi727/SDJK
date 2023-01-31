using SCKRM.SaveLoad.UI;
using UnityEditor;
using UnityEngine;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SaveLoadUIDisableCondition))]
    public class SaveLoadUIDisableConditionEditor : CustomInspectorEditor
    {
        [System.NonSerialized] SaveLoadUIDisableCondition editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (SaveLoadUIDisableCondition)target;
        }

        public override void OnInspectorGUI()
        {
            UseProperty("_setting", "감지 할 설정");
            UseProperty("_disableGameObject", "비활성화 할 오브젝트");

            DrawLine();

            UseProperty("_reversal", "반전");

            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }
    }
}