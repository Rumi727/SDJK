using Cysharp.Threading.Tasks;
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

namespace SDJK.MainMenu
{
    public sealed class BGM : ObjectPoolingBase
    {
        public ISoundPlayer soundPlayer { get; private set; }

        public float volumePade { get; private set; } = 0;
        public bool padeOut { get; set; } = false;

        public bool isLoaded { get; private set; } = false;



        AudioClip audioClip;
        MapFile map;
        public async UniTaskVoid Refresh(MapPack lastMapPack, float lastTime)
        {
            map = MapManager.selectedMap;
            string path = PathTool.Combine(map.mapFilePathParent, map.info.songFile);
            if (ResourceManager.FileExtensionExists(path, out string fullPath, ResourceManager.audioExtension))
            {
                audioClip = await ResourceManager.GetAudio(fullPath, true, true);
                if (!Kernel.isPlaying || this == null)
                {
                    AudioDestroy();
                    return;
                }

                SoundMetaData soundMetaData = ResourceManager.CreateSoundMetaData(1, 1, 0, audioClip);
                SoundData<SoundMetaData> soundData = ResourceManager.CreateSoundData("", true, soundMetaData);

                soundPlayer = SoundManager.PlaySound(soundData, 0, true);
                soundPlayer.looped += Looped;

                if (MainMenu.currentScreenMode == ScreenMode.mapPackSelect || MainMenu.currentScreenMode == ScreenMode.mapSelect)
                {
                    if (lastMapPack != MapManager.selectedMapPack)
                        soundPlayer.time = (float)map.info.mainMenuStartTime;
                    else
                        soundPlayer.time = lastTime.Clamp(0, soundPlayer.length - 0.01f);
                }

                isLoaded = true;
            }
            else if (File.Exists(path + ".nbs"))
            {
                NBSFile nbsFile = NBSManager.ReadNBSFile(path + ".nbs");
                NBSMetaData nbsMetaData = ResourceManager.CreateNBSMetaData(1, 1, nbsFile);
                SoundData<NBSMetaData> soundData = ResourceManager.CreateSoundData("", true, nbsMetaData);

                soundPlayer = SoundManager.PlayNBS(soundData, 0, true);
                soundPlayer.looped += Looped;

                if (MainMenu.currentScreenMode == ScreenMode.mapPackSelect || MainMenu.currentScreenMode == ScreenMode.mapSelect)
                {
                    if (lastMapPack != MapManager.selectedMapPack)
                        soundPlayer.time = (float)map.info.mainMenuStartTime;
                    else
                        soundPlayer.time = lastTime.Clamp(0, soundPlayer.length - 0.01f);
                }

                isLoaded = true;
            }
            else
                Remove();
        }

        void Update()
        {
            if (!isLoaded || ResourceManager.isAudioReset)
                return;

            if (soundPlayer.isRemoved)
            {
                Remove();
                return;
            }

            if (padeOut)
            {
                soundPlayer.volume = soundPlayer.volume.MoveTowards(0, 0.05f * Kernel.fpsUnscaledDeltaTime);

                if (soundPlayer.volume <= 0)
                {
                    Remove();
                    return;
                }
            }
            else
            {
                volumePade = volumePade.MoveTowards(1, 0.05f * Kernel.fpsUnscaledDeltaTime);
                soundPlayer.volume = (float)map.globalEffect.volume.GetValue(RhythmManager.currentBeatSound) * volumePade;

                soundPlayer.pitch = (float)map.globalEffect.pitch.GetValue(RhythmManager.currentBeatSound);
                soundPlayer.tempo = (float)map.globalEffect.tempo.GetValue(RhythmManager.currentBeatSound);
            }
        }

        void Looped()
        {
            if (MainMenu.currentScreenMode == ScreenMode.mapPackSelect || MainMenu.currentScreenMode == ScreenMode.mapSelect)
                soundPlayer.time = (float)map.info.mainMenuStartTime;
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

                AudioDestroy();

                audioClip = null;
                return true;
            }

            return false;
        }

        void OnDestroy() => AudioDestroy();

        void AudioDestroy()
        {
            if (audioClip != null)
                Destroy(audioClip, 1);
        }
    }
}
