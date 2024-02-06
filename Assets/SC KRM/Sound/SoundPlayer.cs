using UnityEngine;
using SCKRM.Resource;
using K4.Threading;
using SCKRM.Threads;
using SCKRM.UI.Overlay;

namespace SCKRM.Sound
{
    [WikiDescription("사운드를 플레이하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/Sound/Sound Player", 0)]
    [RequireComponent(typeof(AudioSource)), RequireComponent(typeof(AudioLowPassFilter))]
    public sealed class SoundPlayer : SoundPlayerBase<SoundMetaData>
    {
        [System.NonSerialized] AudioSource _audioSource; public AudioSource audioSource => _audioSource = this.GetComponentFieldSave(_audioSource);
        [System.NonSerialized] AudioLowPassFilter _audioLowPassFilter; public AudioLowPassFilter audioLowPassFilter => _audioLowPassFilter = this.GetComponentFieldSave(_audioLowPassFilter);



        [WikiDescription("현재 시간 (Get is Thread-safe)")]
        public override double time
        {
            get => frequency != 0 ? (double)timeSamples / frequency : 0;
            set => timeSamples = (int)(value * frequency);
        }

        public int timeSamples
        {
            get => _timeSamples;
            set
            {
                if (_timeSamples != value)
                {
                    _timeSamples = value;
                    if (audioSource != null)
                        audioSource.timeSamples = value.Clamp(0, samples);

                    _timeChanged?.Invoke();
                }
            }
        }
        int _timeSamples = 0;

        [WikiDescription("현재 실제 시간")] public override double realTime { get => time / speed; set => time = value * speed; }

        [WikiDescription("곡의 길이")] public override double length => metaData != null ? metaData.length : 0;
        [WikiDescription("곡의 실제 길이")] public override double realLength => length / speed;

        [WikiDescription("루프 가능 여부")]
        public override bool loop
        {
            get => base.loop;
            set
            {
                if (base.loop != value)
                {
                    base.loop = value;
                    audioSource.loop = value;
                }
            }
        }



        [WikiDescription("현재 루프 중인지에 대한 여부")]
        public override bool isLooped { get; protected set; } = false;

        bool _isPaused = false;
        [WikiDescription("일시중지 여부")]
        public override bool isPaused
        {
            get => _isPaused;
            set
            {
                if (_isPaused != value && !isSpeedZero)
                {
                    if (value)
                        audioSource.Pause();
                    else
                        audioSource.UnPause();
                }

                _isPaused = value;
            }
        }



        double lastPitch = 1;
        [WikiDescription("피치")]
        public override float pitch
        {
            get => base.pitch;
            set
            {
                if (base.pitch != value)
                {
                    base.pitch = value;

                    {
                        float pitch = audioSource.pitch.Sign();
                        if (lastPitch != pitch)
                        {
                            lastTime = time - (pitch * 0.001);
                            lastPitch = pitch;
                        }
                    }

                    RefreshTempoAndPitch();
                }
            }
        }
        [WikiDescription("템포")]
        public override float tempo
        {
            get => base.tempo;
            set
            {
                if (base.tempo != value)
                {
                    base.tempo = value;
                    RefreshTempoAndPitch();
                }
            }
        }

        [WikiDescription("속도")]
        public override float speed
        {
            get => tempo;
            set => tempo = value;
        }
        [WikiDescription("실제 속도")]
        public override float realSpeed => metaData != null ? tempo * metaData.tempo : 0;

        [WikiDescription("볼륨")]
        public override float volume
        {
            get => base.volume;
            set
            {
                base.volume = value;
                RefreshVolume();
            }
        }


        [WikiDescription("최소 거리")]
        public override float minDistance
        {
            get => base.minDistance;
            set
            {
                base.minDistance = value;
                audioSource.minDistance = value;
            }
        }
        [WikiDescription("최대 거리")]
        public override float maxDistance
        {
            get => base.maxDistance;
            set
            {
                base.maxDistance = value;
                audioSource.maxDistance = value;
            }
        }

        [WikiDescription("스테레오")]
        public override float panStereo
        {
            get => base.panStereo;
            set
            {
                base.panStereo = value;
                audioSource.panStereo = value;
            }
        }



        [WikiDescription("공간")]
        public override bool spatial
        {
            get => base.spatial;
            set
            {
                base.spatial = value;

                if (value)
                    audioSource.spatialBlend = 1;
                else
                    audioSource.spatialBlend = 0;
            }
        }
        [WikiDescription("좌표")]
        public override Vector3 localPosition
        {
            get => base.localPosition;
            set
            {
                base.localPosition = value;
                transform.localPosition = value;
            }
        }

        public int frequency => metaData != null ? metaData.frequency : 0;
        public int channels => metaData != null ? metaData.channels : 0;

        public int samples => metaData != null ? metaData.samples : 0;

        public bool isSpeedZero { get; private set; }



