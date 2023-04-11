using SCKRM.Discord;
using SCKRM.NTP;
using SDJK.Effect;
using SDJK.Map;
using SDJK.Ruleset;
using System;
using UnityEngine;

namespace SDJK.Discord
{
    public sealed class SDJKDiscordPresence : MonoBehaviour
    {
        public EffectManager effectManager => _effectManager; [SerializeField] EffectManager _effectManager;
        public bool isMainMenu { get => _isMainMenu; set => _isMainMenu = value; } [SerializeField] bool _isMainMenu = false;

        long startTime = 0;
        void Awake() => startTime = ((DateTimeOffset)NTPDateTime.utcNow).ToUnixTimeMilliseconds();

        IRuleset lastRuleset;
        MapFile lastMap;
        void Update()
        {
            if (effectManager != null && (lastRuleset != effectManager.selectedRuleset || (!isMainMenu && lastMap != effectManager.selectedMap)))
            {
                UpdateActivity();

                lastRuleset = effectManager.selectedRuleset;
                lastMap = effectManager.selectedMap;
            }
        }

        public void UpdateActivity()
        {
            string details;
            string state;

            if (effectManager != null)
            {
                if (isMainMenu)
                {
                    details = "Main Menu";
                    state = null;
                }
                else
                {
                    if (effectManager.selectedMap != null)
                        details = $"{effectManager.selectedMap.info.artist} - {effectManager.selectedMap.info.songName} [{effectManager.selectedMap.info.difficultyLabel}]";
                    else
                        details = null;

                    state = $"Playing {effectManager.selectedRuleset.displayName} Ruleset";
                }

                DiscordManager.UpdateActivity(
                    details,
                    state,
                    null,
                    null,
                    effectManager.selectedRuleset.discordIconKey,
                    effectManager.selectedRuleset.displayName,
                    startTime);
            }
        }
    }
}
