using SCKRM.Object;
using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Effect
{
    public sealed class BackgroundEffect : Effect
    {
        [SerializeField] string _prefab = "background_effect.background"; public string prefab { get => _prefab; set => _prefab = value; }
        public BackgroundEffectPrefab background { get; private set; } = null;

        MapPack lastMapPack;
        BeatValuePairList<BackgroundEffectPair> lastBackgrounds = new BeatValuePairList<BackgroundEffectPair>();
        public override void Refresh(bool force = false)
        {
            if (background != null && !background.isRemoved)
                background.padeOut = true;

            if (force || BackgroundCheck() || lastMapPack != mapPack)
            {
                background = (BackgroundEffectPrefab)ObjectPoolingSystem.ObjectCreate(prefab, transform, false).monoBehaviour;
                background.Refresh(map);

                lastBackgrounds.CopyTo(map.globalEffect.background.ToArray());
                lastMapPack = mapPack;
            }
        }

        bool BackgroundCheck()
        {
            if (lastBackgrounds.Count != map.globalEffect.background.Count)
                return true;

            for (int i = 0; i < map.globalEffect.background.Count; i++)
            {
                BackgroundEffectPair lastBackground = lastBackgrounds[i].value;
                BackgroundEffectPair background = map.globalEffect.background[i].value;

                if (lastBackground.backgroundFile != background.backgroundFile || lastBackground.backgroundNightFile != background.backgroundNightFile)
                    return true;
            }

            return false;
        }
    }
}
