using UnityEngine;
using UnityEditor;
using SCKRM.SaveLoad;
using SCKRM.ProjectSetting;
using UnityEngine.UIElements;
using SCKRM.Loading;

namespace SCKRM.Editor
{
    public class LoadingAniProjectSetting : SettingsProvider
    {
        public LoadingAniProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            if (instance == null)
                instance = new LoadingAniProjectSetting("SC KRM/로딩 애니메이션", SettingsScope.Project);

            return instance;
        }



        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            if (!Kernel.isPlaying)
            {
                if (loadingAniProjectSetting == null)
                    SaveLoadManager.Initialize<ProjectSettingSaveLoadAttribute>(typeof(LoadingAniManager.Data), out loadingAniProjectSetting);

                SaveLoadManager.Load(loadingAniProjectSetting, Kernel.projectSettingPath);
            }
        }

        public override void OnGUI(string searchContext) => DrawGUI();

        public static SaveLoadClass loadingAniProjectSetting = null;
        public static void DrawGUI()
        {
            EditorGUILayout.Space();

            {
                LoadingAniManager.Data.longLoadingTime = EditorGUILayout.FloatField("긴 로딩 시간", LoadingAniManager.Data.longLoadingTime).Clamp(0);
                LoadingAniManager.Data.aniLerp = EditorGUILayout.Slider("애니메이션 속도", LoadingAniManager.Data.aniLerp, 0, 1).Clamp01();
            }

            //플레이 모드가 아니면 변경한 리스트의 데이터를 잃어버리지 않게 파일로 저장
            if (GUI.changed && !Kernel.isPlaying)
                SaveLoadManager.Save(loadingAniProjectSetting, Kernel.projectSettingPath);
        }
    }
}
