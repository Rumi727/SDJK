using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using SCKRM.Camera;
using SCKRM.UI;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using SCKRM.Splash;
using SCKRM.SaveLoad;
using SCKRM.ProjectSetting;
using SCKRM.Renderer;
using System.IO;
using UnityEditorInternal;
using SCKRM.SaveLoad.UI;

namespace SCKRM.Editor
{
    [InitializeOnLoad]
    public static class SCKRMSetting
    {
        public static string splashScenePath => SplashScreen.Data.splashScenePath;
        public static string sceneLoadingScenePath => SplashScreen.Data.sceneLoadingScenePath;
        public static string kernelPrefabPath => SplashScreen.Data.kernelPrefabPath;

        static SCKRMSetting()
        {
            PlayerSettings.allowFullscreenSwitch = false;
            AudioListener.volume = 0.5f;

            SetPlayModeStartScene(true);

            EditorBuildSettings.sceneListChanged += () => SceneListChanged(true);
            EditorApplication.hierarchyChanged += () => HierarchyChanged(true);

            EditorApplication.update += Update;

            File.WriteAllText(PathUtility.Combine(Directory.GetCurrentDirectory(), "SC-KRM-Version"), Kernel.sckrmVersion.ToString());
        }

        static void Update()
        {
            if (EditorBuildSettings.scenes.Length <= 0)
            {
                SceneListChanged(true);
                HierarchyChanged(false);
            }

            EditorApplication.update -= Update;
        }



        [MenuItem("SC KRM/Show control panel")]
        public static void ShowControlPanelWindow() => EditorWindow.GetWindow<SCKRMWindowEditor>(false, "SC KRM");

        [MenuItem("SC KRM/Github Wiki Setting")]
        public static void ShowGithubWikiWindow() => EditorWindow.GetWindow<SCKRMWikiEditor>(false, "SC KRM Github Wiki");

        [MenuItem("SC KRM/All Rerender")]
        public static void AllRerender()
        {
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            CustomRendererBase[] customAllRenderers;

            if (prefabStage == null)
#if UNITY_2023_1_OR_NEWER
                customAllRenderers = UnityEngine.Object.FindObjectsByType<CustomRendererBase>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
                customAllRenderers = UnityEngine.Object.FindObjectsOfType<CustomRendererBase>(true);
#endif
            else
                customAllRenderers = prefabStage.FindComponentsOfType<CustomRendererBase>();

            for (int i = 0; i < customAllRenderers.Length; i++)
            {
                CustomRendererBase customAllRenderer = customAllRenderers[i];
                customAllRenderer.Refresh();
            }
        }

        [MenuItem("SC KRM/Selected Object Navigation Disable In Children")]
        public static void SelectedObjectNavigationDisableInChildren()
        {
            if (!EditorUtility.DisplayDialog("Navigaion Disable", "Do you want to disable navigation to all child objects of the selected object?", "Yes", "No"))
                return;

            GameObject gameObject = Selection.activeGameObject;
            if (gameObject == null)
                return;

            Selectable[] selectables = gameObject.GetComponentsInChildren<Selectable>(true);
            for (int i = 0; i < selectables.Length; i++)
            {
                Selectable selectable = selectables[i];
                selectable.navigation = new Navigation() { mode = Navigation.Mode.None };

                EditorUtility.SetDirty(selectable);
            }
        }

        [MenuItem("SC KRM/Selected Object No Object Pooling Enable In Children")]
        public static void SelectedObjectNoObjectPoolingEnableInChildren()
        {
            if (!EditorUtility.DisplayDialog("No Object Pooling Enable", "Do you want to enable no object pooling to all child objects of the selected object?", "Yes", "No"))
                return;

            GameObject gameObject = Selection.activeGameObject;
            if (gameObject == null)
                return;

            SaveLoadUIBase[] saveLoadUIBases = gameObject.GetComponentsInChildren<SaveLoadUIBase>(true);
            for (int i = 0; i < saveLoadUIBases.Length; i++)
            {
                SaveLoadUIBase saveLoadUIBase = saveLoadUIBases[i];
                saveLoadUIBase.autoRefresh = true;

                EditorUtility.SetDirty(saveLoadUIBase);
            }
        }



