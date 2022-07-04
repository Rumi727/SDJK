using Cysharp.Threading.Tasks;
using SCKRM.Renderer;
using SCKRM.Resource;
using SCKRM.SaveLoad;
using SCKRM.Threads;
using System;
using UnityEngine;
using SCKRM.Input;
using System.IO;

namespace SCKRM
{
    [AddComponentMenu("SC KRM/Kernel/Kernel")]
    public sealed class Kernel : Manager<Kernel>
    {
        public static Version sckrmVersion { get; } = new Version(0, 8, 1);



        public static float fps { get; private set; } = 60;

        public static float deltaTime { get; private set; } = fps60second;
        public static float fpsDeltaTime { get; private set; } = 1;
        public static float unscaledDeltaTime { get; private set; } = fps60second;
        public static float fpsUnscaledDeltaTime { get; private set; } = 1;
        public static float fixedDeltaTime { get; private set; } = fps60second;

        public const float fps60second = 1f / 60f;
        


        /// <summary>
        /// Application.dataPath
        /// </summary>
        public static string dataPath
        {
            get
            {
                if (_dataPath != "")
                    return _dataPath;
                else
                    return _dataPath = Application.dataPath;
            }
        }
        static string _dataPath = "";

        /// <summary>
        /// Application.streamingAssetsPath
        /// </summary>
        public static string streamingAssetsPath { get; } = Application.streamingAssetsPath;

        /// <summary>
        /// Application.persistentDataPath
        /// </summary>
        public static string persistentDataPath
        {
            get
            {
                if (_persistentDataPath != "")
                    return _persistentDataPath;
                else
                    return _persistentDataPath = Application.persistentDataPath;
            }
        }
        static string _persistentDataPath = "";

        /// <summary>
        /// Application.temporaryCachePath
        /// </summary>
        public static string temporaryCachePath
        {
            get
            {
                if (_temporaryCachePath != "")
                    return _temporaryCachePath;
                else
                    return _temporaryCachePath = Application.temporaryCachePath;
            }
        }
        static string _temporaryCachePath = "";

        /// <summary>
        /// Kernel.persistentDataPath + "/Save Data"
        /// </summary>
        public static string saveDataPath
        {
            get
            {
                if (_saveDataPath != "")
                    return _saveDataPath;
                else
                {
                    _saveDataPath = persistentDataPath + "/Save Data";

                    if (!Directory.Exists(_saveDataPath))
                        Directory.CreateDirectory(_saveDataPath);

                    return _saveDataPath;
                }
            }
        }
        static string _saveDataPath = "";

        /// <summary>
        /// Kernel.persistentDataPath + "/Resource Pack"
        /// </summary>
        public static string resourcePackPath
        {
            get
            {
                if (_resourcePackPath != "")
                    return _resourcePackPath;
                else
                {
                    _resourcePackPath = persistentDataPath + "/Resource Pack";

                    if (!Directory.Exists(_resourcePackPath))
                        Directory.CreateDirectory(_resourcePackPath);

                    return _resourcePackPath;
                }
            }
        }
        static string _resourcePackPath = "";

        /// <summary>
        /// Kernel.streamingAssetsPath + "/projectSettings"
        /// </summary>
        public static string projectSettingPath { get; } = streamingAssetsPath + "/projectSettings";



        /// <summary>
        /// Application.companyName
        /// </summary>
        public static string companyName
        {
            get
            {
                if (_companyName != "")
                    return _companyName;
                else
                    return _companyName = Application.companyName;
            }
        }
        static string _companyName = "";

        /// <summary>
        /// Application.productName
        /// </summary>
        public static string productName
        {
            get
            {
                if (_productName != "")
                    return _productName;
                else
                    return _productName = Application.productName;
            }
        }
        static string _productName = "";

        /// <summary>
        /// Application.version
        /// </summary>
        public static string version
        {
            get
            {
                if (_version != "")
                    return _version;
                else
                    return _version = Application.version;
            }
        }
        static string _version = "";



