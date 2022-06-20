using SCKRM.Object;
using SCKRM.Resource;
using System;
using System.Threading;
using UnityEngine;

namespace SCKRM.Sound
{
    public delegate void OnAudioFilterReadAction(float[] data, int channels);

    public interface ISoundPlayerData<MetaData> where MetaData : SoundMetaDataParent
    {
        SoundData<MetaData> soundData { get; }
        SoundData<MetaData> customSoundData { get; set; }

        MetaData metaData { get; }
    }

    public interface ISoundPlayer : IRefreshable, IObjectPooling
    {
        string nameSpace { get; set; }
        string key { get; set; }



        float time { get; set; }
        float realTime { get; set; }

        float length { get; }
        float realLength { get; }



        event Action timeChanged;
        event Action looped;



        bool isLooped { get; }
        bool isPaused { get; set; }



        float pitch { get; set; }
        float tempo { get; set; }

        float speed { get; set; }
        float realSpeed { get; }




        float volume { get; set; }

        float minDistance { get; set; }
        float maxDistance { get; set; }

        float panStereo { get; set; }



        bool spatial { get; set; }
        Vector3 localPosition { get; set; }



        event OnAudioFilterReadAction onAudioFilterReadEvent;
    }

    public abstract class SoundPlayerParent<MetaData> : ObjectPooling, ISoundPlayer, ISoundPlayerData<MetaData> where MetaData : SoundMetaDataParent
    {
        SoundData<MetaData> ISoundPlayerData<MetaData>.soundData { get => soundData; }
        public SoundData<MetaData> soundData { get; protected set; }
        public SoundData<MetaData> customSoundData { get; set; }

        public MetaData metaData { get; protected set; }



        public string key { get; set; } = "";
        public string nameSpace { get; set; } = "";



        public abstract float time { get; set; }
        public abstract float realTime { get; set; }

        public abstract float length { get; }
        public abstract float realLength { get; }

        public virtual bool loop { get; set; } = false;



        protected Action _timeChanged;
        protected Action _looped;
        public event Action timeChanged { add => _timeChanged += value; remove => _timeChanged -= value; }
        public event Action looped { add => _looped += value; remove => _looped -= value; }



        bool ISoundPlayer.isLooped => isLooped;
        public abstract bool isLooped { get; protected set; }
        public abstract bool isPaused { get; set; }



        public virtual float pitch { get; set; } = 1;
        public virtual float tempo { get; set; } = 1;

        public abstract float speed { get; set; }
        public abstract float realSpeed { get; }



        public virtual float volume { get; set; } = 1;

        public virtual float minDistance { get; set; } = 0;
        public virtual float maxDistance { get; set; } = 16;

        public virtual float panStereo { get; set; } = 0;



        public virtual bool spatial { get; set; } = false;
        public virtual Vector3 localPosition { get; set; } = Vector3.zero;



        int onAudioFilterReadEventLock = 0;
        event OnAudioFilterReadAction _onAudioFilterReadEvent;

        /// <summary>
        /// Thread-Safe (onAudioFilterReadEvent += () => { onAudioFilterReadEvent += () => { }; }; Do not add more methods to this event from inside this event method like this. This causes deadlock)
        /// </summary>
        public event OnAudioFilterReadAction onAudioFilterReadEvent
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

        protected void OnAudioFilterReadInvoke(float[] data, int channels)
        {
            while (Interlocked.CompareExchange(ref onAudioFilterReadEventLock, 1, 0) != 0)
                Thread.Sleep(1);

            try
            {
                _onAudioFilterReadEvent?.Invoke(data, channels);
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



        public abstract void Refresh();



        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            key = "";
            nameSpace = "";


            time = 0;
            realTime = 0;

            loop = false;


            _looped = null;
            _timeChanged = null;

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

            _onAudioFilterReadEvent = null;
            return true;
        }
    }
}
