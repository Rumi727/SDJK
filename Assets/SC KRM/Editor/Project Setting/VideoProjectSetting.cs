using UnityEngine;
using UnityEditor;
using SCKRM.SaveLoad;
using SCKRM.ProjectSetting;
using UnityEngine.UIElements;

namespace SCKRM.Editor
{
    public class VideoProjectSetting : SettingsProvider
    {
        public VideoProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            if (instance == null)
                instance = new VideoProjectSetting("SC KRM/비디오", SettingsScope.Project);

            return instance;
        }



        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            if (!Kernel.isPlaying)
            {
                if (videoProjectSetting == null)
                    SaveLoadManager.Initialize<VideoManager.Data, ProjectSettingSaveLoadAttribute>(out videoProjectSetting);

                SaveLoadManager.Load(videoProjectSetting, Kernel.projectSettingPath);
            }
        }

        public override void OnGUI(string searchContext) => DrawGUI();

        public static SaveLoadClass videoProjectSetting = null;
        public static void DrawGUI()
        {
            EditorGUILayout.Space();

            {
                VideoManager.Data.standardFPS = EditorGUILayout.FloatField("기준 프레임", VideoManager.Data.standardFPS);
                VideoManager.Data.notFocusFpsLimit = EditorGUILayout.IntField("포커스가 아닐때 FPS 제한", VideoManager.Data.notFocusFpsLimit);

                VideoManager.Data.standardFPS = VideoManager.Data.standardFPS;
                VideoManager.Data.notFocusFpsLimit = VideoManager.Data.notFocusFpsLimit;
            }

            //플레이 모드가 아니면 변경한 리스트의 데이터를 잃어버리지 않게 파일로 저장
            if (GUI.changed && !Kernel.isPlaying)
                SaveLoadManager.Save(videoProjectSetting, Kernel.projectSettingPath);
        }
    }
}
