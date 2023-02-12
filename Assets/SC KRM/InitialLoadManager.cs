using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SCKRM.Input;
using SCKRM.ProjectSetting;
using SCKRM.Renderer;
using SCKRM.Resource;
using SCKRM.SaveLoad;
using SCKRM.Sound;
using SCKRM.Splash;
using SCKRM.Threads;
using SCKRM.UI;
using SCKRM.UI.StatusBar;
using SCKRM.VM;
using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if (UNITY_ANDROID || ENABLE_ANDROID_SUPPORT) && !UNITY_EDITOR
using System.IO;
using UnityEngine.Networking;
using SCKRM.Compress;
#endif

[assembly: InternalsVisibleTo("SC-KRM-Editor")]

namespace SCKRM
{
    [WikiDescription("초기 로딩을 위한 클래스 입니다")]
    public static class InitialLoadManager
    {
        public static bool isInitialLoadStart { get; private set; } = false;
        public static bool isSettingLoadEnd { get; private set; } = false;
        public static bool isInitialLoadEnd { get; private set; } = false;
        public static bool isSceneMoveEnd { get; private set; } = false;

        public static event Action initialLoadEnd;
        public static event Action initialLoadEndSceneMove;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static async UniTaskVoid InitialLoad()
        {
            isInitialLoadStart = false;
            isSettingLoadEnd = false;
            isInitialLoadEnd = false;
            isSceneMoveEnd = false;

            try
            {
                //초기로딩이 시작됬습니다
                isInitialLoadStart = true;

                //이 함수는 어떠한 경우에도 메인스레드가 아닌 스레드에서 실행되면 안됩니다
                if (!ThreadManager.isMainThread)
                    throw new NotMainThreadMethodException();
                //이 함수는 어떠한 경우에도 앱이 플레이중이 아닐때 실행되면 안됩니다
                if (!Kernel.isPlaying)
                    throw new NotPlayModeMethodException();
#if UNITY_EDITOR
                if (UnityEditor.EditorSettings.enterPlayModeOptionsEnabled && UnityEditor.EditorSettings.enterPlayModeOptions.HasFlag(UnityEditor.EnterPlayModeOptions.DisableDomainReload))
                {
                    GameObject[] gameObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(true);
                    int length = gameObjects.Length;
                    for (int i = 0; i < length; i++)
                    {
                        GameObject gameObject = gameObjects[i];
                        if (gameObject != null && UnityEditor.PrefabUtility.GetPrefabInstanceStatus(gameObject) == UnityEditor.PrefabInstanceStatus.NotAPrefab)
                            UnityEngine.Object.DestroyImmediate(gameObject);
                    }

                    UnityEditor.EditorApplication.isPlaying = false;
                    throw new NotSupportedException("SC KRM은 Disable Domain Reload를 지원 하지 않습니다\nSC KRM does not support Disable Domain Reload");
                }
#endif

                //UniTask를 초기화 합니다
                PlayerLoopSystem loop = PlayerLoop.GetCurrentPlayerLoop();
                PlayerLoopHelper.Initialize(ref loop);

                StatusBarManager.allowStatusBarShow = false;

                //스레드를 자동 삭제해주는 함수를 작동시킵니다
                ThreadManager.ThreadAutoRemove().Forget();

                //CPU가 GPU가 렌더링할때까지 기다리는것을 끕니다
                QualitySettings.maxQueuedFrames = 1;

#if UNITY_EDITOR
                //에디터에선 SCKRMSetting 에디터 클래스가 시작 씬을 변경하니 아무런 조건문 없이 시작해도 됩니다
#endif
                //빌드된곳에선 스플래시 씬에서 시작하기 때문에
                //아무런 조건문 없이 바로 시작합니다

                //다른 스레드에서 이 값을 설정하기 전에
                //미리 설정합니다
                //(참고: 이 변수는 프로퍼티고 변수가 비어있다면 Application를 호출합니다)
                {
                    _ = Kernel.dataPath;
                    _ = Kernel.persistentDataPath;
                    _ = Kernel.temporaryCachePath;
                    _ = Kernel.saveDataPath;
                    _ = Kernel.resourcePackPath;

                    _ = Kernel.companyName;
                    _ = Kernel.productName;

                    _ = Kernel.version;
                    _ = Kernel.unityVersion;
                }

#if UNITY_WEBGL && !UNITY_EDITOR
                //CS0162 접근할 수 없는 코드 경고를 비활성화 하기 위해 변수로 우회합니다
                bool warningDisable = true;
                if (warningDisable)
                    throw new NotSupportedException("SC KRM은 WebGL을 지원하지 않습니다\nSC KRM does not support WebGL");
#elif (UNITY_ANDROID || ENABLE_ANDROID_SUPPORT) && !UNITY_EDITOR
                if (!Directory.Exists(PathTool.Combine(Kernel.streamingAssetsPath, "assets")))
                {
                    if (Directory.Exists(Kernel.streamingAssetsPath))
                    {
                        Directory.Delete(Kernel.streamingAssetsPath, true);
                        Directory.CreateDirectory(Kernel.streamingAssetsPath);
                    }

                    string zipPath = PathTool.Combine(Kernel.streamingAssetsPath, Kernel.streamingAssetsFolderName + ".zip");

                    Debug.Log(nameof(zipPath) + ": " + zipPath, nameof(InitialLoadManager));
                    Debug.Log(nameof(Kernel.streamingAssetsPath) + ": " + Kernel.streamingAssetsPath, nameof(InitialLoadManager));

                    using (UnityWebRequest webRequest = UnityWebRequest.Get(PathTool.Combine(Application.streamingAssetsPath, Kernel.streamingAssetsFolderName + ".zip")))
                    {
                        await webRequest.SendWebRequest();

                        if (webRequest.result != UnityWebRequest.Result.Success)
                            Debug.LogError(webRequest.error);

                        await File.WriteAllBytesAsync(zipPath, webRequest.downloadHandler.data);
                    }

                    CompressFileManager.DecompressZipFile(zipPath, Kernel.streamingAssetsPath, "");
                    ThreadMetaData metaData = ThreadManager.Create(x => CompressFileManager.DecompressZipFile(zipPath, Kernel.streamingAssetsPath, "", x));
                    ResourceManager.resourceRefreshDetailedAsyncTask = metaData;

                    if (await UniTask.WaitUntil(() => metaData.progress >= metaData.maxProgress, PlayerLoopTiming.Update, AsyncTaskManager.cancelToken).SuppressCancellationThrow())
                        return;

                    ResourceManager.resourceRefreshDetailedAsyncTask = null;
                    File.Delete(zipPath);
                }
#endif

                Debug.ForceLog("Waiting for settings to load...", nameof(InitialLoadManager));
                {
                    //세이브 데이터의 기본값과 변수들을 다른 스레드에서 로딩합니다
                    if (await UniTask.RunOnThreadPool(Initialize, cancellationToken: AsyncTaskManager.cancelToken).SuppressCancellationThrow())
                        return;

                    //세이브 데이터를 다른 스레드에서 로딩합니다
                    if (await UniTask.RunOnThreadPool(() => SaveLoadManager.LoadAll(ProjectSettingManager.projectSettingSLCList, Kernel.projectSettingPath), cancellationToken: AsyncTaskManager.cancelToken).SuppressCancellationThrow())
                        return;

                    //세이브 데이터를 다른 스레드에서 로딩합니다
                    if (await UniTask.RunOnThreadPool(() => SaveLoadManager.LoadAll(SaveLoadManager.generalSLCList, Kernel.saveDataPath), cancellationToken: AsyncTaskManager.cancelToken).SuppressCancellationThrow())
                        return;

                    static void Initialize()
                    {
                        SaveLoadManager.InitializeAll<GeneralSaveLoadAttribute>(out SaveLoadClass[] saveLoadClass);
#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
                        SaveLoadManager.generalSLCList = saveLoadClass;
#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.

                        SaveLoadManager.InitializeAll<ProjectSettingSaveLoadAttribute>(out saveLoadClass);
#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
                        ProjectSettingManager.projectSettingSLCList = saveLoadClass;
#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.
                    }
                }

                isSettingLoadEnd = true;

                //가상머신 밴이 활성화되어있을때 가상 머신일 경우 프로그램을 강제종료 합니다
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
                if (VirtualMachineDetector.Data.vmBan && (VirtualMachineDetector.HardwareDetection() || VirtualMachineDetector.ProcessDetection() || VirtualMachineDetector.FileDetection()))
                    ApplicationForceQuit("Virtual machines are prohibited");
#endif

                //오디오 레이턴시를 수정합니다
                AudioConfiguration audioConfiguration = AudioSettings.GetConfiguration();
                if (SoundManager.SaveData.fixAudioLatency && audioConfiguration.dspBufferSize != 256)
                {
                    audioConfiguration.dspBufferSize = 256;
                    AudioSettings.Reset(audioConfiguration);
                }
                else if (!SoundManager.SaveData.fixAudioLatency && audioConfiguration.dspBufferSize != 1024)
                {
                    audioConfiguration.dspBufferSize = 1024;
                    AudioSettings.Reset(audioConfiguration);
                }

                //Awake, OnEnable 함수의 작동이 끝날때까지 기다립니다
                if (await UniTask.NextFrame(PlayerLoopTiming.LastTimeUpdate, AsyncTaskManager.cancelToken).SuppressCancellationThrow())
                    return;

                AwakenManager.AllAwakenableMethodAwaken();

                {
                    //리소스를 로딩합니다
                    Debug.ForceLog("Waiting for resource to load...", nameof(InitialLoadManager));
                    await ResourceManager.ResourceRefresh(true);

#if UNITY_EDITOR
                    if (!Kernel.isPlaying)
                        return;
#endif
                }

                {
                    //초기 로딩이 끝났습니다
                    isInitialLoadEnd = true;
                    initialLoadEnd?.Invoke();

                    Debug.ForceLog("Initial loading finished!", nameof(InitialLoadManager));
                }

                StartenManager.AllStartableMethodAwaken();

                //강제종료 된 상태면, 씬을 이동하지 않습니다
                if (!isForceQuit)
                {
#if UNITY_EDITOR
                    int lastActivatedSceneIndex = 0;
                    if (SplashScreen.Data.startSceneIndex >= 0)
                        lastActivatedSceneIndex = SplashScreen.Data.startSceneIndex.Clamp(2);

                    if (lastActivatedSceneIndex != 0)
                        SplashScreen.isAniPlaying = false;
#endif
                    //씬 애니메이션이 끝날때까지 기다립니다
                    Debug.ForceLog("Kernel: Waiting for scene animation...", nameof(InitialLoadManager));
                    if (await UniTask.WaitUntil(() => !SplashScreen.isAniPlaying, cancellationToken: AsyncTaskManager.cancelToken).SuppressCancellationThrow())
                        return;

                    StatusBarManager.allowStatusBarShow = true;

                    //씬이 바뀌었을때 렌더러를 재 렌더링해야하기때문에 이벤트를 걸어줍니다
                    SceneManager.sceneLoaded += (UnityEngine.SceneManagement.Scene scene, LoadSceneMode loadSceneMode) => RendererManager.AllRerender();

                    //GC를 호출합니다
                    GC.Collect();
#if UNITY_EDITOR
                    //씬을 이동합니다
                    if (lastActivatedSceneIndex != 0)
                        SceneManager.LoadScene(lastActivatedSceneIndex);
                    else
                        SceneManager.LoadScene(2);
#else
                    SceneManager.LoadScene(2);
#endif

                    //씬을 이동했으면 이벤트를 호출합니다
                    isSceneMoveEnd = true;
                    initialLoadEndSceneMove?.Invoke();
                }
            }
            catch (Exception e)
            {
                //예외를 발견하면 앱을 강제 종료합니다
                //에디터 상태라면 플레이 모드를 종료합니다
                Debug.LogException(e);

                if (!isInitialLoadEnd)
                {
                    Debug.ForceLogError("Initial loading failed", nameof(InitialLoadManager));

                    if (isInitialLoadStart)
                        ApplicationForceQuit("Initial loading failed\n\n" + e.GetType().Name + ": " + e.Message + "\n\n" + e.StackTrace.Substring(5));
                }
            }
        }

