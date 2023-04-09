using SCKRM;
using SCKRM.Object;
using SCKRM.Rhythm;
using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Effect
{
    public sealed class BackgroundEffect : Effect
    {
        [SerializeField] string _prefab = "background_effect.background"; public string prefab { get => _prefab; set => _prefab = value; }

        public BackgroundEffectPrefab background { get; private set; } = null;
        Queue<BackgroundEffectPrefab> backgroundRemoveQueue = new Queue<BackgroundEffectPrefab>();

        void Update()
        {
            if (background == null || background.isRemoved || background.canvasGroup.alpha >= 1)
            {
                while (backgroundRemoveQueue.TryDequeue(out BackgroundEffectPrefab result))
                    result.Remove();
            }
        }

        MapPack lastMapPack;
        BeatValuePairList<BackgroundEffectPair> lastBackgrounds = new BeatValuePairList<BackgroundEffectPair>(default);
        public override void Refresh(bool force = false)
        {
            if (force || BackgroundCheck() || lastMapPack != mapPack)
            {
                if (background != null)
                {
                    backgroundRemoveQueue.Enqueue(background);
                    background.isRemoveQueue = true;
                }

                background = (BackgroundEffectPrefab)ObjectPoolingSystem.ObjectCreate(prefab, transform, false).monoBehaviour;
                background.Refresh(effectManager);

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
                BeatValuePair<BackgroundEffectPair> lastBackground = lastBackgrounds[i];
                BeatValuePair<BackgroundEffectPair> background = map.globalEffect.background[i];

                if (lastBackground.Equals(background) || lastBackground.Equals(background))
                    return true;
            }

            return false;
        }
    }
}
