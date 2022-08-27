using SCKRM;
using SCKRM.Rhythm;
using SCKRM.Sound;
using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK
{
    public class ADOFAIMapTest : MonoBehaviour
    {
        [SerializeField] bool hitSoundPlay = true;
        [SerializeField] bool effectPlay = true;

        double tempValue = -1;
        void Update()
        {
            Map.Map map = MapManager.selectedMap;

            if (hitSoundPlay)
            {
                double value;
                if (map.allBeat.Count <= 0)
                {
                    value = -1;
                    tempValue = -1;
                }
                else if (map.allBeat.Count <= 1 || map.allBeat[0] >= RhythmManager.currentBeat)
                {
                    value = -1;
                    tempValue = -1;
                }
                else
                {
                    int findIndex = map.allBeat.FindIndex(x => x >= RhythmManager.currentBeat);
                    if (findIndex < 0)
                        findIndex = map.allBeat.Count;

                    value = map.allBeat[findIndex - 1];
                }

                if (!tempValue.Equals(value))
                    SoundManager.PlaySound("hitsound.kick", "sdjk", 2);

                tempValue = value;
            }

            if (effectPlay)
            {
                transform.position = map.globalEffect.cameraPos.GetValue() + new Vector3(0, 0, (float)(-14 * map.globalEffect.cameraZoom.GetValue() + 14));
                transform.eulerAngles = map.globalEffect.cameraRotation.GetValue();
            }
        }
    }
}
