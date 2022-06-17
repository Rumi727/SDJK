using Newtonsoft.Json;
using SCKRM.Easing;
using SCKRM.Json;
using SCKRM.SaveLoad;
using SCKRM.Sound;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SCKRM.Rhythm
{
    [AddComponentMenu("SC KRM/Rhythm/Rhythm Manager")]
    public sealed class RhythmManager : Manager<RhythmManager>
    {
        [GeneralSaveLoad]
        public sealed class SaveData
        {
            [JsonProperty] public static double screenOffset { get; set; } = 0;
            [JsonProperty] public static double soundOffset { get; set; } = 0;
        }

        public static IBPM iBPM { get; private set; }
        public static IOffset iOffset { get; private set; }
        public static IDropPart iDropPart { get; private set; }
        public static ITime iTime { get; private set; }
        public static ISpeed iSpeed { get; private set; }



        public static bool isPlaying { get; private set; } = false;



        public static bool dropPart { get; set; } = false;



        public static double bpm { get; private set; }
        public static float bpmFpsDeltaTime { get; private set; }
        public static float bpmUnscaledFpsDeltaTime { get; private set; }



        public static float time => iTime != null ? iTime.time : 0;
        public static double currentBeat { get; private set; }
        public static double currentBeatSound { get; private set; }
        public static double currentBeatScreen { get; private set; }
        public static double currentBeat1Beat { get; private set; }

        static double bpmOffsetBeat;
        static double bpmOffsetTime;



        [Obsolete("Use currentBeat1Beat instead")] public static event Action oneBeat;
        [Obsolete("Use currentBeat1Beat instead")] public static event Action oneBeatDropPart;



        void Awake() => SingletonCheck(this);



        static int tempCurrentBeat = 0;
        void Update()
        {
            if (isPlaying)
            {
                if (iBPM == null || iOffset == null || iDropPart == null || iTime == null || iSpeed == null)
                    Stop();
                else
                {
                    SetCurrentBeat();

                    {
                        bpm = iBPM.bpm.GetValue(currentBeat, out double beat, out bool isValueChanged);

                        if (isValueChanged)
                        {
                            BPMChange(bpm, beat);
                            SetCurrentBeat();
                        }
                    }

                    {
                        bpmFpsDeltaTime = (float)(bpm * 0.01f * Kernel.fpsDeltaTime * iSpeed.speed);
                        bpmUnscaledFpsDeltaTime = (float)(bpm * 0.01f * Kernel.fpsUnscaledDeltaTime * iSpeed.speed);
                    }

                    dropPart = iDropPart.dropPart.GetValue();

                    if (tempCurrentBeat != (int)currentBeat && currentBeat >= 0)
                    {
                        oneBeat?.Invoke();
                        if (dropPart)
                            oneBeatDropPart?.Invoke();

                        tempCurrentBeat = (int)currentBeat;
                    }

                    /*Debug.Log("time: " + time);
                    Debug.Log("currentBeat: " + currentBeat);
                    Debug.Log("bpm: " + bpm);
                    Debug.Log("dropPart: " + dropPart);*/
                }
            }
        }

        static void SetCurrentBeat()
        {
            double soundTime = (double)time - iOffset.offset - bpmOffsetTime;
            double bpmDivide60 = bpm / 60d;

            currentBeat = (soundTime * bpmDivide60) + bpmOffsetBeat;
            currentBeatSound = ((soundTime - SaveData.soundOffset) * bpmDivide60) + bpmOffsetBeat;
            currentBeatScreen = ((soundTime - SaveData.screenOffset) * bpmDivide60) + bpmOffsetBeat;

            currentBeat1Beat = currentBeat.Reapeat(1);
        }

        static void BPMChange(double bpm, double offsetBeat)
        {
            bpmOffsetBeat = offsetBeat;

            bpmOffsetTime = 0;
            double tempBeat = 0;
            for (int i = 0; i < iBPM.bpm.Count; i++)
            {
                if (iBPM.bpm[0].beat >= offsetBeat)
                    break;

                double tempBPM;
                if (i - 1 < 0)
                    tempBPM = iBPM.bpm[0].value;
                else
                    tempBPM = iBPM.bpm[i - 1].value;

                bpmOffsetTime += (iBPM.bpm[i].beat - tempBeat) * (60d / tempBPM);
                tempBeat = iBPM.bpm[i].beat;

                if (iBPM.bpm[i].beat >= offsetBeat)
                    break;
            }

            RhythmManager.bpm = bpm;
        }

        public static void Play(IBPM iBPM, IOffset iOffset, IDropPart iDropPart, SoundPlayer soundPlayer) => Play(iBPM, iOffset, iDropPart, soundPlayer, soundPlayer);
        public static void Play(IBPM iBPM, IOffset iOffset, IDropPart iDropPart, ITime iTime, ISpeed iSpeed)
        {
            currentBeat = 0;
            bpmOffsetBeat = 0;
            bpmOffsetTime = 0;

            RhythmManager.iBPM = iBPM;
            RhythmManager.iOffset = iOffset;
            RhythmManager.iDropPart = iDropPart;
            RhythmManager.iTime = iTime;
            RhythmManager.iSpeed = iSpeed;

            RhythmManager.iTime.timeChanged += SoundPlayerTimeChange;
            isPlaying = true;
        }

        public static void Stop()
        {
            currentBeat = 0;
            bpmOffsetBeat = 0;
            bpmOffsetTime = 0;

            if (iTime != null)
                iTime.timeChanged -= SoundPlayerTimeChange;

            iBPM = null;
            iOffset = null;
            iDropPart = null;
            iTime = null;
            iSpeed = null;

            isPlaying = false;
        }

        static void SoundPlayerTimeChange()
        {
            for (int i = 0; i < iBPM.bpm.Count; i++)
            {
                {
                    BeatValuePair<double> bpm = iBPM.bpm[i];
                    BPMChange(bpm.value, bpm.beat);
                    SetCurrentBeat();
                }

                if (bpmOffsetTime >= time)
                {
                    if (i - 1 >= 0)
                    {
                        BeatValuePair<double> bpm = iBPM.bpm[i - 1];
                        SetCurrentBeat();
                        BPMChange(bpm.value, bpm.beat);
                    }

                    break;
                }
            }
        }
    }
}