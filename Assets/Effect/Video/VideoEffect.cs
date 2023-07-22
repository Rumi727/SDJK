using Newtonsoft.Json;
using SCKRM.Object;
using SCKRM.SaveLoad;
using SDJK.Map;
using UnityEngine;

namespace SDJK.Effect
{
    public sealed class VideoEffect : Effect
    {
        [GeneralSaveLoad]
        public sealed class SaveData
        {
            [JsonProperty] public static bool videoBackgroundEnable { get; set; } = true;
        }

        [SerializeField] string _prefab = "video_effect.video"; public string prefab { get => _prefab; set => _prefab = value; }
        public VideoEffectPrefab video { get; private set; } = null;

        MapPack lastMapPack;
        string lastVideoBackgroundFile;
        string lastVideoBackgroundNightFile;
        double lastVideoOffset;
        public override void Refresh(bool force = false)
        {
            if (force || lastMapPack != mapPack || lastVideoBackgroundFile != map.info.videoBackgroundFile || lastVideoBackgroundNightFile != map.info.videoBackgroundNightFile || lastVideoOffset != map.info.videoOffset)
            {
                if (video != null && !video.isRemoved)
                    video.PadeOut().Forget();

                if (SaveData.videoBackgroundEnable)
                {
                    video = (VideoEffectPrefab)ObjectPoolingSystem.ObjectCreate(prefab, transform, false).monoBehaviour;
                    video.Refresh(effectManager);
                }

                lastVideoBackgroundFile = map.info.videoBackgroundFile;
                lastVideoBackgroundNightFile = map.info.videoBackgroundNightFile;
                lastVideoOffset = map.info.videoOffset;
                lastMapPack = mapPack;
            }
        }
    }
}
