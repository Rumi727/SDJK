using SCKRM.SaveLoad;
using SCKRM.SaveLoad.UI;
using System;
using UnityEditor;
using UnityEngine;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SaveLoadUI))]
    [Obsolete("Incomplete!")]
    public class SaveLoadUIEditor : UIEditor
    {
        public static SaveLoadClass[] saveLoadClassList;

        [System.NonSerialized] SaveLoadUI editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (SaveLoadUI)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (saveLoadClassList == null)
                SaveLoadManager.InitializeAll<GeneralSaveLoadAttribute>(out saveLoadClassList);

            string[] fullNames = new string[saveLoadClassList.Length];
            for (int i = 0; i < saveLoadClassList.Length; i++)
            {
                SaveLoadClass saveLoadClass = saveLoadClassList[i];
                string fullName = saveLoadClass.name;
                fullNames[i] = fullName;
            }

            UseProperty("_autoRefresh", "자동 새로고침");
            UseProperty("_isLineShow", "구분선 활성화");

            DrawLine();

            editor.saveLoadClassName = UsePropertyAndDrawStringArray("_saveLoadClassName", "값을 변경 할 클래스", editor.saveLoadClassName, fullNames);

            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }
    }
}