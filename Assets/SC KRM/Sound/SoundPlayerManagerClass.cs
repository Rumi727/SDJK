using SCKRM.Object;
using SCKRM.Resource;
using System;
using UnityEngine;

namespace SCKRM.Sound
{
    public interface INameSpaceKey
    {
        string nameSpace { get; set; }
        string key { get; set; }
    }

    public interface ITime
    {
        float time { get; set; }
        float realTime { get; set; }

        event Action timeChanged;



        bool isPaused { get; set; }
    }

    public interface ILength
    {
        float time { get; set; }
        float realTime { get; set; }

        event Action timeChanged;
    }

    public interface ISpeed
    {
        float pitch { get; set; }
        float tempo { get; set; }

        float speed { get; set; }
    }

    public interface ILoop
    {
        event Action looped;
        bool isLooped { get; }
    }

    public interface IVolume
    {
        float volume { get; set; }
    }

    public interface ISpatial
    {
        float minDistance { get; set; }
        float maxDistance { get; set; }

        float panStereo { get; set; }



        bool spatial { get; set; }
        Vector3 localPosition { get; set; }
    }

    public interface ISoundPlayerRefresh : IRefreshable
    {

    }

    public interface ISoundPlayer<MetaData> : IObjectPooling, INameSpaceKey, ITime, ILength, ISpeed, ILoop, IVolume, ISpatial, ISoundPlayerRefresh where MetaData : SoundMetaDataParent
    {
        public SoundData<MetaData> soundData { get; }
        public SoundData<MetaData> customSoundData { get; set; }

        public MetaData metaData { get; }
    }

    public abstract class SoundPlayerParent<MetaData> : ObjectPooling, ISoundPlayer<MetaData> where MetaData : SoundMetaDataParent
    {
        SoundData<MetaData> ISoundPlayer<MetaData>.soundData { get => soundData; }
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



        bool ILoop.isLooped { get => isLooped; }
        public abstract bool isLooped { get; protected set; }
        public abstract bool isPaused { get; set; }



        public virtual float pitch { get; set; } = 1;
        public virtual float tempo { get; set; } = 1;

        public abstract float speed { get; set; }



        public virtual float volume { get; set; } = 1;

        public virtual float minDistance { get; set; } = 0;
        public virtual float maxDistance { get; set; } = 16;

        public virtual float panStereo { get; set; } = 0;



        public virtual bool spatial { get; set; } = false;
        public virtual Vector3 localPosition { get; set; } = Vector3.zero;



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

            return true;
        }
    }
}
