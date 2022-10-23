using Newtonsoft.Json;
using SCKRM.Object;
using SCKRM.ProjectSetting;
using System.Collections.Generic;

namespace SCKRM.Loading
{
    public static class LoadingAniManager
    {
        [ProjectSettingSaveLoad]
        public class Data
        {
            [JsonProperty] public static float longLoadingTime { get; set; } = 1.5f;
            [JsonProperty] public static float aniLerp { get; set; } = 0.2f;
        }

        public static List<LoadingAni> loadingAnis { get; } = new List<LoadingAni>();
        public static bool isLoading => loadingAnis.Count > 0;

        public static LoadingAni LoadingStart()
        {
            LoadingAni loadingAni = (LoadingAni)ObjectPoolingSystem.ObjectCreate("loading_ani_manager.loading_ani").monoBehaviour;
            loadingAnis.Add(loadingAni);

            return loadingAni;
        }
    }
}
