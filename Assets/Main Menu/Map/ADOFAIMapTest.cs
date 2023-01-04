using SCKRM;
using SCKRM.Rhythm;
using SCKRM.Sound;
using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.MainMenu
{
    public class ADOFAIMapTest : MonoBehaviour
    {
        [SerializeField] bool hitSoundPlay = true;

        double tempValue = -1;
        void Update()
        {
            Map.MapFile map = MapManager.selectedMap;
            if (map == null)
                return;

            if (hitSoundPlay)
            {
                double value;
                if (map.allJudgmentBeat.Count <= 0)
                {
                    value = -1;
                    tempValue = -1;
                }
                else if (map.allJudgmentBeat.Count <= 1 || map.allJudgmentBeat[0] >= RhythmManager.currentBeat)
                {
                    value = -1;
                    tempValue = -1;
                }
                else
                {
                    int findIndex = map.allJudgmentBeat.FindIndex(x => x >= RhythmManager.currentBeat);
                    if (findIndex < 0)
                        findIndex = map.allJudgmentBeat.Count;

                    value = map.allJudgmentBeat[findIndex - 1];
                }

                if (!tempValue.Equals(value))
                    SoundManager.PlaySound("hitsound.normal", "sdjk", 0.95f);

                tempValue = value;
            }
        }
    }
}
