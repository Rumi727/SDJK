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
    public sealed class SoundPlayer : SoundPlayerParent<SoundMetaData>
    {
        [System.NonSerialized] AudioSource _audioSource; public AudioSource audioSource => _audioSource = this.GetComponentFieldSave(_audioSource);
        [System.NonSerialized] AudioLowPassFilter _audioLowPassFilter; public AudioLowPassFilter audioLowPassFilter => _audioLowPassFilter = this.GetComponentFieldSave(_audioLowPassFilter);



        [WikiDescription("현재 시간")]
        public override float time
        {
            get => audioSource.time;
            set
            {
                float lastTime = audioSource.time;

                if (lastTime != value)
                {
                    audioSource.time = value;
                    tempTime = audioSource.time;

                    _timeChanged?.Invoke();
                }
            }
        }
        [WikiDescription("현재 실제 시간")] public override float realTime { get => time / speed; set => time = value * speed; }

        [WikiDescription("곡의 길이")] public override float length => (float)(audioSource.clip != null ? audioSource.clip.length : 0);
        [WikiDescription("곡의 실제 길이")] public override float realLength => length / speed;

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
                if (_isPaused != value)
                {
                    if (value)
                        audioSource.Pause();
                    else
                        audioSource.UnPause();
                }

                _isPaused = value;
            }
        }



        [WikiDescription("피치")]
        public override float pitch
        {
            get => base.pitch;
            set
            {
                if (base.pitch != value)
                {
                    base.pitch = value;

                    RefreshTempoAndPitch();
                    RefreshVolume();
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
                    RefreshVolume();
                }
            }
        }

        [WikiDescription("속도")]
        public override float speed
        {
            get
            {
                if (soundData != null && soundData.isBGM && SoundManager.Data.useTempo)
                    return tempo;
                else
                    return pitch;
            }
            set
            {
                if (soundData != null && soundData.isBGM && SoundManager.Data.useTempo)
                    tempo = value;
                else
                    pitch = value;
            }
        }
        [WikiDescription("실제 속도")]
        public override float realSpeed
        {
            get
            {
                if (metaData == null)
                    return 0;

                if (soundData != null && soundData.isBGM && SoundManager.Data.useTempo)
                    return tempo * metaData.tempo;
                else
                    return pitch * metaData.pitch;
            }
        }

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



        float tempTime = 0;
        void Update()
        {
            if (UIOverlayManager.isOverlayShow)
                audioLowPassFilter.cutoffFrequency = 687.5f;
            else
                audioLowPassFilter.cutoffFrequency = 22000f;

            if (audioSource.loop)
            {
                isLooped = false;
                if (audioSource.pitch < 0)
                {
                    if (audioSource.time < metaData.loopStartTime)
                        audioSource.time = length - 0.001f;

                    if (audioSource.time > tempTime)
                    {
                        isLooped = true;
                        _looped?.Invoke();
                    }
                }
                else
                {
                    if (audioSource.time < tempTime)
                    {
                        if (metaData.loopStartTime > 0)
                            audioSource.time = metaData.loopStartTime;

                        isLooped = true;
                        _looped?.Invoke();
                    }
                }

                tempTime = audioSource.time;
            }

            if (!isPaused && !audioSource.isPlaying && !ResourceManager.isAudioReset)
                Remove();
        }



        void OnAudioFilterRead(float[] data, int channels) => OnAudioFilterReadInvoke(data, channels);



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

            float backTime = time;
            {
                metaData = soundData.sounds[Random.Range(0, soundData.sounds.Length)];
                audioSource.clip = metaData.audioClip;

                if (soundData.isBGM && SoundManager.Data.useTempo)
                    audioSource.outputAudioMixerGroup = SoundManager.instance.audioMixerGroup;
                else
                    audioSource.outputAudioMixerGroup = null;

                RefreshTempoAndPitch();
                RefreshVolume();

                audioSource.Play();
            }

            if (!SoundManager.soundList.Contains(this))
                SoundManager.soundList.Add(this);

            {
                if (isPaused)
                    isPaused = false;

                if (audioSource.pitch < 0 && !metaData.stream && tempTime == 0)
                    audioSource.time = length - 0.001f;
                else
                    audioSource.time = Mathf.Min(backTime, length - 0.001f);

                tempTime = audioSource.time;
            }
        }

        public void RefreshTempoAndPitch()
        {
            if (soundData == null || metaData == null)
            {
                audioSource.pitch = 0;
                return;
            }

            if (soundData == null || soundData.isBGM && SoundManager.Data.useTempo)
            {
                if (metaData.stream)
                    base.tempo = base.tempo.Clamp(0);

                float allPitch = base.pitch * metaData.pitch;
                float allTempo = base.tempo * metaData.tempo;

                //base.pitch = allPitch.Clamp(allTempo.Abs() * 0.5f, allTempo.Abs() * 2f) / metaData.pitch;

                allTempo *= Kernel.gameSpeed;
                audioSource.pitch = allTempo;
                audioSource.outputAudioMixerGroup.audioMixer.SetFloat("pitch", 1f / allTempo.Abs() * allPitch.Clamp(allTempo.Abs() * 0.5f, allTempo.Abs() * 2f));
            }
            else
            {
                if (metaData.stream)
                    base.pitch = base.pitch.Clamp(0);

                audioSource.pitch = base.pitch * metaData.pitch * Kernel.gameSpeed;
            }
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
        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            tempTime = 0;

            audioSource.clip = null;
            audioSource.outputAudioMixerGroup = null;

            audioSource.time = 0;
            audioSource.Stop();

            soundData = null;
            customSoundData = null;

            SoundManager.soundList.Remove(this);

            return true;
        }
    }
}