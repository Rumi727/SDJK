using Newtonsoft.Json;
using SCKRM.SaveLoad;
using SCKRM.Sound;
using System;
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

        [WikiDescription("현재 BPM 델타타임")] public static float bpmDeltaTime { get; private set; }
        [WikiDescription("현재 스케일 되지 않은 BPM 델타타임")] public static float bpmUnscaledDeltaTime { get; private set; }

        [WikiDescription("현재 BPM FPS 델타타임")] public static float bpmFpsDeltaTime { get; private set; }
        [WikiDescription("현재 스케일 되지 않은 BPM FPS 델타타임")] public static float bpmUnscaledFpsDeltaTime { get; private set; }



        [WikiDescription("현재 시간")]
        public static double time
        {
            get => _time;
            set
            {
                if (!isPlaying)
                    return;
                else if (soundPlayer != null || soundPlayer.isRemoved)
                    _time = value;
                else //사운드 플레이어의 구현이 정상적이라면 사운드 플레이어의 시간이 변경됨과 동시에 이벤트로인해 time의 시간도 바뀌게 됩니다
                    soundPlayer.time = (float)value;
            }
        }
        static double _time;

        public static double currentSpeed { get; private set; }

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
        static bool isStart = false;
        static bool isEnd = false;
        void Update()
        {
            if (isPlaying)
            {
                if (soundPlayer != null)
                {
                    if (soundPlayer.isActived)
                    {
                        soundPlayer.speed = soundPlayer.speed.Clamp(0);
                        currentSpeed = soundPlayer.speed;
                    }
                    else
                        soundPlayer = null;
                }

                double plusValue = Kernel.deltaTime * currentSpeed;
                _time += plusValue;

                if (soundPlayer != null && soundPlayer.isActived)
                {
                    double sync = soundPlayer.time - time;
                    if (time < 0)
                    {
                        if (soundPlayer.time != 0)
                        {
                            timeChangedEventLock = true;
                            soundPlayer.time = 0;
                            timeChangedEventLock = false;
                        }

                        soundPlayer.isPaused = true;
                        isStart = true;
                    }
                    else if (time > soundPlayer.length - MathTool.epsilonFloatWithAccuracy)
                    {
                        if (soundPlayer.loop)
                            _time = 0;
                        else
                        {
                            soundPlayer.isPaused = true;
                            isEnd = true;
                        }
                    }
                    else if (sync.Abs() >= 0.015625f)
                    {
                        if (sync >= 0)
                        {
                            _time = soundPlayer.time;
                            Debug.Log("Slow sync correction");
                        }
                        else
                        {
                            _time -= plusValue;
                            Debug.Log("Fast sync correction");
                        }
                    }

                    if (isStart && time >= 0)
                    {
                        soundPlayer.isPaused = false;
                        isStart = false;
                    }
                    else if (isEnd && time <= soundPlayer.length - MathTool.epsilonFloatWithAccuracy)
                    {
                        soundPlayer.isPaused = false;
                        isEnd = false;
                    }
                }

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
                    bpmDeltaTime = (float)(bpm / 60 * Kernel.deltaTime * currentSpeed);
                    bpmUnscaledDeltaTime = (float)(bpm / 60 * Kernel.unscaledDeltaTime * currentSpeed);

                    bpmFpsDeltaTime = bpmDeltaTime * VideoManager.Data.standardFPS;
                    bpmUnscaledFpsDeltaTime = bpmUnscaledDeltaTime * VideoManager.Data.standardFPS;
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

            Debug.Log("BPM Changed");
            Debug.Log("BPM: " + bpm);
            Debug.Log("BPM Offset Beat: " + bpmOffsetBeat);
            Debug.Log("BPM Offset Time: " + bpmOffsetTime);

        }

        [WikiDescription("리듬 재생")]
        public static void Play(BeatValuePairList<double> bpmList, double offset, BeatValuePairList<bool> dropPartList, ISoundPlayer soundPlayer = null, double startDelay = 0)
        {
            _time = -(startDelay - offset).Clamp(0);

            currentBeat = double.MinValue;
            currentBeat1Beat = 0;
            currentBeatScreen = double.MinValue;
            currentBeatSound = double.MinValue;

            bpmOffsetBeat = 0;
            bpmOffsetTime = 0;

            RhythmManager.bpmList = bpmList;
            RhythmManager.offset = offset;
            RhythmManager.dropPartList = dropPartList;
            RhythmManager.soundPlayer = soundPlayer;

            if (RhythmManager.soundPlayer != null)
                RhythmManager.soundPlayer.timeChanged += SoundPlayerTimeChange;

            isPlaying = true;
            isStart = false;
            isEnd = false;

            FixBPM();

            Debug.Log("Play");
        }

        [WikiDescription("리듬 정지")]
        public static void Stop()
        {
            _time = 0;

            currentBeat = double.MinValue;
            currentBeat1Beat = 0;
            currentBeatScreen = double.MinValue;
            currentBeatSound = double.MinValue;

            bpmOffsetBeat = 0;
            bpmOffsetTime = 0;

            if (soundPlayer != null)
                soundPlayer.timeChanged -= SoundPlayerTimeChange;

            bpmList = null;
            offset = 0;
            dropPartList = null;
            soundPlayer = null;

            isPlaying = false;
            isStart = false;
            isEnd = false;

            Debug.Log("Stop");
        }

        public static void SoundPlayerChange(ISoundPlayer soundPlayer)
        {
            if (!isPlaying)
                return;

            if (RhythmManager.soundPlayer != null)
                RhythmManager.soundPlayer.timeChanged -= SoundPlayerTimeChange;

            RhythmManager.soundPlayer = soundPlayer;

            if (RhythmManager.soundPlayer != null)
                RhythmManager.soundPlayer.timeChanged += SoundPlayerTimeChange;

            Debug.Log("Sound Player Changed");
        }

        static bool timeChangedEventLock = false;
        static void SoundPlayerTimeChange()
        {
            if (timeChangedEventLock)
                return;

            _time = soundPlayer.time;
            FixBPM();

            Debug.Log("Sound Player Time Changed");
        }

        static void FixBPM()
        {
            for (int i = 0; i < bpmList.Count; i++)
            {
                {
                    BeatValuePair<double> bpm = bpmList[i];
                    BPMChange(bpm.value, bpm.beat);
                }

                if (bpmOffsetTime > time - offset)
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
            Debug.Log("Fix BPM");
        }
    }
}