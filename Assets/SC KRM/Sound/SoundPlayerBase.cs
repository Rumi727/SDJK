using SCKRM.Object;
using SCKRM.Resource;
using System;
using System.Threading;
using UnityEngine;

namespace SCKRM.Sound
{
    public delegate void OnAudioFilterReadAction(ref float[] data, int channels);

    public interface ISoundPlayerData<MetaData> where MetaData : SoundMetaDataBase
    {
        SoundData<MetaData> soundData { get; }
        SoundData<MetaData> customSoundData { get; set; }

        MetaData metaData { get; }
    }

    [WikiDescription("사운드 플레이어를 구현하기 위한 인터페이스")]
    public interface ISoundPlayer : IRefreshable, IObjectPooling
    {
        [WikiDescription("재생할 네임스페이스")] string nameSpace { get; set; }
        [WikiDescription("재생할 키")] string key { get; set; }



        [WikiDescription("시간")] double time { get; set; }
        [WikiDescription("실제 시간")] double realTime { get; set; }

        [WikiDescription("곡의 길이")] double length { get; }
        [WikiDescription("곡의 실제 길이")] double realLength { get; }

        [WikiDescription("루프 가능 여부")] bool loop { get; set; }



        event Action timeChanged;
        event Action looped;



        [WikiDescription("이 프레임에서 루프가 됬는지 여부")] bool isLooped { get; }
        [WikiDescription("일시중지 여부")] bool isPaused { get; set; }



        [WikiDescription("피치")] float pitch { get; set; }
        [WikiDescription("템포")] float tempo { get; set; }

        [WikiDescription("속도")] float speed { get; set; }
        [WikiDescription("실제 속도")] float realSpeed { get; }




        [WikiDescription("볼륨")] float volume { get; set; }

        [WikiDescription("최소 거리")] float minDistance { get; set; }
        [WikiDescription("최대 거리")] float maxDistance { get; set; }

        [WikiDescription("스테레오")] float panStereo { get; set; }



        [WikiDescription("공간")] bool spatial { get; set; }
        [WikiDescription("좌표")] Vector3 localPosition { get; set; }



        event OnAudioFilterReadAction onAudioFilterReadEvent;
    }

    [WikiDescription("내장 사운드 플레이어")]
    public abstract class SoundPlayerBase<MetaData> : ObjectPoolingBase, ISoundPlayer, ISoundPlayerData<MetaData> where MetaData : SoundMetaDataBase
    {
        SoundData<MetaData> ISoundPlayerData<MetaData>.soundData { get => soundData; }
        public SoundData<MetaData> soundData { get; protected set; }
        public SoundData<MetaData> customSoundData { get; set; }

        public MetaData metaData { get; protected set; }



        [WikiDescription("재생할 사운드 키")] public string key { get; set; } = "";
        [WikiDescription("재생할 사운드 네임스페이스")] public string nameSpace { get; set; } = "";



        [WikiDescription("시간")] public abstract double time { get; set; }
        [WikiDescription("실제 시간")] public abstract double realTime { get; set; }

        [WikiDescription("곡의 길이")] public abstract double length { get; }
        [WikiDescription("곡의 실제 길이")] public abstract double realLength { get; }

        [WikiDescription("루프 가능 여부")] public virtual bool loop { get; set; } = false;



        protected Action _timeChanged;
        protected Action _looped;
        public event Action timeChanged { add => _timeChanged += value; remove => _timeChanged -= value; }
        public event Action looped { add => _looped += value; remove => _looped -= value; }



        bool ISoundPlayer.isLooped => isLooped;
        [WikiDescription("이 프레임에서 루프가 됬는지 여부")] public abstract bool isLooped { get; protected set; }
        [WikiDescription("일시중지 여부")] public abstract bool isPaused { get; set; }



        [WikiDescription("피치")] public virtual float pitch { get; set; } = 1;
        [WikiDescription("템포")] public virtual float tempo { get; set; } = 1;

        [WikiDescription("속도")] public abstract float speed { get; set; }
        [WikiDescription("실제 속도")] public abstract float realSpeed { get; }



        [WikiDescription("볼륨")] public virtual float volume { get; set; } = 1;

        [WikiDescription("최소 거리")] public virtual float minDistance { get; set; } = 0;
        [WikiDescription("최대 거리")] public virtual float maxDistance { get; set; } = 16;

        [WikiDescription("스테레오")] public virtual float panStereo { get; set; } = 0;



        [WikiDescription("공간")] public virtual bool spatial { get; set; } = false;
        [WikiDescription("좌표")] public virtual Vector3 localPosition { get; set; } = Vector3.zero;



        int onAudioFilterReadEventLock = 0;
        event OnAudioFilterReadAction _onAudioFilterReadEvent;

        /// <summary>
        /// Thread-Safe (onAudioFilterReadEvent += () => { onAudioFilterReadEvent += () => { }; }; Do not add more methods to this event from inside this event method like this. This causes deadlock)
        /// </summary>
        public virtual event OnAudioFilterReadAction onAudioFilterReadEvent
        {
            add
            {
                while (Interlocked.CompareExchange(ref onAudioFilterReadEventLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _onAudioFilterReadEvent += value;

                Interlocked.Decrement(ref onAudioFilterReadEventLock);
            }
            remove
            {
                while (Interlocked.CompareExchange(ref onAudioFilterReadEventLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _onAudioFilterReadEvent -= value;

                Interlocked.Decrement(ref onAudioFilterReadEventLock);
            }
        }

        protected virtual void OnAudioFilterReadInvoke(ref float[] data, int channels)
        {
            while (Interlocked.CompareExchange(ref onAudioFilterReadEventLock, 1, 0) != 0)
                Thread.Sleep(1);

            try
            {
                _onAudioFilterReadEvent?.Invoke(ref data, channels);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                Interlocked.Decrement(ref onAudioFilterReadEventLock);
            }
        }



        [WikiDescription("새로고침")]
        public abstract void Refresh();



        [WikiDescription("플레이어 삭제")]
        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            _looped = null;
            _timeChanged = null;
            _onAudioFilterReadEvent = null;

            key = "";
            nameSpace = "";


            time = 0;
            realTime = 0;

            loop = false;

            isLooped = false;
            isPaused = false;


            pitch = 1;
            tempo = 1;

            speed = 1;


            volume = 1;

            minDistance = 0;
            maxDistance = 16;

            panStereo = 0;

            spatial = false;
            localPosition = Vector3.zero;

            return true;
        }

        public virtual void OnDestroy()
        {
            _looped = null;
            _timeChanged = null;
            _onAudioFilterReadEvent = null;
        }
    }
}
