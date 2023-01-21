using K4.Threading;
using SCKRM.Resource;
using SCKRM.Sound;
using SCKRM.Threads;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SCKRM.NBS
{
    [WikiDescription("NBS를 플레이하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/NBS/NBS Player")]
    public sealed class NBSPlayer : SoundPlayerBase<NBSMetaData>
    {
        [WikiDescription("NBS 파일을 가져옵니다")] public NBSFile nbsFile => metaData.nbsFile;



        float tickTimer = 0.05f;

        int _index = 0;
        [WikiDescription("현재 인덱스")]
        public int index
        {
            get => _index;
            set
            {
                if (metaData == null || nbsFile == null || nbsFile.nbsNotes == null)
                {
                    tickTimer = 0;
                    _tick = 0;
                    _index = 0;

                    return;
                }

                value = value.Clamp(0, nbsFile.nbsNotes.Count - 1);

                tickTimer = 0;
                _index = value;
                _tick = nbsFile.nbsNotes[value].delayTick;
            }
        }

        int _tick;
        [WikiDescription("현재 틱")]
        public int tick
        {
            get => _tick;
            set
            {
                if (metaData == null || nbsFile == null || nbsFile.nbsNotes == null)
                {
                    tickTimer = 0;
                    _tick = 0;
                    _index = 0;

                    return;
                }

                value = value.Clamp(0, tickLength);

                int lastTick = _tick;

                tickTimer = 0;
                _tick = value;
                _index = nbsFile.nbsNotes.Select((d, i) => new { d.delayTick, index = i }).MinBy(x => (x.delayTick - value).Abs()).index;

                if (lastTick != value)
                    _timeChanged?.Invoke();
            }
        }

        [WikiDescription("현재 시간")]
        public override float time
        {
            get
            {
                if (nbsFile == null)
                    return 0;

                return ((_tick * 0.05f) + tickTimer - 0.05f) / (nbsFile.tickTempo * 0.0005f);
            }
            set
            {
                if (nbsFile != null)
                {
                    float value20 = (value * (nbsFile.tickTempo * 0.0005f)) * 20;
                    float lastTime = time;

                    tick = (int)value20;
                    tickTimer = ((value20 - (int)value20) * 0.05f) + 0.05f;

                    if (lastTime != tick)
                        _timeChanged?.Invoke();
                }
            }
        }
        [WikiDescription("실제 현재 시간")] public override float realTime { get => time / tempo; set => time = value * tempo; }

        [WikiDescription("곡의 길이")]
        public override float length
        {
            get
            {
                if (nbsFile == null)
                    return 0;

                return tickLength / (nbsFile.tickTempo * 0.01f);
            }
        }

        [WikiDescription("곡의 틱 길이")]
        public int tickLength
        {
            get
            {
                if (nbsFile == null)
                    return 0;

                return nbsFile.songLength;
            }
        }

        [WikiDescription("곡의 실제 길이")] public override float realLength => length / tempo;

        [WikiDescription("곡의 실제 틱 길이")] public float realTickLength => tickLength / tempo;

        [WikiDescription("루프 여부")] public override bool isLooped { get; protected set; } = false;
        [WikiDescription("일시정지 여부")] public override bool isPaused { get; set; } = false;

        [WikiDescription("속도")] public override float speed { get => tempo; set => tempo = value; }
        [WikiDescription("실제 속도")]
        public override float realSpeed
        {
            get
            {
                if (metaData == null)
                    return 0;

                return tempo * metaData.tempo;
            }
        }

        /// <summary>
        /// Although this event is called on the main thread due to the nature of the implementation, it is thread safe.
        /// But it's read-only so you can't insert DPS chains and it converts to mono.
        /// </summary>
        [WikiDescription(
@"Although this event is called on the main thread due to the nature of the implementation, it is thread safe.
But it's read-only so you can't insert DPS chains and it converts to mono."
)]
        public override event OnAudioFilterReadAction onAudioFilterReadEvent { add => base.onAudioFilterReadEvent += value; remove => base.onAudioFilterReadEvent -= value; }

        void Update()
        {
            transform.localPosition = localPosition;

            if (!isPaused && realSpeed != 0)
            {
                if (realSpeed < 0)
                {
                    tickTimer -= Kernel.deltaTime * (nbsFile.tickTempo * 0.0005f) * realSpeed.Abs();

                    bool soundPlayerAllow = false;
                    while (tickTimer <= 0)
                    {
                        _tick--;
                        tickTimer += 0.05f;

                        soundPlayerAllow = true;
                    }

                    if (soundPlayerAllow)
                        SoundPlay();
                }
                else
                {
                    tickTimer += Kernel.deltaTime * (nbsFile.tickTempo * 0.0005f) * realSpeed.Abs();

                    bool soundPlayerAllow = false;
                    while (tickTimer >= 0.05f)
                    {
                        _tick++;
                        tickTimer -= 0.05f;

                        soundPlayerAllow = true;
                    }

                    if (soundPlayerAllow)
                        SoundPlay();
                }

                GetAudioDataToMonoAndInvoke();
            }
        }



        [WikiDescription("새로고침")]
        public override void Refresh()
        {
            if (!ThreadManager.isMainThread)
                K4UnityThreadDispatcher.Execute(refresh);
            else
                refresh();
        }

        bool allLayerLock;
        void refresh()
        {
            {
                if (!InitialLoadManager.isInitialLoadEnd)
                {
                    Remove();
                    return;
                }



                if (customSoundData == null)
                    soundData = ResourceManager.SearchNBSData(key, nameSpace);
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

                metaData = soundData.sounds[Random.Range(0, soundData.sounds.Length)];
            }



            if (nameSpace == null || nameSpace == "")
                name = ResourceManager.defaultNameSpace + ":" + key;
            else
                name = nameSpace + ":" + key;



            if (!SoundManager.nbsList.Contains(this))
                SoundManager.nbsList.Add(this);

            allLayerLock = nbsFile.nbsLayers.Any((b) => b.layerLock == 2);

            if (tempo < 0 && tick == 0)
            {
                tickTimer = 0;
                _tick = nbsFile.nbsNotes[nbsFile.nbsNotes.Count - 1].delayTick;
                _index = nbsFile.nbsNotes.Count - 2;
            }
            else if (tick > tickLength)
            {
                if (loop)
                {
                    if (tempo > 0)
                        tick = nbsFile.loopStartTick;
                    else
                    {
                        tickTimer = 0;
                        _tick = nbsFile.nbsNotes[nbsFile.nbsNotes.Count - 1].delayTick;
                        _index = nbsFile.nbsNotes.Count - 2;
                    }
                }
                else
                    Remove();
            }
            else
                time = time;
        }



        void SoundPlay()
        {
            if (ResourceManager.isAudioReset)
                return;

            if (index >= 0)
            {
                NBSNote nbsNote = nbsFile.nbsNotes[index];
                if ((tempo < 0 && nbsNote.delayTick >= tick) || (tempo >= 0 && nbsNote.delayTick <= tick))
                {
                    for (int i = 0; i < nbsNote.nbsNoteMetaDatas.Count; i++)
                    {
                        NBSNoteMetaData nbsNoteMetaData = nbsNote.nbsNoteMetaDatas[i];
                        NBSLayer nbsLayer = nbsFile.nbsLayers[nbsNoteMetaData.layerIndex];
                        if (nbsLayer.layerLock != 0 && !allLayerLock)
                            continue;
                        else if (nbsLayer.layerLock != 2 && allLayerLock)
                            continue;

                        float pitch = Mathf.Pow(2, (nbsNoteMetaData.key - 45f) * 0.0833333333f) * Mathf.Pow(1.059463f, nbsNoteMetaData.pitch * 0.01f);
                        float volume = nbsNoteMetaData.velocity * 0.01f * (nbsLayer.layerVolume * 0.01f);
                        float panStereo = (nbsNoteMetaData.panning - 100) * 0.01f * ((nbsLayer.layerStereo - 100) * 0.01f);

                        string blockType = "block.note_block.";
                        if (nbsNoteMetaData.instrument == 0)
                            blockType += "harp";
                        else if (nbsNoteMetaData.instrument == 1)
                            blockType += "bass";
                        else if (nbsNoteMetaData.instrument == 2)
                            blockType += "bassdrum";
                        else if (nbsNoteMetaData.instrument == 3)
                            blockType += "snare";
                        else if (nbsNoteMetaData.instrument == 4)
                            blockType += "hat";
                        else if (nbsNoteMetaData.instrument == 5)
                            blockType += "guitar";
                        else if (nbsNoteMetaData.instrument == 6)
                            blockType += "flute";
                        else if (nbsNoteMetaData.instrument == 7)
                            blockType += "bell";
                        else if (nbsNoteMetaData.instrument == 8)
                            blockType += "chime";
                        else if (nbsNoteMetaData.instrument == 9)
                            blockType += "xylophone";
                        else if (nbsNoteMetaData.instrument == 10)
                            blockType += "iron_xylophone";
                        else if (nbsNoteMetaData.instrument == 11)
                            blockType += "cow_bell";
                        else if (nbsNoteMetaData.instrument == 12)
                            blockType += "didgeridoo";
                        else if (nbsNoteMetaData.instrument == 13)
                            blockType += "bit";
                        else if (nbsNoteMetaData.instrument == 14)
                            blockType += "banjo";
                        else if (nbsNoteMetaData.instrument == 15)
                            blockType += "pling";

                        SoundPlayer soundPlayer;
                        if (spatial)
                            soundPlayer = SoundManager.PlaySound(blockType, "minecraft", volume * this.volume, false, pitch * this.pitch * metaData.pitch / Kernel.gameSpeed, 1, panStereo + this.panStereo, minDistance, maxDistance, transform);
                        else
                            soundPlayer = SoundManager.PlaySound(blockType, "minecraft", volume * this.volume, false, pitch * this.pitch * metaData.pitch / Kernel.gameSpeed, 1, panStereo + this.panStereo);

                        allPlayingSounds.Add(soundPlayer);
                    }

                    _index++;
                }
            }
            else
                _index = 0;

            SetIndex();
            Loop();
        }

        void SetIndex()
        {
            if (realSpeed < 0)
            {
                while (index > 0 && index < nbsFile.nbsNotes.Count && nbsFile.nbsNotes[index].delayTick >= tick)
                    _index--;

                if (index == 0)
                    _index--;
            }
            else
            {
                while (index > 0 && index < nbsFile.nbsNotes.Count && nbsFile.nbsNotes[index].delayTick < tick)
                    _index++;
            }
        }

        void Loop()
        {
            isLooped = false;
            if (tick < nbsFile.loopStartTick || index >= nbsFile.nbsNotes.Count)
            {
                if (loop)
                {
                    if (tempo < 0)
                    {
                        tickTimer = 0;
                        _tick = nbsFile.nbsNotes[nbsFile.nbsNotes.Count - 1].delayTick;
                        _index = nbsFile.nbsNotes.Count - 2;
                    }
                    else
                    {
                        tickTimer = 0;

                        if (nbsFile.loopStartTick > 0)
                            _tick = nbsFile.loopStartTick;
                        else
                        {
                            _tick = 0;
                            _index = 0;
                        }
                    }

                    isLooped = true;
                    _looped?.Invoke();
                }
                else
                    Remove();
            }
        }

        List<SoundPlayer> allPlayingSounds = new List<SoundPlayer>();
        float[] audioDatas = new float[0];
        float[] tempDatas = new float[0];
        void GetAudioDataToMonoAndInvoke()
        {
            AudioSettings.GetDSPBufferSize(out int bufferLength, out _);

            if (tempDatas.Length != bufferLength)
                tempDatas = new float[bufferLength];
            if (audioDatas.Length != bufferLength)
                audioDatas = new float[bufferLength];

            for (int i = 0; i < audioDatas.Length; i++)
                audioDatas[i] = 0;

            for (int i = 0; i < allPlayingSounds.Count; i++)
            {
                SoundPlayer soundPlayer = allPlayingSounds[i];
                if (soundPlayer == null || soundPlayer.isRemoved)
                {
                    allPlayingSounds.RemoveAt(i);
                    i--;

                    continue;
                }

                for (int j = 0; j < soundPlayer.metaData.audioClip.channels; j++)
                {
                    soundPlayer.audioSource.GetOutputData(tempDatas, j + 1);

                    for (int k = 0; k < tempDatas.Length; k++)
                        audioDatas[k] += tempDatas[k] / soundPlayer.metaData.audioClip.channels;
                }
            }

            OnAudioFilterReadInvoke(audioDatas, 1);
        }

        [WikiDescription("플레이어 삭제")]
        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            soundData = null;
            customSoundData = null;

            _index = 0;
            _tick = 0;
            tickTimer = 0.05f;

            SoundManager.nbsList.Remove(this);
            SoundPlayer[] soundObjects = GetComponentsInChildren<SoundPlayer>();
            for (int i = 0; i < soundObjects.Length; i++)
                soundObjects[i].Remove();

            return true;
        }
    }
}