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
        public ISoundPlayer soundPlayer { get; private set; }

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

                soundPlayer = SoundManager.PlaySound(soundData, 0, true);

                if (MainMenu.currentScreenMode == ScreenMode.mapSelect)
                    soundPlayer.time = (float)map.info.mainMenuStartTime;

                soundPlayer.looped += Looped;

                RhythmManager.Play(MapManager.selectedMapEffect.bpm, MapManager.selectedMapInfo.songOffset, MapManager.selectedMapEffect.dropPart, soundPlayer);

                isLoaded = true;
            }
            else if (File.Exists(path + ".nbs"))
            {
                NBSFile nbsFile = NBSManager.ReadNBSFile(path + ".nbs");
                NBSMetaData nbsMetaData = ResourceManager.CreateNBSMetaData(1, 1, nbsFile);
                SoundData<NBSMetaData> soundData = ResourceManager.CreateSoundData("", true, nbsMetaData);

                soundPlayer = SoundManager.PlayNBS(soundData, 0, true);

                if (MainMenu.currentScreenMode == ScreenMode.mapSelect)
                    soundPlayer.time = (float)map.info.mainMenuStartTime;

                soundPlayer.looped += Looped;

                RhythmManager.Play(MapManager.selectedMapEffect.bpm, MapManager.selectedMapInfo.songOffset, MapManager.selectedMapEffect.dropPart, soundPlayer);

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

            if (soundPlayer.isRemoved)
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

            soundPlayer.volume = (float)map.globalEffect.volume.GetValue() * volumePade;

            soundPlayer.pitch = (float)map.globalEffect.pitch.GetValue();
            soundPlayer.tempo = (float)map.globalEffect.tempo.GetValue();
        }

        void Looped()
        {
            if (MainMenu.currentScreenMode == ScreenMode.mapSelect)
            {
                SDJKMap map = MapManager.selectedMap;
                soundPlayer.time = (float)map.info.mainMenuStartTime;
            }
        }

        public override bool Remove()
        {
            if (base.Remove())
            {
                if (soundPlayer != null)
                {
                    soundPlayer.looped -= Looped;

                    if (!soundPlayer.isRemoved)
                        soundPlayer.Remove();
                }

                isLoaded = false;

                soundPlayer = null;

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
