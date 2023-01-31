using SCKRM.SaveLoad;
using SCKRM.SaveLoad.UI;
using UnityEditor;
using UnityEngine;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SaveLoadUIBase))]
    public class SaveLoadUIBaseEditor : UIEditor
    {
        public static SaveLoadClass[] saveLoadClassList;

        [System.NonSerialized] SaveLoadUIBase editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (SaveLoadUIBase)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (saveLoadClassList == null)
                SaveLoadManager.InitializeAll<GeneralSaveLoadAttribute>(out saveLoadClassList);

            SaveLoadClass selectedSaveLoadClass = null;

            string[] fullNames = new string[saveLoadClassList.Length];
            for (int i = 0; i < saveLoadClassList.Length; i++)
            {
                SaveLoadClass saveLoadClass = saveLoadClassList[i];
                string fullName = saveLoadClass.name;
                fullNames[i] = fullName;

                if (fullName == editor.saveLoadClassName)
                    selectedSaveLoadClass = saveLoadClass;
            }

            UseProperty("_autoRefresh", "자동 새로고침");

            DrawLine();

            editor.saveLoadClassName = UsePropertyAndDrawStringArray("_saveLoadClassName", "값을 변경 할 클래스", editor.saveLoadClassName, fullNames);

            if (selectedSaveLoadClass != null)
                editor.variableName = UsePropertyAndDrawStringArray("_variableName", "값을 변경 할 변수", editor.variableName, selectedSaveLoadClass.GetVariableNames());

            DrawLine();

            UseProperty("_roundingDigits", "반올림 자릿수");

            DrawLine();

            UseProperty("_hotkeyToDisplay", "표시할 단축키");

            DrawLine();

            UseProperty("_resetButton", "리셋 버튼");
            UseProperty("_nameText", "이름 텍스트");
            UseProperty("_nameTextRenderer", "이름 텍스트 렌더러");
            UseProperty("_tooltip", "툴팁");

            if (editor.propertyInfo != null)
                EditorGUILayout.LabelField(editor.type + " " + editor.propertyInfo.Name + " = " + editor.propertyInfo.GetValue(editor.type));
            else if (editor.fieldInfo != null)
                EditorGUILayout.LabelField(editor.type + " " + editor.fieldInfo.Name + " = " + editor.fieldInfo.GetValue(editor.type));

            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(SettingDrag))]
    public class SettingDragEditor : SaveLoadUIBaseEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawLine();

            UseProperty("_mouseSensitivity", "마우스 감도");
        }
    }
}