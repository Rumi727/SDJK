using UnityEngine;
using System.IO;
using SCKRM.Compress;
using SCKRM.Threads;
using System;
using SCKRM.UI.SideBar;
using K4.Threading;
using SCKRM.ProjectSetting;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
using B83.Win32;
using System.Collections.Generic;
#else
using System.Reflection;
using UnityEditor;
#endif

namespace SCKRM.DragAndDrop
{
    [WikiDescription("드래그 앤 드랍을 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/Drag And Drop/Drag And Drop Manager")]
    public sealed class DragAndDropManager : ManagerBase<DragAndDropManager>
    {
#if UNITY_EDITOR
        /// <summary>
        /// 이 클래스는 에디터에서만 접근할 수 있습니다
        /// </summary>
        [ProjectSettingSaveLoad]
        public class Data
        {
            [JsonProperty] public static bool editorDADEnable { get; set; } = true;
        }
#endif

        /// <summary>
        /// </summary>
        /// <param name="paths">
        /// 드롭 된 파일 또는 폴더의 경로들
        /// Paths to dropped files or folders
        /// </param>
        /// <param name="isFolder">
        /// 경로가 폴더인지 감지합니다
        /// Detect if path is a folder
        /// </param>
        /// <param name="mousePos">
        /// 드롭 됐을 때의 마우스 위치
        /// mouse position when dropped
        /// </param>
        /// <returns>
        /// 메소드가 파일 감지에 성공했을 경우 true를 반환해야 하며, 감지에 실패했으면 false를 반환해야 합니다
        /// The method should return true if the file was detected successfully, and false if the detection was unsuccessful.
        /// </returns>
        public delegate bool DragAndDropFunc(string path, bool isFolder, Vector2 mousePos, ThreadMetaData threadMetaData);

        [WikiDescription("사용자가 파일을 드래그 앤 드랍할때 발생하는 이벤트 입니다")]
        public static event DragAndDropFunc dragAndDropEvent;



        void Awake()
        {
            if (SingletonCheck(this))
                dragAndDropEvent += ResourcePackDragAndDrop;
        }

        static bool ResourcePackDragAndDrop(string path, bool isFolder, Vector2 mousePos, ThreadMetaData threadMetaData)
        {
            if (!isFolder)
                return false;

            string jsonPath = PathUtility.Combine(path, "pack.json");
            if (File.Exists(jsonPath))
            {
                threadMetaData.name = "notice.running_task.drag_and_drop.resource_pack_load";
                threadMetaData.info = "";

                threadMetaData.progress = 0;
                threadMetaData.maxProgress = 1;



                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                string destPath = PathUtility.Combine(Kernel.resourcePackPath, directoryInfo.Name);

                if (Directory.Exists(destPath))
                {
                    K4UnityThreadDispatcher.Execute(() => NoticeManager.Notice("notice.running_task.drag_and_drop.resource_pack_load.exists", "", NoticeManager.Type.warning));
                    return true;
                }

                Directory.CreateDirectory(destPath);
                DirectoryUtility.Copy(path, destPath);

                threadMetaData.progress = 1;

                return true;
            }

            return false;
        }



#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
        void OnEnable()
        {
            UnityDragAndDropHook.InstallHook();
            UnityDragAndDropHook.OnDroppedFiles += OnFiles;

            void OnFiles(List<string> aFiles, POINT aPos) => DragAndDropEventInvoke(aFiles.ToArray(), new Vector2(aPos.x, ScreenManager.height - aPos.y));
        }

        void OnDisable() => UnityDragAndDropHook.UninstallHook();
#endif



#if UNITY_EDITOR
        string[] tempDragAndDropPath = null;
        bool drag = false;
        Assembly assembly = typeof(EditorWindow).Assembly;
        Type type;
        void Update()
        {
            if (!Data.editorDADEnable)
                return;

            if (type == null)
            {
                type = assembly.GetType("UnityEditor.GameView");
                return;
            }

            string[] paths = UnityEditor.DragAndDrop.paths;
            EditorWindow gameView = EditorWindow.mouseOverWindow;
            if (gameView != null && gameView.GetType() == type)
            {
                if ((paths == null || paths.Length <= 0) && tempDragAndDropPath != null && tempDragAndDropPath.Length > 0)
                {
                    if (drag)
                    {
                        drag = false;
                        DragAndDropEventInvoke(tempDragAndDropPath, UnityEngine.Input.mousePosition);
                    }
                    else
                        drag = true;
                }
            }
            else
                drag = false;

            tempDragAndDropPath = paths;
        }
#endif

        static void DragAndDropEventInvoke(string[] paths, Vector2 mousePos)
        {
            Delegate[] delegates = dragAndDropEvent?.GetInvocationList();
            if (delegates == null || delegates.Length <= 0)
                return;

            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];

                ThreadManager.Create(DragAndDrop, "notice.running_task.drag_and_drop.file_load");

                void DragAndDrop(ThreadMetaData threadMetaData)
                {
                    bool isFolder = Directory.Exists(path);
                    bool isCompressedFile = false;
                    string tempFolderPath = "";
                    if (!isFolder)
                    {
                        if (!File.Exists(path))
                            return;
                        else if (Path.GetExtension(path).ToLower().Equals(".zip"))
                        {
                            string uuid = Guid.NewGuid().ToString();
                            tempFolderPath = PathUtility.Combine(Kernel.temporaryCachePath, uuid);
                            string decompressFolerPath = Path.Combine(tempFolderPath, Path.GetFileNameWithoutExtension(path));
                            if (!CompressFileManager.DecompressZipFile(path, decompressFolerPath, "", threadMetaData))
                            {
                                if (Directory.Exists(tempFolderPath))
                                    Directory.Delete(tempFolderPath, true);

                                return;
                            }

                            path = decompressFolerPath;
                            isFolder = true;
                            isCompressedFile = true;
                        }
                    }

                    threadMetaData.cantCancel = true;

                    if (delegates != null)
                    {
                        for (int j = 0; j < delegates.Length; j++)
                        {
                            try
                            {
                                if (((DragAndDropFunc)delegates[j]).Invoke(path, isFolder, mousePos, threadMetaData))
                                    break;
                            }
                            catch (Exception e)
                            {
                                Debug.LogException(e);
                            }
                        }
                    }

                    if (isCompressedFile && Directory.Exists(tempFolderPath))
                        Directory.Delete(tempFolderPath, true);
                }
            }
        }
    }
}
