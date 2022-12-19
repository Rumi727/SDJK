using UnityEngine;
using UnityEditor;
using SCKRM.SaveLoad;
using SCKRM.ProjectSetting;
using UnityEngine.UIElements;
using SCKRM.Resource;

namespace SCKRM.Editor
{
    public class DefaultNameSpaceProjectSetting : SettingsProvider
    {
        public DefaultNameSpaceProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            if (instance == null)
                instance = new DefaultNameSpaceProjectSetting("SC KRM/기본 네임스페이스", SettingsScope.Project);

            return instance;
        }



        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            if (!Kernel.isPlaying)
            {
                if (resourceProjectSetting == null)
                    SaveLoadManager.Initialize<ProjectSettingSaveLoadAttribute>(typeof(ResourceManager.Data), out resourceProjectSetting);

                SaveLoadManager.Load(resourceProjectSetting, Kernel.projectSettingPath);
            }
        }

        bool deleteSafety = true;
        public override void OnGUI(string searchContext) => DrawGUI(ref deleteSafety);

        public static SaveLoadClass resourceProjectSetting;
        public static void DrawGUI(ref bool deleteSafety)
        {
            //GUI
            {
                EditorGUILayout.Space();

                CustomInspectorEditor.DeleteSafety(ref deleteSafety);

                EditorGUILayout.Space();
            }

            CustomInspectorEditor.DrawList(ResourceManager.Data.nameSpaces, "네임스페이스", 0, 0, deleteSafety);

            if (GUI.changed && !Kernel.isPlaying)
                SaveLoadManager.Save(resourceProjectSetting, Kernel.projectSettingPath);
        }
    }
}