        /// <summary>
        /// Application.unityVersion
        /// </summary>
        public static string unityVersion
        {
            get
            {
                if (_version != "")
                    return _unityVersion;
                else
                    return _unityVersion = Application.unityVersion;
            }
        }
        static string _unityVersion = "";



        /// <summary>
        /// Application.platform
        /// </summary>
        public static RuntimePlatform platform { get; } = Application.platform;



        /// <summary>
        /// 게임의 전체 속도를 결정 합니다
        /// </summary>
        public static float gameSpeed { get; set; } = 1;



#if UNITY_EDITOR
        /// <summary>
        /// Editor: ThreadManager.isMainThread && Application.isEditor
        /// /
        /// Build: const false
        /// </summary>
        public static bool isEditor => ThreadManager.isMainThread && Application.isEditor;

        /// <summary>
        /// Editor: !ThreadManager.isMainThread || Application.isPlaying
        /// /
        /// Build: const true
        /// </summary>
        public static bool isPlaying => !ThreadManager.isMainThread || Application.isPlaying;

        /// <summary>
        /// Editor: !ThreadManager.isMainThread || (Application.isPlaying && !UnityEditor.EditorApplication.isPaused)
        /// /
        /// Build: const true
        /// </summary>
        public static bool isPlayingAndNotPaused => !ThreadManager.isMainThread || (Application.isPlaying && !UnityEditor.EditorApplication.isPaused);
#else
        public const bool isEditor = false;
        public const bool isPlaying = true;
        public const bool isPlayingAndNotPaused = true;
#endif



        /// <summary>
        /// 빈 게임 오브젝트
        /// </summary>
        public static Transform emptyTransform
        {
            get
            {
                if (!isPlaying)
                    throw new NotPlayModePropertyException(nameof(emptyTransform));

                return _emptyTransform;
            }
        }
        static Transform _emptyTransform;

        /// <summary>
        /// 사각 트랜스폼이 추가된 빈 게임 오브젝트
        /// </summary>
        public static RectTransform emptyRectTransform
        {
            get
            {
                if (!isPlaying)
                    throw new NotPlayModePropertyException(nameof(emptyTransform));

                return _emptyRectTransform;
            }
        }
        static RectTransform _emptyRectTransform;



        /// <summary>
        /// 프로그램 종료 이벤트
        /// </summary>
        public static event Func<bool> shutdownEvent;



        void Awake()
        {
            if (SingletonCheck(this))
            {
                DontDestroyOnLoad(instance);

                Transform emptyTransform = new GameObject().transform;
                emptyTransform.SetParent(transform);
                emptyTransform.name = "Empty Transform";

                _emptyTransform = emptyTransform;

                RectTransform emptyRectTransform = new GameObject().AddComponent<RectTransform>();
                emptyRectTransform.SetParent(transform);
                emptyRectTransform.name = "Empty Rect Transform";

                _emptyRectTransform = emptyRectTransform;
            }
        }

        async UniTaskVoid Start()
        {
            if (isEditor)
                return;

            while (true)
            {
                if (InitialLoadManager.isInitialLoadEnd && InputManager.GetKey("kernel.full_screen", InputType.Down, InputManager.inputLockDenyAllForce))
                {
                    if (Screen.fullScreen)
                        Screen.SetResolution((int)(ScreenManager.currentResolution.width / 1.5f), (int)(ScreenManager.currentResolution.height / 1.5f), false);
                    else
                    {
                        Screen.SetResolution(ScreenManager.currentResolution.width, ScreenManager.currentResolution.height, false);

                        if (await UniTask.DelayFrame(4, PlayerLoopTiming.LastPostLateUpdate, AsyncTaskManager.cancelToken).SuppressCancellationThrow())
                            return;

                        Screen.SetResolution(ScreenManager.currentResolution.width, ScreenManager.currentResolution.height, true);
                    }
                }

                if (await UniTask.DelayFrame(1, PlayerLoopTiming.Update, AsyncTaskManager.cancelToken).SuppressCancellationThrow())
                    return;
            }
        }

