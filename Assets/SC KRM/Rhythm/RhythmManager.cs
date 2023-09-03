using Newtonsoft.Json;
using SCKRM.SaveLoad;
using SCKRM.Sound;
using System;
using System.Diagnostics;
using UnityEngine;

namespace SCKRM.Rhythm
{
    [WikiDescription("리듬을 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/Rhythm/Rhythm Manager")]
    public sealed class RhythmManager : ManagerBase<RhythmManager>
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
        [WikiDescription("시작 딜레이")] public static double startDelay { get; private set; }
        [WikiDescription("현재 유키 모드 리스트")] public static BeatValuePairList<bool> yukiModeList { get; private set; }



        [WikiDescription("리듬이 플레이 중 여부")] public static bool isPlaying { get; private set; } = false;



        [WikiDescription("현재 스크린 비트가 유키 모드인지 여부")] public static bool screenYukiMode { get; set; } = false;



        [WikiDescription("현재 BPM")] public static double bpm { get; private set; }

        [WikiDescription("현재 BPM 델타타임")] public static float bpmDeltaTime { get; private set; }
        [WikiDescription("현재 스케일 되지 않은 BPM 델타타임")] public static float bpmUnscaledDeltaTime { get; private set; }

        [WikiDescription("현재 BPM FPS 델타타임")] public static float bpmFpsDeltaTime { get; private set; }
        [WikiDescription("현재 스케일 되지 않은 BPM FPS 델타타임")] public static float bpmUnscaledFpsDeltaTime { get; private set; }



        [WikiDescription("현재 시간")]
        public static double time
        {
            get => internalTime - offset;
            set => internalTime = value + offset;
        }

        [WikiDescription("내부 시간")]
        public static double internalTime
        {
            get => _internalTime;
            set
            {
                if (!isPlaying)
                    return;
                else if (soundPlayer == null || soundPlayer.isRemoved || soundPlayer.IsDestroyed())
                    _internalTime = value;
                else
                {
                    timeChangedEventLock = true;

                    _internalTime = value;
                    soundPlayer.time = (float)_internalTime.Clamp(0, soundPlayer.length - 0.01f);

                    FixBPM();

                    timeChangedEventLock = false;
                }
            }
        }
        static double _internalTime;

        public static bool isPaused
        {
            get
            {
                if (soundPlayer != null && !soundPlayer.isRemoved && !soundPlayer.IsDestroyed() && internalTime >= 0 && internalTime <= soundPlayer.length - MathUtility.epsilonFloatWithAccuracy)
                    _isPaused = soundPlayer.isPaused;

                return _isPaused;
            }
            set
            {
                _isPaused = value;

                if (soundPlayer != null && !soundPlayer.isRemoved && !soundPlayer.IsDestroyed() && internalTime >= 0 && internalTime <= soundPlayer.length - MathUtility.epsilonFloatWithAccuracy)
                    soundPlayer.isPaused = value;
            }
        }
        static bool _isPaused;

        public static double speed
        {
            get => _speed;
            set
            {
                _speed = value;

                if (soundPlayer != null && !soundPlayer.isRemoved && !soundPlayer.IsDestroyed())
                    soundPlayer.speed = (float)value;
            }
        }
        static double _speed;

        [WikiDescription("현재 비트")] public static double currentBeat { get; private set; }
        [WikiDescription("현재 사운드 비트")] public static double currentBeatSound { get; private set; }
        [WikiDescription("현재 스크린 비트")] public static double currentBeatScreen { get; private set; }
        [WikiDescription("현재 1 스크린 비트")] public static double currentBeatScreen1Beat { get; private set; }

        [WikiDescription("오디오 델타 타임")] public static double audioDeltaTime { get; private set; }

        public static double bpmOffsetBeat { get; private set; }
        public static double bpmOffsetTime { get; private set; }



        [Obsolete("Use currentBeat1Beat instead")] public static event Action oneBeat;
        [Obsolete("Use currentBeat1Beat instead")] public static event Action oneBeatYukiMode;



