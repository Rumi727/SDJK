using UnityEngine;
using UnityEditor;
using SCKRM.Renderer;
using SCKRM.Resource;
using SCKRM.Language;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CustomTextRendererBase), true)]
    public class CustomAllTextRendererEditor : CustomInspectorEditor
    {
        CustomTextRendererBase editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (CustomTextRendererBase)target;
        }

        public override void OnInspectorGUI()
        {
            string[] languageKeys = ResourceManager.GetLanguageKeys(LanguageManager.SaveData.currentLanguage, editor.nameSpace);
            for (int i = 0; i < languageKeys.Length; i++)
                languageKeys[i] = languageKeys[i].Replace(".", "/");

            editor.nameSpace = UsePropertyAndDrawNameSpace("_nameSpace", "네임스페이스", editor.nameSpace);
            editor.path = UsePropertyAndDrawStringArray("_path", "이름", editor.path.Replace(".", "/"), languageKeys).Replace("/", ".");

            EditorGUILayout.Space();

            if (GUI.changed || GUILayout.Button("새로고침"))
            {
                EditorUtility.SetDirty(target);

                if (editor.enabled)
                    editor.Refresh();
            }

            GUI.enabled = true;
        }
    }
}