        [WikiDescription("프로그램의 강제 종료 여부")] public static bool isForceQuit { get; private set; }
        [WikiDescription("프로그램을 강제 종료하지만 로그는 띄워야할때 사용하는 메소드 입니다")]
        public static async void ApplicationForceQuit(string message)
        {
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException();
            else if (isForceQuit)
                return;

            string typeName = Debug.NameOfCallingClass();
            isForceQuit = true;

            await UniTask.WaitUntil(() => UIManager.instance != null);
            await UniTask.NextFrame(PlayerLoopTiming.LastPostLateUpdate);

            if (isInitialLoadEnd)
            {
                SceneManager.LoadScene(0);
                await UniTask.NextFrame(PlayerLoopTiming.LastPostLateUpdate);

                //Background
                Image image = UnityEngine.Object.Instantiate(Kernel.emptyRectTransform, UIManager.instance.kernelCanvas.transform).gameObject.AddComponent<Image>();
                image.color = Color.black;

                image.rectTransform.anchorMin = Vector2.zero;
                image.rectTransform.anchorMax = Vector2.one;

                image.rectTransform.offsetMin = Vector2.zero;
                image.rectTransform.offsetMax = Vector2.zero;

                //Text
                TMP_Text exceptionText = UnityEngine.Object.Instantiate(UIManager.instance.exceptionText, UIManager.instance.kernelCanvas.transform);
                exceptionText.text = $"{typeName}: {message}";

                GameObject[] gameObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(true);
                GameObject kernel = Kernel.instance.gameObject;
                GameObject uiManager = UIManager.instance.gameObject;
                GameObject kernelCanvas = UIManager.instance.kernelCanvas.gameObject;

                for (int i = 0; i < gameObjects.Length; i++)
                {
                    GameObject gameObject = gameObjects[i];
                    if (gameObject != image.gameObject && gameObject != kernel && gameObject != uiManager && gameObject != kernelCanvas && gameObject != exceptionText.gameObject)
                        UnityEngine.Object.DestroyImmediate(gameObjects[i]);
                }

                UnityEngine.Cursor.visible = true;

                isInitialLoadStart = false;
                isSettingLoadEnd = false;
                isInitialLoadEnd = false;
                isSceneMoveEnd = false;
            }
            else
                UnityEngine.Object.Instantiate(UIManager.instance.exceptionText, UIManager.instance.kernelCanvas.transform).text = $"{typeName}: {message}";

            InputManager.forceInputLock = true;
        }
    }
}