        void Awake() => SingletonCheck(this);



        static int tempCurrentBeat = 0;
        static bool isStart = false;
        static double lastBPMBeat = 0;
        static double lastRealAudioTime = 0;
        static Stopwatch audioDeltaTimeStopwatch = new Stopwatch();
        void Update()
        {
            if (isPlaying)
            {
                if (soundPlayer != null)
                {
                    if (soundPlayer.isActived && !soundPlayer.IsDestroyed())
                        soundPlayer.speed = (float)speed;
                    else
                        soundPlayer = null;
                }

                double timePlusValue = Kernel.deltaTime * speed;
                if (soundPlayer != null && soundPlayer.isActived)
                {
                    //Audio Delta Time
                    {
                        audioDeltaTime = audioDeltaTimeStopwatch.Elapsed.TotalSeconds;

                        if (soundPlayer.time != lastRealAudioTime)
                        {
                            audioDeltaTimeStopwatch.Restart();
                            lastRealAudioTime = soundPlayer.time;
                        }
                    }

                    double sync = soundPlayer.time - internalTime;
                    if (internalTime < 0)
                    {
                        if (soundPlayer.time != 0)
                        {
                            timeChangedEventLock = true;
                            soundPlayer.time = 0;
                            timeChangedEventLock = false;
                        }

                        soundPlayer.isPaused = true;
                        isStart = true;

                        if (!isPaused)
                            _internalTime += timePlusValue;
                    }
                    else if (internalTime > soundPlayer.length - MathUtility.epsilonFloatWithAccuracy)
                    {
                        if (soundPlayer.loop)
                            _internalTime = 0;

                        if (!isPaused)
                            _internalTime += timePlusValue;
                    }
                    else if (sync.Abs() >= audioDeltaTime * 4)
                    {
                        if (sync * speed.Sign() >= 0)
                        {
                            _internalTime = soundPlayer.time;

                            if (sync.Abs() >= 1)
                                FixBPM();
                        }
                    }
                    else if (!isPaused)
                        _internalTime += timePlusValue;

                    if (isStart && internalTime >= 0)
                    {
                        soundPlayer.isPaused = false;
                        isStart = false;
                    }
                }
                else
                    _internalTime += timePlusValue;

                SetCurrentBeat();

                {
                    bpm = bpmList.GetValue(currentBeat, out double beat);

                    if (lastBPMBeat != beat)
                    {
                        BPMChange(bpm, beat);
                        SetCurrentBeat();

                        lastBPMBeat = beat;
                    }
                }

                {
                    bpmDeltaTime = (float)(bpm / 60 * Kernel.deltaTime * speed.Abs());
                    bpmUnscaledDeltaTime = (float)(bpm / 60 * Kernel.unscaledDeltaTime * speed.Abs());

                    bpmFpsDeltaTime = bpmDeltaTime * VideoManager.Data.standardFPS;
                    bpmUnscaledFpsDeltaTime = bpmUnscaledDeltaTime * VideoManager.Data.standardFPS;
                }

                screenYukiMode = yukiModeList.GetValue(currentBeatScreen);

                if (tempCurrentBeat != (int)currentBeat && currentBeat >= 0)
                {
                    oneBeat?.Invoke();
                    if (screenYukiMode)
                        oneBeatYukiMode?.Invoke();

                    tempCurrentBeat = (int)currentBeat;
                }

                /*Debug.Log("time: " + time);
                Debug.Log("currentBeat: " + currentBeat);
                Debug.Log("bpm: " + bpm);
                Debug.Log("yukiMode: " + yukiMode);*/
            }
        }

        static void SetCurrentBeat()
        {
            double soundTime = (double)time - bpmOffsetTime;
            double bpmDivide60 = bpm / 60d;

            currentBeat = (soundTime * bpmDivide60) + bpmOffsetBeat;
            currentBeatSound = ((soundTime - SaveData.soundOffset) * bpmDivide60) + bpmOffsetBeat;
            currentBeatScreen = ((soundTime - SaveData.screenOffset) * bpmDivide60) + bpmOffsetBeat;

            currentBeatScreen1Beat = currentBeatScreen.Repeat(1);
        }

