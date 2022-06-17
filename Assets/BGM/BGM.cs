using SCKRM;
using SCKRM.NBS;
using SCKRM.Object;
using SCKRM.Resource;
using SCKRM.Rhythm;
using SCKRM.Sound;
using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SDJK
{
    public sealed class BGM : ObjectPooling
    {
        public IObjectPooling iObjectPooling { get; private set; }
        public ITime iTime { get; private set; }
        public ILoop iLoop { get; private set; }
        public IVolume iVolume { get; private set; }
        public ISpeed iSpeed { get; private set; }

        public float volumePade { get; private set; } = 0;
        public bool padeOut { get; set; } = false;

        public bool isLoaded { get; private set; } = false;



        AudioClip audioClip;
        public override async void OnCreate()
        {
            base.OnCreate();

            SDJKMap map = MapManager.selectedMap;
            string path = PathTool.Combine(map.mapFilePathParent, map.info.songFile);
            if (ResourceManager.FileExtensionExists(path, out _, ResourceManager.audioExtension))
            {
                audioClip = await ResourceManager.GetAudio(path, true);
                SoundMetaData soundMetaData = ResourceManager.CreateSoundMetaData(1, 1, 0, audioClip);
                SoundData<SoundMetaData> soundData = ResourceManager.CreateSoundData("", true, soundMetaData);

                SoundPlayer soundPlayer = SoundManager.PlaySound(soundData, 0, true);
                iObjectPooling = soundPlayer;
                iTime = soundPlayer;
                iLoop = soundPlayer;
                iVolume = soundPlayer;
                iSpeed = soundPlayer;

                if (MainMenu.currentScreenMode == ScreenMode.mapSelect)
                    iTime.time = (float)map.info.mainMenuStartTime;

                iLoop.looped += Looped;

                RhythmManager.Play(MapManager.selectedMapEffect, MapManager.selectedMapInfo, MapManager.selectedMapEffect, soundPlayer, soundPlayer);

                isLoaded = true;
            }
            else if (File.Exists(path + ".nbs"))
            {
                NBSFile nbsFile = NBSManager.ReadNBSFile(path + ".nbs");
                NBSMetaData nbsMetaData = ResourceManager.CreateNBSMetaData(1, 1, nbsFile);
                SoundData<NBSMetaData> soundData = ResourceManager.CreateSoundData("", true, nbsMetaData);

                NBSPlayer nbsPlayer = SoundManager.PlayNBS(soundData, 0, true);
                iObjectPooling = nbsPlayer;
                iTime = nbsPlayer;
                iLoop = nbsPlayer;
                iVolume = nbsPlayer;
                iSpeed = nbsPlayer;

                if (MainMenu.currentScreenMode == ScreenMode.mapSelect)
                    iTime.time = (float)map.info.mainMenuStartTime;

                iLoop.looped += Looped;

                RhythmManager.Play(MapManager.selectedMapEffect, MapManager.selectedMapInfo, MapManager.selectedMapEffect, nbsPlayer, nbsPlayer);

                isLoaded = true;
            }
            else
                Remove();
        }

        void Update()
        {
            if (!isLoaded || ResourceManager.isAudioReset)
                return;

            SDJKMap map = MapManager.selectedMap;

            if (iObjectPooling.isRemoved)
            {
                Remove();
                return;
            }
            else if (MapManager.isMapLoading)
            {
                Remove();
                return;
            }

            if (padeOut)
            {
                volumePade = volumePade.MoveTowards(0, 0.05f * Kernel.fpsUnscaledDeltaTime);

                if (volumePade <= 0)
                {
                    Remove();
                    return;
                }
            }
            else
                volumePade = volumePade.MoveTowards(1, 0.05f * Kernel.fpsUnscaledDeltaTime);

            iVolume.volume = (float)map.globalEffect.volume.GetValue() * volumePade;

            iSpeed.pitch = (float)map.globalEffect.pitch.GetValue();
            iSpeed.tempo = (float)map.globalEffect.tempo.GetValue();
        }

        void Looped()
        {
            if (MainMenu.currentScreenMode == ScreenMode.mapSelect)
            {
                SDJKMap map = MapManager.selectedMap;
                iTime.time = (float)map.info.mainMenuStartTime;
            }
        }

        public override bool Remove()
        {
            if (base.Remove())
            {
                if (iLoop != null)
                {
                    iLoop.looped -= Looped;

                    if (!iObjectPooling.isRemoved)
                        iObjectPooling.Remove();
                }

                isLoaded = false;

                iObjectPooling = null;
                iTime = null;
                iLoop = null;
                iVolume = null;
                iSpeed = null;

                volumePade = 0;
                padeOut = false;

                if (audioClip != null)
                    Destroy(audioClip);

                audioClip = null;
                return true;
            }

            return false;
        }
    }
}