        void Update()
        {
            //게임 속도를 0에서 100 사이로 정하고, 타임 스케일을 게임 속도로 정합니다
            gameSpeed = gameSpeed.Clamp(0, 100);
            Time.timeScale = gameSpeed;

            //유니티의 내장 변수들은 테스트 결과, 약간의 성능을 더 먹는것으로 확인되었기 때문에
            //이렇게 관리 스크립트가 변수를 할당하고 다른 스크립트가 그 변수를 가져오는것이 성능에 더 도움 되는것을 확인하였습니다
            deltaTime = Time.deltaTime;
            fpsDeltaTime = deltaTime * VideoManager.Data.standardFPS;
            unscaledDeltaTime = Time.unscaledDeltaTime;
            fpsUnscaledDeltaTime = unscaledDeltaTime * VideoManager.Data.standardFPS;
            fixedDeltaTime = 1f / VideoManager.Data.standardFPS;
            Time.fixedDeltaTime = fixedDeltaTime;

            fps = 1f / deltaTime;
        }

        void OnApplicationQuit()
        {
            if (shutdownEvent != null)
            {
                Delegate[] delegates = shutdownEvent.GetInvocationList();
                for (int i = 0; i < delegates.Length; i++)
                {
                    try
                    {
                        if (!((Func<bool>)delegates[i]).Invoke())
#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
                            Application.CancelQuit();
#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }

            AsyncTaskManager.AllAsyncTaskCancel(false);
            ThreadManager.AllThreadRemove();

            if (InitialLoadManager.isInitialLoadEnd)
                SaveLoadManager.SaveAll(SaveLoadManager.generalSLCList, saveDataPath);
        }



        public static event Action allRefreshStart;
        public static event Action allRefreshEnd;
        public static async UniTaskVoid AllRefresh()
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException(nameof(AllRefresh));

            if (!isPlaying)
                throw new NotPlayModeMethodException(nameof(AllRefresh));

            allRefreshStart?.Invoke();

            if (!ResourceManager.isResourceRefesh)
            {
                await ResourceManager.ResourceRefresh(false);
                RendererManager.AllRefresh();

                ResourceManager.GarbageRemoval();
                GC.Collect();
            }

            allRefreshEnd?.Invoke();
        }

        public static void RevealInFinder(string path) => Application.OpenURL("file:///" + path);
    }


    public class NotPlayModeMethodException : Exception
    {
        /// <summary>
        /// It is not possible to use this function when not in play mode.
        /// 플레이 모드가 아닐때 이 함수를 사용하는건 불가능합니다.
        /// </summary>
        public NotPlayModeMethodException() : base("It is not possible to use this function when not in play mode.\n플레이 모드가 아닐때 이 함수를 사용하는건 불가능합니다") { }

        /// <summary>
        /// It is not possible to use {method} functions when not in play mode.
        /// 플레이 모드가 아닐때 이 함수를 사용하는건 불가능합니다.
        /// </summary>
        public NotPlayModeMethodException(string method) : base($"It is not possible to use {method} functions when not in play mode.\n플레이 모드가 아닐때 {method} 함수를 사용하는건 불가능합니다") { }
    }



    public class NotPlayModePropertyException : Exception
    {
        /// <summary>
        /// It is not possible to use this property when not in play mode.
        /// 플레이 모드가 아닐때 이 프로퍼티를 사용하는건 불가능합니다.
        /// </summary>
        public NotPlayModePropertyException() : base("It is not possible to use this property when not in play mode.\n플레이 모드가 아닐때 이 프로퍼티를 사용하는건 불가능합니다") { }

        /// <summary>
        /// It is not possible to use {method} property when not in play mode.
        /// 플레이 모드가 아닐때 이 프로퍼티를 사용하는건 불가능합니다.
        /// </summary>
        public NotPlayModePropertyException(string property) : base($"It is not possible to use {property} propertys when not in play mode.\n플레이 모드가 아닐때 {property} 프로퍼티를 사용하는건 불가능합니다") { }
    }



    public class NotInitialLoadEndMethodException : Exception
    {
        /// <summary>
        /// Initial loading was not finished, but I tried to use a function that needs loading
        /// 초기 로딩이 안끝났는데 로딩이 필요한 함수를 사용하려 했습니다
        /// </summary>
        public NotInitialLoadEndMethodException() : base("Initial loading was not finished, but I tried to use a function that needs loading\n초기 로딩이 안끝났는데 로딩이 필요한 함수를 사용하려 했습니다") { }

        /// <summary>
        /// Initial loading was not finished, but I tried to use a {method} function that needs loading
        /// 초기 로딩이 안끝났는데 로딩이 필요한 {method} 함수를 사용하려 했습니다
        /// </summary>
        public NotInitialLoadEndMethodException(string method) : base($"Initial loading was not finished, but I tried to use a {method} function that needs loading\n초기 로딩이 안끝났는데 로딩이 필요한 {method} 함수를 사용하려 했습니다") { }
    }



    public class NullObjectException : Exception
    {
        /// <summary>
        /// No object in folder
        /// 폴더에 오브젝트가 없습니다
        /// </summary>
        public NullObjectException() : base("No object in folder\n폴더에 오브젝트가 없습니다") { }

        /// <summary>
        /// No object in {objectPath} folder
        /// {objectPath} 폴더에 오브젝트가 없습니다
        /// </summary>
        public NullObjectException(string objectPath) : base($"No object in {objectPath} folder\n{objectPath} 폴더에 오브젝트가 없습니다") { }

        /// <summary>
        /// Object {objectName} does not exist in {objectPath} folder
        /// {objectPath} 폴더에 {objectName} 오브젝트가 없습니다
        /// </summary>
        public NullObjectException(string objectPath, string objectName) : base($"Object {objectName} does not exist in {objectPath} folder\n{objectPath} 폴더에 {objectName} 오브젝트가 없습니다") { }
    }

    public class NullResourceObjectException : Exception
    {
        /// <summary>
        /// No object in resource folder
        /// 리소스 폴더에 오브젝트가 없습니다
        /// </summary>
        public NullResourceObjectException() : base("No object in resource folder\n리소스 폴더에 오브젝트가 없습니다") { }

        /// <summary>
        /// Object {objectName} does not exist in resource folder
        /// 리소스 폴더에 {objectName} 오브젝트가 없습니다
        /// </summary>
        public NullResourceObjectException(string objectName) : base($"Object {objectName} does not exist in resource folder\n리소스 폴더에 {objectName} 오브젝트가 없습니다") { }
    }

    public class NullSceneException : Exception
    {
        /// <summary>
        /// no scene
        /// 씬이 없습니다
        /// </summary>
        public NullSceneException() : base("no scene\n씬이 없습니다") { }

        /// <summary>
        /// {sceneName} no scene
        /// {sceneName} 씬이 없습니다
        /// </summary>
        public NullSceneException(string sceneName) : base($"{sceneName} no scene\n{sceneName} 씬이 없습니다") { }
    }

    public class NullScriptMethodException : Exception
    {
        /// <summary>
        /// Failed to execute function because script does not exist
        /// 스크립트가 없어서 함수를 실행하지 못했습니다
        /// </summary>
        public NullScriptMethodException() : base("Failed to execute function because script does not exist\n스크립트가 없어서 함수를 실행하지 못했습니다") { }

        /// <summary>
        /// Failed to execute function because script asdf does not exist
        /// {script} 스크립트가 없어서 함수를 실행하지 못했습니다
        /// </summary>
        public NullScriptMethodException(string scriptName) : base($"Failed to execute function because script {scriptName} does not exist\n{scriptName} 스크립트가 없어서 함수를 실행하지 못했습니다") { }

        /// <summary>
        /// Failed to execute {methodName} function because script {scriptName} does not exist
        /// {script} 스크립트가 없어서 {method} 함수를 실행하지 못했습니다
        /// </summary>
        public NullScriptMethodException(string scriptName, string methodName) : base($"Failed to execute {methodName} function because script {scriptName} does not exist\n{scriptName} 스크립트가 없어서 {methodName} 함수를 실행하지 못했습니다") { }
    }
}