        static void BPMChange(double bpm, double offsetBeat)
        {
            BPMChangeCalculate(offsetBeat, bpmList, out double bpmOffsetBeat, out double bpmOffsetTime);

            RhythmManager.bpmOffsetBeat = bpmOffsetBeat;
            RhythmManager.bpmOffsetTime = bpmOffsetTime;

            RhythmManager.bpm = bpm;

            Debug.Log("BPM Changed");
            Debug.Log("BPM: " + bpm);
            Debug.Log("BPM Offset Beat: " + bpmOffsetBeat);
            Debug.Log("BPM Offset Time: " + bpmOffsetTime);
        }

        public static void BPMChangeCalculate(double changeBeat, BeatValuePairList<double> bpmList, out double bpmOffsetBeat, out double bpmOffsetTime)
        {
            changeBeat = changeBeat.Clamp(0);
            bpmOffsetBeat = changeBeat;

            bpmOffsetTime = 0;
            double tempBeat = 0;
            for (int i = 0; i < bpmList.Count; i++)
            {
                double beat = bpmList[i].beat.Clamp(0);
                if (bpmList[0].beat.Clamp(0) >= changeBeat)
                    break;

                double tempBPM;
                if (i - 1 < 0)
                    tempBPM = bpmList[0].value;
                else
                    tempBPM = bpmList[i - 1].value;
                bpmOffsetTime += (beat - tempBeat) * (60d / tempBPM);
                tempBeat = beat;

                if (beat >= changeBeat)
                    break;
            }
        }

        [WikiDescription("리듬 재생")]
        public static void Play(BeatValuePairList<double> bpmList, double offset, BeatValuePairList<bool> yukiModeList, ISoundPlayer soundPlayer = null, double startDelay = 0)
        {
            Debug.Log("Play");

            _internalTime = -(startDelay - offset).Clamp(0);

            currentBeat = double.MinValue;
            currentBeatScreen1Beat = 0;
            currentBeatScreen = double.MinValue;
            currentBeatSound = double.MinValue;

            bpmOffsetBeat = 0;
            bpmOffsetTime = 0;

            RhythmManager.bpmList = bpmList;
            RhythmManager.offset = offset;
            RhythmManager.startDelay = startDelay;
            RhythmManager.yukiModeList = yukiModeList;

            if (RhythmManager.soundPlayer != null)
            {
                RhythmManager.soundPlayer.timeChanged -= SoundPlayerTimeChange;
                RhythmManager.soundPlayer.looped -= SoundPlayerTimeChange;
            }

            RhythmManager.soundPlayer = soundPlayer;

            if (RhythmManager.soundPlayer != null)
            {
                RhythmManager.soundPlayer.timeChanged += SoundPlayerTimeChange;
                RhythmManager.soundPlayer.looped += SoundPlayerTimeChange;
            }

            isPlaying = true;
            isPaused = false;
            isStart = false;

            FixBPM();
        }

        [WikiDescription("리듬 정지")]
        public static void Stop()
        {
            Debug.Log("Stop");

            _internalTime = 0;

            currentBeat = double.MinValue;
            currentBeatScreen1Beat = 0;
            currentBeatScreen = double.MinValue;
            currentBeatSound = double.MinValue;

            bpmOffsetBeat = 0;
            bpmOffsetTime = 0;

            if (soundPlayer != null)
            {
                soundPlayer.timeChanged -= SoundPlayerTimeChange;
                soundPlayer.looped -= SoundPlayerTimeChange;
            }

            bpmList = null;
            offset = 0;
            yukiModeList = null;
            soundPlayer = null;

            isPlaying = false;
            isStart = false;
        }

        public static void Rewind(double value)
        {
            timeChangedEventLock = true;

            _internalTime -= value;
            if (soundPlayer != null)
                soundPlayer.time = _internalTime.Clamp(0, soundPlayer.length - 0.01f);

            timeChangedEventLock = false;
        }

