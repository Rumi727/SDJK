using UnityEngine;
using UnityEditor;
using SCKRM.SaveLoad;
using SCKRM.ProjectSetting;
using SCKRM.Splash;
using SCKRM.VM;
using UnityEngine.UIElements;

namespace SCKRM.Editor
{
    public class SplashProjectSetting : SettingsProvider
    {
        public SplashProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            if (instance == null)
                instance = new SplashProjectSetting("SC KRM/스플래시", SettingsScope.Project);

            return instance;
        }



        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            if (!Kernel.isPlaying)
            {
                if (splashProjectSetting == null)
                    SaveLoadManager.Initialize<ProjectSettingSaveLoadAttribute>(typeof(SplashScreen.Data), out splashProjectSetting);

                if (virtualMachineDetector == null)
                    SaveLoadManager.Initialize<ProjectSettingSaveLoadAttribute>(typeof(VirtualMachineDetector.Data), out virtualMachineDetector);

                SaveLoadManager.Load(splashProjectSetting, Kernel.projectSettingPath);
                SaveLoadManager.Load(virtualMachineDetector, Kernel.projectSettingPath);
            }
        }

        public override void OnGUI(string searchContext) => DrawGUI();

        public static SaveLoadClass splashProjectSetting = null;
        public static SaveLoadClass virtualMachineDetector = null;
        public static void DrawGUI()
        {
            EditorGUILayout.Space();

            string path = SplashScreen.Data.splashScreenPath;
            string name = SplashScreen.Data.splashScreenName;
            CustomInspectorEditor.FileObjectField<SceneAsset>("재생 할 스플래시 씬", ".unity", ref path, ref name, out bool isChanged);
            SplashScreen.Data.splashScreenPath = path;
            SplashScreen.Data.splashScreenName = name;

            path = SplashScreen.Data.sceneLoadingScenePath;
            name = SplashScreen.Data.sceneLoadingSceneName;
            CustomInspectorEditor.FileObjectField<SceneAsset>("씬을 불러올때 사용할 씬", ".unity", ref path, ref name, out bool isChanged2);
            SplashScreen.Data.sceneLoadingScenePath = path;
            SplashScreen.Data.sceneLoadingSceneName = name;

            EditorGUILayout.Space();

            path = SplashScreen.Data.kernelObjectPath;
            name = SplashScreen.Data.kernelObjectName;
            CustomInspectorEditor.FileObjectField<Kernel>("사용 될 커널 프리팹", ".prefab", ref path, ref name, out bool isChanged3);
            SplashScreen.Data.kernelObjectPath = path;
            SplashScreen.Data.kernelObjectName = name;

            if (isChanged || isChanged2 || isChanged3)
                SCKRMSetting.SceneListChanged(false);

            EditorGUILayout.Space();

            VirtualMachineDetector.Data.vmBan = EditorGUILayout.Toggle("가상머신 밴", VirtualMachineDetector.Data.vmBan);

            //플레이 모드가 아니면 변경한 리스트의 데이터를 잃어버리지 않게 파일로 저장
            if (GUI.changed && !Kernel.isPlaying)
            {
                SaveLoadManager.Save(splashProjectSetting, Kernel.projectSettingPath);
                SaveLoadManager.Save(virtualMachineDetector, Kernel.projectSettingPath);
            }
        }
    }
}
