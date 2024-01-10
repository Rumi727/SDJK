using Cysharp.Threading.Tasks;
using SCKRM.NBS;
using SCKRM.Resource;
using SCKRM;
using SCKRM.Rhythm;
using SCKRM.Sound;
using SDJK.Map;
using System.Media;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace SDJK.Effect
{
    public sealed class HitsoundEffect : Effect
    {
        double lastValue = -1;
        protected override void RealUpdate()
        {
            int count = GetHitsoundPlayCount(map, RhythmManager.currentBeatSound, ref lastValue);
            for (int i = 0; i < count; i++)
                DefaultHitsoundPlay();
        }

        public static void DefaultHitsoundPlay(float volume = 0.5f, float pitch = 0.95f) => SoundManager.PlaySound("hitsound.normal", "sdjk", volume, false, pitch);



        public static void CustomHitsoundPlay(Dictionary<string, HitsoundInfo> loadedHitsounds, MapFile map, UnityEngine.Object instance, HitsoundFile hitsound) => customHitsoundPlay(loadedHitsounds, map, instance, hitsound).Forget();

        static async UniTaskVoid customHitsoundPlay(Dictionary<string, HitsoundInfo> loadedHitsounds, MapFile map, UnityEngine.Object instance, HitsoundFile hitsound)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hitsound.path))
                    DefaultHitsoundPlay();
                else if (loadedHitsounds.TryGetValue(hitsound.path, out HitsoundInfo hitsoundInfo))
                {
                    if (!hitsoundInfo.defaultAudio)
                    {
                        if (hitsoundInfo.soundData != null)
                            SoundManager.PlaySound(hitsoundInfo.soundData, hitsound.volume, false, hitsound.pitch, hitsound.tempo);
                        else if (hitsoundInfo.nbsData != null)
                            SoundManager.PlayNBS(hitsoundInfo.nbsData, hitsound.volume, false, hitsound.pitch, hitsound.tempo);
                        else
                            DefaultHitsoundPlay();
                    }
                    else
                    {
                        string key = "hitsound." + hitsound.path;
                        if (ResourceManager.SearchSoundData(key, "sdjk") != null)
                            SoundManager.PlaySound(key, "sdjk", hitsound.volume, false, hitsound.pitch, hitsound.tempo);
                        else
                            DefaultHitsoundPlay();
                    }
                }
                else
                {
                    string path = PathUtility.Combine(map.mapFilePathParent, hitsound.path);
                    if (ResourceManager.FileExtensionExists(path, out string fullPath, ResourceManager.audioExtension))
                    {
                        try
                        {
                            AudioClip audioClip2 = await ResourceManager.GetAudio(fullPath, true);
                            if (!Kernel.isPlaying || instance == null)
                            {
                                DestroyImmediate(audioClip2);
                                return;
                            }

                            SoundMetaData soundMetaData = ResourceManager.CreateSoundMetaData(1, 1, 0, audioClip2);
                            SoundData<SoundMetaData> soundData = new SoundData<SoundMetaData>("", false, soundMetaData);

                            loadedHitsounds.TryAdd(hitsound.path, new HitsoundInfo(soundData));
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                            Debug.ForceLogError($"[Path] {fullPath}", nameof(HitsoundEffect));

                            loadedHitsounds.TryAdd(hitsound.path, new HitsoundInfo());
                        }
                    }
                    else if (File.Exists(path + ".nbs"))
                    {
                        try
                        {
                            NBSFile nbsFile2 = NBSManager.ReadNBSFile(path + ".nbs");
                            NBSMetaData nbsMetaData = ResourceManager.CreateNBSMetaData(1, 1, nbsFile2);
                            SoundData<NBSMetaData> soundData = new SoundData<NBSMetaData>("", false, nbsMetaData);

                            loadedHitsounds.TryAdd(hitsound.path, new HitsoundInfo(soundData));
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                            Debug.ForceLogError($"[Path] {fullPath}", nameof(HitsoundEffect));

                            loadedHitsounds.TryAdd(hitsound.path, new HitsoundInfo());
                        }
                    }
                    else
                        loadedHitsounds.TryAdd(hitsound.path, new HitsoundInfo());

                    CustomHitsoundPlay(loadedHitsounds, map, instance, hitsound);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                DefaultHitsoundPlay(hitsound.volume, hitsound.pitch);
            }
        }



        public static int GetHitsoundPlayCount(MapFile map, double currentBeat, ref double lastValue) => GetHitsoundPlayCount(map, currentBeat, ref lastValue, out _);

        public static int GetHitsoundPlayCount(MapFile map, double currentBeat, ref double lastValue, out int index)
        {
            index = 0;

            if (map == null)
                return 0;

            double value = -1;
            int soundPlayCount = 0;
            if (map.allJudgmentBeat.Count <= 0)
                soundPlayCount = 0;
            else if (map.allJudgmentBeat[0] >= currentBeat)
                soundPlayCount = 1;
            else if (map.allJudgmentBeat.Count >= 2)
            {
                for (int i = 0; i < map.allJudgmentBeat.Count; i++)
                {
                    if (map.allJudgmentBeat[i] >= currentBeat)
                    {
                        double beat = map.allJudgmentBeat[i - 1];
                        value = beat;
                        index = i - 1;
                        soundPlayCount++;

                        for (int j = i - 2; j >= 0; j--)
                        {
                            if (beat == map.allJudgmentBeat[j])
                                soundPlayCount++;
                            else
                                break;
                        }

                        break;
                    }
                }
            }

            if (lastValue != value)
            {
                lastValue = value;
                return soundPlayCount;
            }

            return 0;
        }

        public class HitsoundInfo
        {
            public HitsoundInfo() => defaultAudio = true;

            public HitsoundInfo(SoundData<SoundMetaData> audioClip) => this.soundData = audioClip;

            public HitsoundInfo(SoundData<NBSMetaData> nbsFile) => this.nbsData = nbsFile;



            public bool defaultAudio { get; } = false;

            public SoundData<SoundMetaData> soundData { get; } = null;
            public SoundData<NBSMetaData> nbsData { get; } = null;
        }
    }
}
