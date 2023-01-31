using UnityEngine;
using UnityEditor;
using SCKRM.SaveLoad;
using SCKRM.ProjectSetting;
using UnityEngine.UIElements;
using SCKRM.Resource;
using SCKRM.DragAndDrop;

namespace SCKRM.Editor
{
    public class DragAndDropProjectSetting : SettingsProvider
    {
        public DragAndDropProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            if (instance == null)
                instance = new DragAndDropProjectSetting("SC KRM/드래그 앤 드랍", SettingsScope.Project);

            return instance;
        }



        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            if (!Kernel.isPlaying)
            {
                if (dadProjectSetting == null)
                    SaveLoadManager.Initialize<DragAndDropManager.Data, ProjectSettingSaveLoadAttribute>(out dadProjectSetting);

                SaveLoadManager.Load(dadProjectSetting, Kernel.projectSettingPath);
            }
        }

        public override void OnGUI(string searchContext) => DrawGUI();

        public static SaveLoadClass dadProjectSetting;
        public static void DrawGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("에디터의 플레이 모드에서 드래그 앤 드랍 활성화", GUILayout.ExpandWidth(false));
            DragAndDropManager.Data.editorDADEnable = EditorGUILayout.Toggle(DragAndDropManager.Data.editorDADEnable);

            EditorGUILayout.EndHorizontal();

            if (GUI.changed && !Kernel.isPlaying)
                SaveLoadManager.Save(dadProjectSetting, Kernel.projectSettingPath);
        }
    }
}
