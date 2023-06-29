using SCKRM.Resource;
using SCKRM.Rhythm;
using SDJK.Map;
using System.Collections.Generic;

namespace SDJK.Effect
{
    public sealed class SoundEffect : Effect
    {
        Dictionary<string, HitsoundEffect.HitsoundInfo> loadedSounds = new Dictionary<string, HitsoundEffect.HitsoundInfo>();

        double lastBeat = 0;
        protected override void RealUpdate()
        {
            TypeList<HitsoundFile> sounds = map.globalEffect.playSounds.GetValue(RhythmManager.currentBeatSound, out double beat);
            if (RhythmManager.currentBeatSound >= beat && lastBeat != beat)
            {
                for (int i = 0; i < sounds.Count; i++)
                    HitsoundEffect.CustomHitsoundPlay(loadedSounds, map, this, sounds[i]);

                lastBeat = beat;
            }
        }

        void OnDestroy()
        {
            foreach (var item in loadedSounds)
            {
                if (item.Value.soundData != null && item.Value.soundData.sounds != null)
                {
                    for (int j = 0; j < item.Value.soundData.sounds.Length; j++)
                    {
                        SoundMetaData soundMetaData = item.Value.soundData.sounds[j];
                        if (soundMetaData.audioClip != null)
                            Destroy(soundMetaData.audioClip);
                    }
                }
            }
        }
    }
}