        static bool sceneListChangedEnable = true;
        static SaveLoadClass splashProjectSetting = null;
        public static void SceneListChanged(bool autoLoad)
        {
            if (Kernel.isPlaying)
                return;
            else if (!sceneListChangedEnable)
                return;

            string activeScenePath = SceneManager.GetActiveScene().path;
            try
            {
                if (autoLoad)
                {
                    if (splashProjectSetting == null)
                        SaveLoadManager.Initialize<SplashScreen.Data, ProjectSettingSaveLoadAttribute>(out splashProjectSetting);

                    SaveLoadManager.Load(splashProjectSetting, Kernel.projectSettingPath);
                }

                sceneListChangedEnable = false;

                string splashScenePath = SCKRMSetting.splashScenePath;
                string sceneLoadingScenePath = SCKRMSetting.sceneLoadingScenePath;

                EditorSceneManager.OpenScene(splashScenePath);
                HierarchyChanged(false);
                EditorSceneManager.SaveOpenScenes();

                List<EditorBuildSettingsScene> buildScenes = EditorBuildSettings.scenes.ToList();
                for (int i = 0; i < buildScenes.Count; i++)
                {
                    EditorBuildSettingsScene scene = buildScenes[i];
                    if (splashScenePath == scene.path || sceneLoadingScenePath == scene.path)
                    {
                        buildScenes.RemoveAt(i);
                        i--;
                    }
                }

                buildScenes.Insert(0, new EditorBuildSettingsScene() { path = splashScenePath, enabled = true });
                buildScenes.Insert(1, new EditorBuildSettingsScene() { path = sceneLoadingScenePath, enabled = true });

                EditorBuildSettings.scenes = buildScenes.ToArray();
            }
            catch (ArgumentException e)
            {
                Debug.LogException(e);
                Debug.LogWarning($"{Path.GetFileNameWithoutExtension(splashScenePath)} 씬이 없는것같습니다 씬을 추가해주세요");
            }
            finally
            {
                EditorSceneManager.OpenScene(activeScenePath);
                sceneListChangedEnable = true;
            }
        }

