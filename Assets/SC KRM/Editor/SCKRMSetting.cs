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

namespace SCKRM.Editor
{
    [InitializeOnLoad]
    public static class SCKRMSetting
    {
        static SCKRMSetting()
        {
            PlayerSettings.allowFullscreenSwitch = false;
            AudioListener.volume = 0.5f;

            EditorBuildSettings.sceneListChanged += () => { SceneListChanged(true); };
            EditorApplication.hierarchyChanged += () => { HierarchyChanged(true); };

            EditorApplication.update += Update;

            File.WriteAllText(PathTool.Combine(Directory.GetCurrentDirectory(), "SC-KRM-Version"), Kernel.sckrmVersion.ToString());
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
            CustomAllRenderer[] customAllRenderers;

            if (prefabStage == null)
                customAllRenderers = UnityEngine.Object.FindObjectsOfType<CustomAllRenderer>();
            else
                customAllRenderers = prefabStage.FindComponentsOfType<CustomAllRenderer>();

            for (int i = 0; i < customAllRenderers.Length; i++)
            {
                CustomAllRenderer customAllRenderer = customAllRenderers[i];
                customAllRenderer.Refresh();
            }
        }



        static bool sceneListChangedEnable = true;
        static SaveLoadClass splashProjectSetting = null;
        public static void SceneListChanged(bool autoLoad)
        {
            if (Kernel.isPlaying)
                return;

            string activeScenePath = SceneManager.GetActiveScene().path;
            try
            {
                if (sceneListChangedEnable)
                {
                    if (autoLoad)
                    {
                        if (splashProjectSetting == null)
                            SaveLoadManager.Initialize<ProjectSettingSaveLoadAttribute>(typeof(SplashScreen.Data), out splashProjectSetting);

                        SaveLoadManager.Load(splashProjectSetting, Kernel.projectSettingPath);
                    }

                    sceneListChangedEnable = false;

                    EditorSceneManager.OpenScene($"{PathTool.Combine(SplashScreen.Data.splashScreenPath, SplashScreen.Data.splashScreenName)}.unity");
                    HierarchyChanged(false);
                    EditorSceneManager.SaveOpenScenes();

                    string splashScenePath = SceneManager.GetActiveScene().path;

                    bool exists = false;
                    List<EditorBuildSettingsScene> buildScenes = EditorBuildSettings.scenes.ToList();

                    if (buildScenes.Count > 0)
                    {
                        if (!buildScenes[0].enabled)
                            buildScenes.RemoveAt(0);

                        for (int i = 0; i < buildScenes.Count; i++)
                        {
                            EditorBuildSettingsScene scene = buildScenes[i];
                            if (splashScenePath == scene.path)
                            {
                                if (i != 0)
                                    buildScenes.Move(i, 0);

                                exists = true;
                                break;
                            }
                        }
                    }

                    if (!exists)
                        buildScenes.Insert(0, new EditorBuildSettingsScene() { path = splashScenePath, enabled = true });

                    EditorBuildSettings.scenes = buildScenes.ToArray();
                    EditorSceneManager.OpenScene(activeScenePath);

                    sceneListChangedEnable = true;
                }
            }
            catch (ArgumentException e)
            {
                Debug.LogException(e);
                Debug.LogWarning($"{SplashScreen.Data.splashScreenName} 씬이 없는것같습니다 씬을 추가해주세요");
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

            try
            {
                if (hierarchyChangedEnable)
                {
                    bool sceneDirty = false;
                    if (autoLoad)
                    {
                        if (splashProjectSetting == null)
                            SaveLoadManager.Initialize<ProjectSettingSaveLoadAttribute>(typeof(SplashScreen.Data), out splashProjectSetting);

                        SaveLoadManager.Load(splashProjectSetting, Kernel.projectSettingPath);
                    }

                    Scene activeScene = SceneManager.GetActiveScene();
                    PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                    hierarchyChangedEnable = false;

                    #region Kernel
                    if (activeScene.path == $"{PathTool.Combine(SplashScreen.Data.splashScreenPath, SplashScreen.Data.splashScreenName)}.unity")
                    {
                        Kernel kernel = UnityEngine.Object.FindObjectOfType<Kernel>(true);
                        string kernelPrefabPath = PathTool.Combine(SplashScreen.Data.kernelObjectPath, SplashScreen.Data.kernelObjectName) + ".prefab";
                        Kernel kernelPrefab = AssetDatabase.LoadAssetAtPath<Kernel>(kernelPrefabPath);
                        if (kernelPrefab == null)
                            throw new NullFolderObjectException(SplashScreen.Data.kernelObjectPath, SplashScreen.Data.kernelObjectName);

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
                        Kernel kernel = UnityEngine.Object.FindObjectOfType<Kernel>(true);
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
                        cameras = UnityEngine.Object.FindObjectsOfType<UnityEngine.Camera>(true);

                    for (int i = 0; i < cameras.Length; i++)
                    {
                        UnityEngine.Camera camera = cameras[i];
                        CameraSetting cameraSetting = camera.GetComponent<CameraSetting>();
                        if (camera.GetComponent<CameraSetting>() == null)
                            AddComponentCompatibleWithPrefab<CameraSetting>(camera.gameObject, ref sceneDirty);
                        else if (!cameraSetting.enabled)
                            DestroyComponentCompatibleWithPrefab(cameraSetting, ref sceneDirty);
                    }
                    #endregion

                    #region Canvas Setting
                    Canvas[] canvases;
                    if (prefabStage != null)
                        canvases = prefabStage.FindComponentsOfType<Canvas>();
                    else
                        canvases = UnityEngine.Object.FindObjectsOfType<Canvas>(true);

                    for (int i = 0; i < canvases.Length; i++)
                    {
                        Canvas canvas = canvases[i];
                        CanvasSetting canvasSetting = canvas.GetComponent<CanvasSetting>();

                        if (canvas.GetComponent<UIManager>() == null)
                        {
                            if (canvasSetting == null)
                                AddComponentCompatibleWithPrefab<CanvasSetting>(canvas.gameObject, ref sceneDirty);
                            else if (!canvasSetting.enabled)
                                DestroyComponentCompatibleWithPrefab(canvasSetting, ref sceneDirty);
                        }

                        if (canvasSetting != null && !canvasSetting.customSetting && !canvasSetting.customGuiSize)
                        {
                            CanvasScaler[] canvasScalers = canvas.GetComponents<CanvasScaler>();
                            for (int j = 0; j < canvasScalers.Length; j++)
                            {
                                CanvasScaler canvasScaler = canvasScalers[j];
                                if (canvasScaler != null)
                                    DestroyComponentCompatibleWithPrefab(canvasScaler, ref sceneDirty);
                            }
                        }
                    }
                    #endregion

                    #region Rect Transform Tool
                    Transform[] transforms;
                    if (prefabStage != null)
                        transforms = prefabStage.FindComponentsOfType<Transform>();
                    else
                        transforms = UnityEngine.Object.FindObjectsOfType<Transform>(true);

                    for (int i = 0; i < transforms.Length; i++)
                    {
                        Transform transform = transforms[i];
                        RectTransform rectTransform = transform.gameObject.GetComponent<RectTransform>();
                        RectTransformTool rectTransformTool = transform.GetComponent<RectTransformTool>();

                        if (rectTransform != null)
                        {
                            if (rectTransformTool == null)
                                AddComponentCompatibleWithPrefab<RectTransformTool>(rectTransform.gameObject, ref sceneDirty, true);
                            else if (!rectTransformTool.enabled)
                                DestroyComponentCompatibleWithPrefab(rectTransformTool, ref sceneDirty);
                        }
                        else if (rectTransformTool != null)
                            DestroyComponentCompatibleWithPrefab(rectTransformTool, ref sceneDirty);
                    }
                    #endregion

                    if (sceneDirty)
                        EditorSceneManager.MarkSceneDirty(activeScene);
                }
            }
            finally
            {
                hierarchyChangedEnable = true;
            }
        }

        public static void AddComponentCompatibleWithPrefab<T>(GameObject gameObject, ref bool isModified, bool backToTop = false) where T : Component
        {
            //오브젝트의 프리팹 타입을 가져옵니다
            PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(gameObject);
            if (prefabAssetType == PrefabAssetType.NotAPrefab) //프리팹이 아니라면 평소대로 컴포넌트를 추가합니다
            {
                T addedRectTransformTool = gameObject.AddComponent<T>();

                if (backToTop)
                    ComponentBackToTop(addedRectTransformTool);

                isModified = true;
            }
            else if (prefabAssetType != PrefabAssetType.MissingAsset) //미싱되지 않은 프리팹이라면
            {
                //프리팹에서 오버라이딩으로 삭제된 컴포넌트를 가져옵니다
                List<RemovedComponent> removedComponents = PrefabUtility.GetRemovedComponents(gameObject);
                for (int j = 0; j < removedComponents.Count; j++)
                {
                    RemovedComponent removedComponent = removedComponents[j];
                    if (removedComponent.assetComponent.GetType() == typeof(T))
                    {
                        /*
                         * 만약 현제 추가할 컴포넌트가 오버라이딩으로 삭제된 컴포넌트랑 타입이 똑같다면
                         * 오버라이딩을 되돌린후, 후에 수행할 작업을 취소합니다
                         */

                        removedComponent.Revert();
                        isModified = true;

                        return;
                    }
                }

                /*
                 * 프리팹의 오리지널 (에셋에 있는) 오브젝트를 가져오고
                 * 그 오리지널 프리팹에 컴포넌트를 추가합니다
                 */
                GameObject original = PrefabUtility.GetCorrespondingObjectFromOriginalSource(gameObject);
                T addedRectTransformTool = original.AddComponent<T>();

                if (backToTop)
                    ComponentBackToTop(addedRectTransformTool);

                //수정 사항을 저장합니다
                EditorUtility.SetDirty(original);
                EditorUtility.SetDirty(addedRectTransformTool);

                isModified = true;
            }
        }

        public static void DestroyComponentCompatibleWithPrefab(Component component, ref bool isModified)
        {
            //오브젝트의 프리팹 타입을 가져옵니다
            PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(component);

            //오브젝트가 프리팹이 아니라면 평소대로 컴포넌트를 삭제합니다
            if (prefabAssetType == PrefabAssetType.NotAPrefab)
            {
                GameObject gameObject = component.gameObject;
                UnityEngine.Object.DestroyImmediate(component);

                //수정 사항을 저장합니다
                EditorUtility.SetDirty(gameObject);
                isModified = true;
            }
            else if (prefabAssetType != PrefabAssetType.MissingAsset) //오브젝트가 미싱되지 않은 프리팹이라면 프리팹의 오리지널을 가져온후, 그 프리팹에서 컴포넌트를 삭제합니다
            {
                Component original = PrefabUtility.GetCorrespondingObjectFromOriginalSource(component);
                GameObject gameObject = original.gameObject;
                UnityEngine.Object.DestroyImmediate(original, true);

                //수정 사항을 저장합니다
                EditorUtility.SetDirty(gameObject);
                isModified = true;
            }
        }

        public static void ComponentBackToTop(Component component)
        {
            int length = component.GetComponents<Component>().Length;
            for (int j = 0; j < length - 2; j++)
                ComponentUtility.MoveComponentUp(component);
        }
    }
}