        public static void SoundPlayerChange(ISoundPlayer soundPlayer)
        {
            if (!isPlaying)
                return;

            Debug.Log("Sound Player Changed");

            if (RhythmManager.soundPlayer != null)
            {
                RhythmManager.soundPlayer.timeChanged -= SoundPlayerTimeChange;
                RhythmManager.soundPlayer.looped -= SoundPlayerTimeChange;
            }

            RhythmManager.soundPlayer = soundPlayer;

            if (RhythmManager.soundPlayer != null)
            {
                soundPlayer.time = internalTime.Clamp(0, soundPlayer.length - 0.01f);

                RhythmManager.soundPlayer.timeChanged += SoundPlayerTimeChange;
                RhythmManager.soundPlayer.looped += SoundPlayerTimeChange;
            }
        }

        public static void MapChange(BeatValuePairList<double> bpmList, double offset, BeatValuePairList<bool> yukiModeList)
        {
            Debug.Log("Map Changed");

            if (RhythmManager.bpmList != bpmList || RhythmManager.bpmList.Count != bpmList.Count)
            {
                RhythmManager.bpmList = bpmList;
                FixBPM();
            }
            else
            {
                for (int i = 0; i < RhythmManager.bpmList.Count; i++)
                {
                    BeatValuePair<double> originalValue = RhythmManager.bpmList[i];
                    BeatValuePair<double> newValue = bpmList[i];

                    if (originalValue.beat != newValue.beat || originalValue.value != newValue.value || originalValue.disturbance != newValue.disturbance)
                    {
                        RhythmManager.bpmList = bpmList;
                        FixBPM();

                        break;
                    }
                }
            }

            if (RhythmManager.offset != offset)
            {
                RhythmManager.offset = offset;
                FixBPM();
            }

            RhythmManager.yukiModeList = yukiModeList;
        }

        static bool timeChangedEventLock = false;
        static void SoundPlayerTimeChange()
        {
            if (timeChangedEventLock)
                return;

            if (isPlaying && soundPlayer != null)
            {
                Debug.Log("Sound Player Time Changed");

                _internalTime = soundPlayer.time;
                FixBPM();
            }
        }

        static void FixBPM()
        {
            Debug.Log("Fix BPM");
            bpm = TimeUseBPMChangeCalulate(bpmList, time, offset, out double bpmOffsetBeat, out double bpmOffsetTime);

            RhythmManager.bpmOffsetBeat = bpmOffsetBeat;
            RhythmManager.bpmOffsetTime = bpmOffsetTime;

            Debug.Log("BPM Changed");
            Debug.Log("BPM: " + bpm);
            Debug.Log("BPM Offset Beat: " + bpmOffsetBeat);
            Debug.Log("BPM Offset Time: " + bpmOffsetTime);

            SetCurrentBeat();
        }

        public static double TimeUseBPMChangeCalulate(BeatValuePairList<double> bpmList, double time, double offset, out double bpmOffsetBeat, out double bpmOffsetTime)
        {
            double bpm = 0;
            bpmOffsetTime = 0;
            bpmOffsetBeat = 0;

            for (int i = 0; i < bpmList.Count; i++)
            {
                {
                    BeatValuePair<double> bpmPair = bpmList[i];
                    BPMChangeCalculate(bpmPair.beat, bpmList, out bpmOffsetBeat, out bpmOffsetTime);

                    bpm = bpmPair.value;
                }

                if (bpmOffsetTime > time - offset)
                {
                    if (i - 1 >= 0)
                    {
                        BeatValuePair<double> bpmPair = bpmList[i - 1];
                        BPMChangeCalculate(bpmPair.beat, bpmList, out bpmOffsetBeat, out bpmOffsetTime);

                        bpm = bpmPair.value;
                    }

                    break;
                }
            }

            return bpm;
        }

        public static double SecondToBeat(double second, double bpm) => second * (bpm / 60);
        public static double BeatToSecond(double beat, double bpm) => beat / (bpm / 60);
    }
}