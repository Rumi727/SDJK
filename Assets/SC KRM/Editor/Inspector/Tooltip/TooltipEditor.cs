using UnityEngine;
using UnityEditor;
using SCKRM.Resource;
using SCKRM.Language;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Tooltip.Tooltip), true)]
    public class TooltipEditor : CustomInspectorEditor
    {
        Tooltip.Tooltip editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (Tooltip.Tooltip)target;
        }

        public override void OnInspectorGUI()
        {
            string[] languageKeys = ResourceManager.GetLanguageKeys(LanguageManager.SaveData.currentLanguage, editor.nameSpace);
            for (int i = 0; i < languageKeys.Length; i++)
                languageKeys[i] = languageKeys[i].Replace(".", "/");

            editor.nameSpace = UsePropertyAndDrawNameSpace("_nameSpace", "네임스페이스", editor.nameSpace);
            editor.text = UsePropertyAndDrawStringArray("_text", "이름", editor.text.Replace(".", "/"), languageKeys).Replace("/", ".");

            EditorGUILayout.Space();

            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }
    }
}