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

        protected override void Update()
        {
            if (background == null || background.isRemoved || background.canvasGroup.alpha >= 1)
            {
                while (backgroundRemoveQueue.TryDequeue(out BackgroundEffectPrefab result))
                    result.Remove();
            }
        }

        MapPack lastMapPack;
        BeatValuePairList<BackgroundFileInfoPair> lastBackgrounds = new BeatValuePairList<BackgroundFileInfoPair>(default);
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

                lastBackgrounds.CopyTo(map.globalEffect.backgroundEffect.background.ToArray());
                lastMapPack = mapPack;
            }
        }

        bool BackgroundCheck()
        {
            if (lastBackgrounds.Count != map.globalEffect.backgroundEffect.background.Count)
                return true;

            for (int i = 0; i < map.globalEffect.backgroundEffect.background.Count; i++)
            {
                BeatValuePair<BackgroundFileInfoPair> lastBackground = lastBackgrounds[i];
                BeatValuePair<BackgroundFileInfoPair> background = map.globalEffect.backgroundEffect.background[i];

                if (lastBackground.Equals(background) || lastBackground.Equals(background))
                    return true;
            }

            return false;
        }
    }
}
