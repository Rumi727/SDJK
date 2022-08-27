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
            ObjectField<SceneAsset>("재생 할 스플래시 씬", ".unity", ref path, ref name, out bool isChanged);
            SplashScreen.Data.splashScreenPath = path;
            SplashScreen.Data.splashScreenName = name;

            if (isChanged)
                SCKRMSetting.SceneListChanged(false);

            EditorGUILayout.Space();

            path = SplashScreen.Data.kernelObjectPath;
            name = SplashScreen.Data.kernelObjectName;
            ObjectField<Kernel>("사용 될 커널 프리팹", ".prefab", ref path, ref name, out isChanged);
            SplashScreen.Data.kernelObjectPath = path;
            SplashScreen.Data.kernelObjectName = name;

            if (isChanged)
                SCKRMSetting.SceneListChanged(false);

            EditorGUILayout.Space();

            VirtualMachineDetector.Data.vmBan = EditorGUILayout.Toggle("가상머신 밴", VirtualMachineDetector.Data.vmBan);


            static void ObjectField<T>(string label, string extension, ref string path, ref string name, out bool isChanged) where T : UnityEngine.Object
            {
                T oldAssets = AssetDatabase.LoadAssetAtPath<T>(PathTool.Combine(path, name) + extension);
                T assets = (T)EditorGUILayout.ObjectField(label, oldAssets, typeof(T), false);

                {
                    string allAssetPath = AssetDatabase.GetAssetPath(assets);
                    if (allAssetPath != "")
                    {
                        string assetPath = allAssetPath.Substring(0, allAssetPath.LastIndexOf("/"));
                        string assetName = allAssetPath.Remove(0, allAssetPath.LastIndexOf("/") + 1);
                        assetName = assetName.Substring(0, assetName.Length - extension.Length);

                        path = assetPath;
                        name = assetName;
                    }
                }

                EditorGUILayout.LabelField($"경로: {PathTool.Combine(path, name) + extension}");
                isChanged = oldAssets != assets;
            }

            //플레이 모드가 아니면 변경한 리스트의 데이터를 잃어버리지 않게 파일로 저장
            if (GUI.changed && !Kernel.isPlaying)
            {
                SaveLoadManager.Save(splashProjectSetting, Kernel.projectSettingPath);
                SaveLoadManager.Save(virtualMachineDetector, Kernel.projectSettingPath);
            }
        }
    }
}
