using SCKRM.SaveLoad;
using Newtonsoft.Json;
using SCKRM.ProjectSetting;

namespace SCKRM.Splash
{
    [WikiDescription("스플래시 스크린을 관리하는 클래스 입니다")]
    public static class SplashScreen
    {
        [ProjectSettingSaveLoad]
        public sealed class Data
        {
            [JsonProperty] public static string splashScreenPath { get; set; } = "Assets/SC KRM/Splash Screen";
            [JsonProperty] public static string splashScreenName { get; set; } = "Splash Screen";

            [JsonProperty] public static string sceneLoadingScenePath { get; set; } = "Assets/SC KRM/Scene";
            [JsonProperty] public static string sceneLoadingSceneName { get; set; } = "Scene Load Scene";

            [JsonProperty] public static string kernelObjectPath { get; set; } = "Assets/SC KRM";
            [JsonProperty] public static string kernelObjectName { get; set; } = "Kernel";
        }

        [GeneralSaveLoad]
        public sealed class SaveData
        {
            [JsonProperty] public static bool allowProgressBarShow { get; set; } = false;
        }

        [WikiDescription("스플래시 스크린이 재생 중인지 여부")]
        public static bool isAniPlaying { get; set; } = true;
    }
}