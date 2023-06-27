//얘는 빌드 코드기 때문에 에디터에서도 작동해야함
#if UNITY_ANDROID || ENABLE_ANDROID_SUPPORT
using SCKRM.Compress;
using System.IO;
using UnityEditor;

namespace SCKRM.Editor
{
    static class AndroidSupport
    {
        static readonly string tempStreamingAssetsFolderPath = PathUtility.Combine(Directory.GetCurrentDirectory(), "Temp" + Kernel.streamingAssetsFolderName);
        static readonly string zipPath = PathUtility.Combine(Kernel.streamingAssetsPath, Kernel.streamingAssetsFolderName + ".zip");

        [InitializeOnLoadMethod]
        static void Init()
        {
            BuildHandler.onBuildStarted += OnPreprocessBuild;
            BuildHandler.onBuildFailure += OnPostprocessBuild;
            BuildHandler.onBuildSuccess += OnPostprocessBuild;
        }

        static bool start = false;
        static bool OnPreprocessBuild()
        {
            if (!EditorUtility.DisplayDialog("Warning",
@"Compress the streaming assets to support Android, then build, and revert changes when the build is finished.
Please note that this may be slow depending on the size of the project.

This operation cannot be undone midway (the editor freezes during operation)

This operation is unstable!
Changes cannot be undone if an error is made along the way.
It is recommended to set up a git-like system to make the operation undoable.", "So what!, build it fast!", "I hate anything slow or unstable!"))
                return false;

            if (!Directory.Exists(Kernel.streamingAssetsPath))
                return true;

            start = true;

            Debug.Log($"{nameof(Kernel.streamingAssetsPath)} : {Kernel.streamingAssetsPath}");
            Debug.Log($"{nameof(tempStreamingAssetsFolderPath)} : {tempStreamingAssetsFolderPath}");

            if (Directory.Exists(tempStreamingAssetsFolderPath))
            {
                EditorUtility.DisplayDialog("Warning", $"The folder already exists in the '{tempStreamingAssetsFolderPath}' path!", "Build Cancel");
                start = false;

                return false;
            }

            if (!File.Exists(zipPath))
            {
                File.Delete(zipPath);
                AssetDatabase.Refresh();
            }

            Directory.Move(Kernel.streamingAssetsPath, tempStreamingAssetsFolderPath);
            Directory.CreateDirectory(Kernel.streamingAssetsPath);

            CompressFileManager.CompressZipFile(tempStreamingAssetsFolderPath, zipPath, "", null, x => Path.GetExtension(x) != ".meta");
            AssetDatabase.Refresh();

            return true;
        }

        static void OnPostprocessBuild()
        {
            if (!start)
                return;

            start = false;

            if (Directory.Exists(tempStreamingAssetsFolderPath))
            {
                Directory.Delete(Kernel.streamingAssetsPath, true);
                Directory.Move(tempStreamingAssetsFolderPath, Kernel.streamingAssetsPath);

                AssetDatabase.Refresh();
            }
        }
    }
}
#endif