        static bool hierarchyChangedEnable = true;
        public static void HierarchyChanged(bool autoLoad)
        {
            if (Kernel.isPlaying)
                return;
            else if (!hierarchyChangedEnable)
                return;

            try
            {
                bool sceneDirty = false;
                if (autoLoad)
                {
                    if (splashProjectSetting == null)
                        SaveLoadManager.Initialize<SplashScreen.Data, ProjectSettingSaveLoadAttribute>(out splashProjectSetting);

                    SaveLoadManager.Load(splashProjectSetting, Kernel.projectSettingPath);
                }

                hierarchyChangedEnable = false;

                UnityEngine.SceneManagement.Scene activeScene = SceneManager.GetActiveScene();
                PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

                string splashScenePath = SCKRMSetting.splashScenePath;
                string kernelPrefabPath = SCKRMSetting.kernelPrefabPath;

                #region Kernel
                if (activeScene.path == splashScenePath)
                {
#if UNITY_2023_1_OR_NEWER
                    Kernel kernel = UnityEngine.Object.FindFirstObjectByType<Kernel>(FindObjectsInactive.Include);
#else
                    Kernel kernel = UnityEngine.Object.FindObjectOfType<Kernel>(true);
#endif
                    Kernel kernelPrefab = AssetDatabase.LoadAssetAtPath<Kernel>(kernelPrefabPath);
                    if (kernelPrefab == null)
                        throw new NullFolderObjectException(kernelPrefabPath);

                    if (kernel == null)
                    {
                        PrefabUtility.InstantiatePrefab(kernelPrefab);
                        sceneDirty = true;
                    }
                    else if (PrefabUtility.GetPrefabAssetType(kernel) == PrefabAssetType.NotAPrefab || PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(kernel) != kernelPrefabPath)
                    {
                        UnityEngine.Object.DestroyImmediate(kernel.gameObject);
                        PrefabUtility.InstantiatePrefab(kernelPrefab);

                        sceneDirty = true;
                    }
                    else if (!kernel.enabled || !kernel.gameObject.activeSelf)
                    {
                        UnityEngine.Object.DestroyImmediate(kernel.gameObject);
                        sceneDirty = true;
                    }
                }
                else
                {
#if UNITY_2023_1_OR_NEWER
                    Kernel kernel = UnityEngine.Object.FindFirstObjectByType<Kernel>(FindObjectsInactive.Include);
#else
                    Kernel kernel = UnityEngine.Object.FindObjectOfType<Kernel>(true);
#endif
                    if (kernel != null)
                    {
                        UnityEngine.Object.DestroyImmediate(kernel.gameObject);
                        sceneDirty = true;
                    }
                }
                #endregion

                #region Camera Setting
                UnityEngine.Camera[] cameras;
                if (prefabStage != null)
                    cameras = prefabStage.FindComponentsOfType<UnityEngine.Camera>();
                else
#if UNITY_2023_1_OR_NEWER
                    cameras = UnityEngine.Object.FindObjectsByType<UnityEngine.Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
                    cameras = UnityEngine.Object.FindObjectsOfType<UnityEngine.Camera>(true);
#endif

                for (int i = 0; i < cameras.Length; i++)
                {
                    UnityEngine.Camera camera = cameras[i];
                    CameraSetting cameraSetting = camera.GetComponent<CameraSetting>();
                    if (camera.GetComponent<CameraSetting>() == null)
                        EditorTool.AddComponentCompatibleWithPrefab<CameraSetting>(camera.gameObject, ref sceneDirty);
                    else if (!cameraSetting.enabled)
                        EditorTool.DestroyComponentCompatibleWithPrefab(cameraSetting, ref sceneDirty);
                }
                #endregion

                #region Canvas Setting
                Canvas[] canvases;
                if (prefabStage != null)
                    canvases = prefabStage.FindComponentsOfType<Canvas>();
                else
#if UNITY_2023_1_OR_NEWER
                    canvases = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
                    canvases  = UnityEngine.Object.FindObjectsOfType<Canvas>(true);
#endif

                for (int i = 0; i < canvases.Length; i++)
                {
                    Canvas canvas = canvases[i];
                    CanvasSetting canvasSetting = canvas.GetComponent<CanvasSetting>();

                    if (canvas.GetComponent<UIManager>() == null)
                    {
                        if (canvasSetting == null)
                            EditorTool.AddComponentCompatibleWithPrefab<CanvasSetting>(canvas.gameObject, ref sceneDirty);
                        else if (!canvasSetting.enabled)
                            EditorTool.DestroyComponentCompatibleWithPrefab(canvasSetting, ref sceneDirty);
                    }

                    if (canvasSetting != null && !canvasSetting.customSetting && !canvasSetting.customGuiSize)
                    {
                        CanvasScaler[] canvasScalers = canvas.GetComponents<CanvasScaler>();
                        for (int j = 0; j < canvasScalers.Length; j++)
                        {
                            CanvasScaler canvasScaler = canvasScalers[j];
                            if (canvasScaler != null)
                                EditorTool.DestroyComponentCompatibleWithPrefab(canvasScaler, ref sceneDirty);
                        }
                    }
                }
                #endregion

                #region Rect Transform Tool
                Transform[] transforms;
                if (prefabStage != null)
                    transforms = prefabStage.FindComponentsOfType<Transform>();
                else
#if UNITY_2023_1_OR_NEWER
                    transforms = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
                    transforms = UnityEngine.Object.FindObjectsOfType<Transform>(true);
#endif

                for (int i = 0; i < transforms.Length; i++)
                {
                    Transform transform = transforms[i];
                    RectTransform rectTransform = transform.gameObject.GetComponent<RectTransform>();
                    RectTransformTool rectTransformTool = transform.GetComponent<RectTransformTool>();

                    if (rectTransform != null)
                    {
                        if (rectTransformTool == null)
                            EditorTool.AddComponentCompatibleWithPrefab<RectTransformTool>(rectTransform.gameObject, ref sceneDirty, true);
                        else if (!rectTransformTool.enabled)
                            EditorTool.DestroyComponentCompatibleWithPrefab(rectTransformTool, ref sceneDirty);
                    }
                    else if (rectTransformTool != null)
                        EditorTool.DestroyComponentCompatibleWithPrefab(rectTransformTool, ref sceneDirty);
                }
                #endregion

                if (sceneDirty)
                    EditorSceneManager.MarkSceneDirty(activeScene);
            }
            finally
            {
                hierarchyChangedEnable = true;
            }
        }

        public static void SetPlayModeStartScene(bool autoLoad)
        {
            if (autoLoad)
            {
                if (splashProjectSetting == null)
                    SaveLoadManager.Initialize<SplashScreen.Data, ProjectSettingSaveLoadAttribute>(out splashProjectSetting);

                SaveLoadManager.Load(splashProjectSetting, Kernel.projectSettingPath);
            }

            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(splashScenePath);
        }
    }
}