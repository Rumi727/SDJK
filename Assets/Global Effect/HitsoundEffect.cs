using SCKRM.Rhythm;
using SCKRM.Sound;
using SDJK.Effect;
using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK
{
    public sealed class HitsoundEffect : Effect.Effect
    {
        public override void Refresh(bool force = false) { }

        double lastValue = -1;
        protected override void RealUpdate()
        {
            int count = GetHitsoundPlayCount(map, RhythmManager.currentBeatSound, ref lastValue);
            for (int i = 0; i < count; i++)
                SoundManager.PlaySound("hitsound.normal", "sdjk", 0.5f, false, 0.95f);
        }

        public static int GetHitsoundPlayCount(MapFile map, double currentBeat, ref double lastValue) => GetHitsoundPlayCount(map, currentBeat, ref lastValue, out _);

        public static int GetHitsoundPlayCount(MapFile map, double currentBeat, ref double lastValue, out int index)
        {
            index = 0;

            if (map == null)
                return 0;

            double value = -1;
            int soundPlayCount = 0;
            if (map.allJudgmentBeat.Count <= 0)
                soundPlayCount = 0;
            else if (map.allJudgmentBeat[0] >= currentBeat)
                soundPlayCount = 1;
            else if (map.allJudgmentBeat.Count >= 2)
            {
                for (int i = 0; i < map.allJudgmentBeat.Count; i++)
                {
                    if (map.allJudgmentBeat[i] >= currentBeat)
                    {
                        double beat = map.allJudgmentBeat[i - 1];
                        value = beat;
                        index = i - 1;
                        soundPlayCount++;

                        for (int j = i - 2; j >= 0; j--)
                        {
                            if (beat == map.allJudgmentBeat[j])
                                soundPlayCount++;
                            else
                                break;
                        }

                        break;
                    }
                }
            }

            if (lastValue != value)
            {
                lastValue = value;
                return soundPlayCount;
            }

            return 0;
        }
    }
}