        double lastTime = 0;
        double tempoAdjustmentTime = 0;
        void Update()
        {
            //시간 보정
            if (!isPaused)
            {
                float pitch = this.pitch * metaData.pitch;
                int value = (int)(frequency * Kernel.unscaledDeltaTime);
                _timeSamples += (int)(value * realSpeed * Kernel.gameSpeed);
                
                //템포
                if (audioSource.isPlaying && timeSamples >= 0 && timeSamples <= samples)
                {
                    int condition = (int)(2048);
                    double pitchDivideTempo = realSpeed * Kernel.gameSpeed / (pitch != 0 ? pitch : 1);
                    
                    tempoAdjustmentTime += value;
                    if (tempoAdjustmentTime >= condition)
                    {
                        float result = (float)((1 - pitchDivideTempo.Abs()) * pitch * condition * realSpeed.Sign());
                        
                        int tempTimeSamples = audioSource.timeSamples;
                        while (tempoAdjustmentTime >= condition)
                        {
                            if (result != 0)
                                tempTimeSamples -= (int)result;

                            tempoAdjustmentTime -= condition;
                        }

                        if (result != 0)
                            audioSource.timeSamples = tempTimeSamples.Clamp(0, samples - 1);

                        _timeSamples = tempTimeSamples;
                    }
                }
                else
                    tempoAdjustmentTime = 0;
            }
            else
                tempoAdjustmentTime = 0;

            if (UIOverlayManager.isOverlayShow)
                audioLowPassFilter.cutoffFrequency = 687.5f;
            else
                audioLowPassFilter.cutoffFrequency = 22000f;

            if (loop)
            {
                int loopStartIndex = metaData.loopStartIndex;
                bool isLooped = false;

                while (tempo >= 0 && timeSamples >= samples)
                {
                    _timeSamples -= samples - loopStartIndex;
                    isLooped = true;
                }

                while (tempo < 0 && timeSamples < loopStartIndex)
                {
                    _timeSamples += samples - loopStartIndex;
                    isLooped = true;
                }

                if (isLooped)
                {
                    if (loopStartIndex != 0 || timeSamples.Abs() > 2048)
                        audioSource.timeSamples = timeSamples;

                    _looped?.Invoke();
                }
            }

            if (!loop && ((timeSamples < 0 && tempo < 0) || (timeSamples > samples && tempo > 0)))
                Remove();
        }



        void OnAudioFilterRead(float[] data, int channels) => OnAudioFilterReadInvoke(ref data, channels);



        [WikiDescription("새로고침")]
        public override void Refresh()
        {
            if (!ThreadManager.isMainThread)
                K4UnityThreadDispatcher.Execute(refresh);
            else
                refresh();
        }

        void refresh()
        {
            {
                if (!InitialLoadManager.isInitialLoadEnd)
                {
                    Remove();
                    return;
                }
                else if (ResourceManager.isAudioReset)
                {
                    Remove();
                    return;
                }



                if (customSoundData == null)
                    soundData = ResourceManager.SearchSoundData(key, nameSpace);
                else
                    soundData = customSoundData;

                if (soundData == null)
                {
                    Remove();
                    return;
                }
                else if (soundData.sounds == null || soundData.sounds.Length <= 0)
                {
                    Remove();
                    return;
                }
            }

            if (nameSpace == null || nameSpace == "")
                name = ResourceManager.defaultNameSpace + ":" + key;
            else
                name = nameSpace + ":" + key;

            {
                metaData = soundData.sounds[Random.Range(0, soundData.sounds.Length)];
                audioSource.clip = metaData.audioClip;

                RefreshTempoAndPitch();
                RefreshVolume();
            }

            if (!SoundManager.soundList.Contains(this))
                SoundManager.soundList.Add(this);

            if (!audioSource.isPlaying)
            {
                if (audioSource.pitch < 0 && !metaData.stream && lastTime == 0)
                    time = length - 0.001;
                else
                    time = time.Min(length - 0.001);

                lastTime = time;
                audioSource.Play();

                if (isPaused || isSpeedZero)
                    audioSource.Pause();
            }
        }

        bool lastIsSpeedZero = false;
        public void RefreshTempoAndPitch()
        {
            if (soundData == null || metaData == null)
            {
                audioSource.pitch = 0;
                return;
            }

            isSpeedZero = realSpeed * Kernel.gameSpeed == 0;

            if (lastIsSpeedZero != isSpeedZero)
            {
                if (isSpeedZero)
                {
                    audioSource.pitch = 0;
                    audioSource.Pause();

                    lastIsSpeedZero = isSpeedZero;
                    return;
                }
                else
                {
                    if (!isPaused)
                        audioSource.UnPause();

                    lastIsSpeedZero = isSpeedZero;
                }
            }

            if (metaData.stream)
                base.pitch = base.pitch.Clamp(0);

            audioSource.pitch = base.pitch * metaData.pitch * realSpeed.Sign();
        }

        public void RefreshVolume()
        {
            if (soundData == null)
            {
                audioSource.volume = 0;
                return;
            }

            if (speed == 0)
                audioSource.volume = 0;
            else
            {
                if (soundData.isBGM)
                    audioSource.volume = base.volume * (SoundManager.SaveData.bgmVolume * 0.01f);
                else
                    audioSource.volume = base.volume * (SoundManager.SaveData.soundVolume * 0.01f);
            }
        }



        [WikiDescription("플레이어 삭제")]
        public override void Remove()
        {
            base.Remove();

            lastTime = 0;

            audioSource.clip = null;
            audioSource.outputAudioMixerGroup = null;

            audioSource.Stop();

            soundData = null;
            customSoundData = null;

            SoundManager.soundList.Remove(this);
        }
    }
}