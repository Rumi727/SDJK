using SCKRM.Discord;
using SCKRM.NTP;
using SDJK.Effect;
using System;
using UnityEngine;

namespace SDJK.Discord
{
    public sealed class SDJKDiscordPresence : MonoBehaviour
    {
        [SerializeField] EffectManager _effectManager;
        public EffectManager effectManager => _effectManager;

        long startTime = 0;
        void Awake() => startTime = ((DateTimeOffset)NTPDateTime.utcNow).ToUnixTimeMilliseconds();

        void Update()
        {
            if (effectManager == null || effectManager.selectedMap == null)
                DiscordManager.UpdateActivity("Main Menu", null, null, null, null, null, startTime);
            else
                DiscordManager.UpdateActivity(
                    $"{effectManager.selectedMap.info.artist} - {effectManager.selectedMap.info.songName} [{effectManager.selectedMap.info.difficultyLabel}]",
                    $"",
                    null, null, "", "", startTime);
        }
    }
}
