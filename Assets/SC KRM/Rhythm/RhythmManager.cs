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
    [WikiDescription("리듬을 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/Rhythm/Rhythm Manager")]
    public sealed class RhythmManager : Manager<RhythmManager>
    {
        [GeneralSaveLoad]
        public sealed class SaveData
        {
            [JsonProperty] public static double screenOffset { get; set; } = 0;
            [JsonProperty] public static double soundOffset { get; set; } = 0;
        }

        [WikiDescription("현재 사운드 플레이어")] public static ISoundPlayer soundPlayer { get; private set; }
        [WikiDescription("현재 BPM 리스트")] public static BeatValuePairList<double> bpmList { get; private set; }
        [WikiDescription("현재 오프셋")] public static double offset { get; private set; }
        [WikiDescription("현재 드롭파트 리스트")] public static BeatValuePairList<bool> dropPartList { get; private set; }



        [WikiDescription("리듬이 플레이 중 여부")] public static bool isPlaying { get; private set; } = false;



        [WikiDescription("현재 드롭파트인지 여부")] public static bool dropPart { get; set; } = false;



        [WikiDescription("현재 BPM")] public static double bpm { get; private set; }
        [WikiDescription("현재 BPM FPS 델타타임")] public static float bpmFpsDeltaTime { get; private set; }
        [WikiDescription("현재 스케일 되지 않은 BPM FPS 델타타임")] public static float bpmUnscaledFpsDeltaTime { get; private set; }



        [WikiDescription("현재 시간")] public static float time => soundPlayer != null ? soundPlayer.time : 0;
        [WikiDescription("현재 비트")] public static double currentBeat { get; private set; }
        [WikiDescription("현재 사운드 비트")] public static double currentBeatSound { get; private set; }
        [WikiDescription("현재 스크린 비트")] public static double currentBeatScreen { get; private set; }
        [WikiDescription("현재 1 비트")] public static double currentBeat1Beat { get; private set; }

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
                if (soundPlayer == null || soundPlayer.isRemoved || bpmList == null || dropPartList == null)
                    Stop();
                else
                {
                    SetCurrentBeat();

                    {
                        bpm = bpmList.GetValue(currentBeat, out double beat, out bool isValueChanged);

                        if (isValueChanged)
                        {
                            BPMChange(bpm, beat);
                            SetCurrentBeat();
                        }
                    }

                    {
                        bpmFpsDeltaTime = (float)(bpm * 0.01f * Kernel.fpsDeltaTime * soundPlayer.speed);
                        bpmUnscaledFpsDeltaTime = (float)(bpm * 0.01f * Kernel.fpsUnscaledDeltaTime * soundPlayer.speed);
                    }

                    dropPart = dropPartList.GetValue();

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
            double soundTime = (double)time - offset - bpmOffsetTime;
            double bpmDivide60 = bpm / 60d;

            currentBeat = (soundTime * bpmDivide60) + bpmOffsetBeat;
            currentBeatSound = ((soundTime - SaveData.soundOffset) * bpmDivide60) + bpmOffsetBeat;
            currentBeatScreen = ((soundTime - SaveData.screenOffset) * bpmDivide60) + bpmOffsetBeat;

            currentBeat1Beat = currentBeat.Reapeat(1);
        }

        static void BPMChange(double bpm, double offsetBeat)
        {
            offsetBeat = offsetBeat.Clamp(0);
            bpmOffsetBeat = offsetBeat;

            bpmOffsetTime = 0;
            double tempBeat = 0;
            for (int i = 0; i < bpmList.Count; i++)
            {
                double beat = bpmList[i].beat.Clamp(0);
                if (bpmList[0].beat.Clamp(0) >= offsetBeat)
                    break;

                double tempBPM;
                if (i - 1 < 0)
                    tempBPM = bpmList[0].value;
                else
                    tempBPM = bpmList[i - 1].value;

                bpmOffsetTime += (beat - tempBeat) * (60d / tempBPM);
                tempBeat = beat;

                if (beat >= offsetBeat)
                    break;
            }

            RhythmManager.bpm = bpm;
        }

        [WikiDescription("리듬 재생")]
        public static void Play(BeatValuePairList<double> bpmList, double offset, BeatValuePairList<bool> dropPartList, ISoundPlayer soundPlayer)
        {
            currentBeat = 0;
            bpmOffsetBeat = 0;
            bpmOffsetTime = 0;

            RhythmManager.bpmList = bpmList;
            RhythmManager.offset = offset;
            RhythmManager.dropPartList = dropPartList;
            RhythmManager.soundPlayer = soundPlayer;

            RhythmManager.soundPlayer.timeChanged += SoundPlayerTimeChange;
            isPlaying = true;
        }

        [WikiDescription("리듬 정지")]
        public static void Stop()
        {
            currentBeat = 0;
            bpmOffsetBeat = 0;
            bpmOffsetTime = 0;

            if (soundPlayer != null)
                soundPlayer.timeChanged -= SoundPlayerTimeChange;

            bpmList = null;
            offset = 0;
            dropPartList = null;
            soundPlayer = null;

            isPlaying = false;
        }

        static void SoundPlayerTimeChange()
        {
            for (int i = 0; i < bpmList.Count; i++)
            {
                {
                    BeatValuePair<double> bpm = bpmList[i];
                    BPMChange(bpm.value, bpm.beat);
                }

                if (bpmOffsetTime >= time)
                {
                    if (i - 1 >= 0)
                    {
                        BeatValuePair<double> bpm = bpmList[i - 1];
                        BPMChange(bpm.value, bpm.beat);
                    }

                    break;
                }
            }

            SetCurrentBeat();
        }
    }
}