using Cysharp.Threading.Tasks;
using K4.Threading;
using Newtonsoft.Json;
using SCKRM.NBS;
using SCKRM.Object;
using SCKRM.Resource;
using SCKRM.SaveLoad;
using SCKRM.Threads;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace SCKRM.Sound
{
    [WikiDescription("사운드를 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/Sound/Sound Manager", 0)]
    public sealed class SoundManager : ManagerBase<SoundManager>
    {
        [GeneralSaveLoad]
        public sealed class SaveData
        {
            [JsonProperty]
            public static int mainVolume
            {
                get => _mainVolume.Clamp(0, 200);
                set
                {
                    _mainVolume = value;

                    if (ThreadManager.isMainThread)
                        VolumeChange();
                    else
                        K4UnityThreadDispatcher.Execute(VolumeChange);

                    void VolumeChange()
                    {
                        //볼륨을 사용자가 설정한 볼륨으로 조정시킵니다. 사용자가 설정한 볼륨은 int 0 ~ 200 이기 때문에 0.01을 곱해주어야 하고,
                        //100 ~ 200 볼륨이 먹혀야하기 때문에 0.5로 볼륨을 낮춰야하기 때문에 0.005를 곱합니다
                        AudioListener.volume = value.Clamp(0, 200) * 0.005f * mainVolumeNoFocusAni;
                    }
                }
            }
            static int _mainVolume = 100;
            static int _mainVolumeNoFocus = 25; [JsonProperty] public static int mainVolumeNoFocus
            {
                get => _mainVolumeNoFocus.Clamp(0, 200);
                set
                {
                    _mainVolumeNoFocus = value;
                    mainVolume = mainVolume;
                }
            }

            static int _bgmVolume = 100; [JsonProperty] public static int bgmVolume { get => _bgmVolume.Clamp(0, 200); set => _bgmVolume = value; }
            static int _soundVolume = 100; [JsonProperty] public static int soundVolume { get => _soundVolume.Clamp(0, 200); set => _soundVolume = value; }

            [JsonProperty]
            public static bool fixAudioLatency
            {
                get => _fixAudioLatency;
                set
                {
                    if (fixAudioLatency == value || ResourceManager.isAudioReset)
                        return;

                    _fixAudioLatency = value;

                    if (!InitialLoadManager.isInitialLoadEnd)
                        return;

                    if (ThreadManager.isMainThread)
                        FixAudioLatencyChange();
                    else
                        K4UnityThreadDispatcher.Execute(FixAudioLatencyChange);

                    void FixAudioLatencyChange()
                    {
                        AudioConfiguration audioConfiguration = AudioSettings.GetConfiguration();
                        if (value)
                            audioConfiguration.dspBufferSize = 256;
                        else
                            audioConfiguration.dspBufferSize = 1024;

                        ResourceManager.AudioReset(audioConfiguration).Forget();
                    }
                }
            }
            static bool _fixAudioLatency = true;

            [JsonProperty] public static bool useTempo { get; set; }
        }

        [SerializeField] AudioMixerGroup _audioMixerGroup;
        public AudioMixerGroup audioMixerGroup => _audioMixerGroup;

        [WikiDescription("현재 재생되고 있는 사운드 리스트")] public static List<SoundPlayer> soundList { get; } = new List<SoundPlayer>();
        [WikiDescription("현재 재생되고 있는 NBS 리스트")] public static List<NBSPlayer> nbsList { get; } = new List<NBSPlayer>();



        public const int maxSoundCount = 256;
        public const int maxNBSCount = 16;



        void Awake() => SingletonCheck(this);

        static float lastGameSpeed = Kernel.gameSpeed;
        static float mainVolumeNoFocusAni = 1;
        static float lastMainVolumeNoFocusAni = 1;
        void Update()
        {
            if (Kernel.gameSpeed != lastGameSpeed)
            {
                for (int i = 0; i < soundList.Count; i++)
                    soundList[i].RefreshTempoAndPitch();

                lastGameSpeed = Kernel.gameSpeed;
            }

            if (Application.isFocused)
                mainVolumeNoFocusAni = mainVolumeNoFocusAni.MoveTowards(1, 0.03f * Kernel.fpsDeltaTime);
            else
                mainVolumeNoFocusAni = mainVolumeNoFocusAni.MoveTowards(SaveData.mainVolumeNoFocus * 0.01f, 0.01f * Kernel.fpsDeltaTime);

            if (lastMainVolumeNoFocusAni != mainVolumeNoFocusAni)
            {
                SaveData.mainVolume = SaveData.mainVolume;
                lastMainVolumeNoFocusAni = mainVolumeNoFocusAni;
            }
        }

        void OnApplicationFocus(bool focus) => SaveData.mainVolume = SaveData.mainVolume;

        /// <summary>
        /// It should only run on the main thread
        /// </summary>
        [WikiDescription("모든 사운드 플레이어 새로고침")]
        public static void SoundRefresh()
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException();
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException();
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException();



#if UNITY_2023_1_OR_NEWER
            ISoundPlayer[] soundPlayer = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISoundPlayer>().ToArray();
#else
            ISoundPlayer[] soundPlayer = FindObjectsOfType<MonoBehaviour>().OfType<ISoundPlayer>().ToArray();
#endif
            for (int i = 0; i < soundPlayer.Length; i++)
                soundPlayer[i].Refresh();
        }

        /// <summary>
        /// 이 메소드는 ResourceManager.AudioReset 메소드를 호출하며 유니티 이벤트를 위해 존재합니다. 스크립트에서 사용하지 말아 주세요
        /// This method calls the ResourceManager.AudioReset method and exists for Unity events. Please don't use it in scripts
        /// </summary>
        [Obsolete("This method calls the ResourceManager.AudioReset method and exists for Unity events. Please don't use it in scripts", true)]
        [WikiIgnore]
        public static void AudioReset() => ResourceManager.AudioReset();

        /// <summary>
        /// 소리를 재생합니다
        /// </summary>
        /// <param name="key">
        /// 오디오 키
        /// </param>
        /// <param name="nameSpace">
        /// 네임스페이스
        /// </param>
        /// <param name="volume">
        /// 볼륨
        /// </param>
        /// <param name="loop">
        /// 반복
        /// </param>
        /// <param name="pitch">
        /// 피치
        /// </param>
        /// <param name="tempo">
        /// 템포
        /// </param>
        /// <param name="panStereo">
        /// 스테레오
        /// </param>
        /// <returns></returns>
        [WikiDescription("소리를 재생합니다")] public static SoundPlayer PlaySound(string key, string nameSpace = "", float volume = 1, bool loop = false, float pitch = 1, float tempo = 1, float panStereo = 0) => playSound(key, nameSpace, null, volume, loop, pitch, tempo, panStereo, false, 0, 16, null, 0, 0, 0);

        /// <summary>
        /// 소리를 재생합니다
        /// </summary>
        /// <param name="key">
        /// 오디오 키
        /// </param>
        /// <param name="nameSpace">
        /// 네임스페이스
        /// </param>
        /// <param name="volume">
        /// 볼륨
        /// </param>
        /// <param name="loop">
        /// 반복
        /// </param>
        /// <param name="pitch">
        /// 피치
        /// </param>
        /// <param name="tempo">
        /// 템포
        /// </param>
        /// <param name="panStereo">
        /// 스테레오
        /// </param>
        /// <param name="minDistance">
        /// 최소 거리
        /// </param>
        /// <param name="maxDistance">
        /// 최대 거리
        /// </param>
        /// <param name="parent">
        /// 부모
        /// </param>
        /// <param name="x">
        /// X 좌표
        /// </param>
        /// <param name="y">
        /// Y 좌표
        /// </param>
        /// <param name="z">
        /// Z 좌표
        /// </param>
        /// <returns></returns>
        [WikiIgnore] public static SoundPlayer PlaySound(string key, string nameSpace, float volume, bool loop, float pitch, float tempo, float panStereo, float minDistance = 0, float maxDistance = 16, Transform parent = null, float x = 0, float y = 0, float z = 0) => playSound(key, nameSpace, null, volume, loop, pitch, tempo, panStereo, true, minDistance, maxDistance, parent, x, y, z);

        /// <summary>
        /// 소리를 재생합니다
        /// </summary>
        /// <param name="soundData">
        /// 사운드 데이터
        /// </param>
        /// <param name="volume">
        /// 볼륨
        /// </param>
        /// <param name="loop">
        /// 반복
        /// </param>
        /// <param name="pitch">
        /// 피치
        /// </param>
        /// <param name="tempo">
        /// 템포
        /// </param>
        /// <param name="panStereo">
        /// 스테레오
        /// </param>
        /// <returns></returns>
        [WikiIgnore] public static SoundPlayer PlaySound(SoundData<SoundMetaData> soundData, float volume = 1, bool loop = false, float pitch = 1, float tempo = 1, float panStereo = 0) => playSound("", "", soundData, volume, loop, pitch, tempo, panStereo, false, 0, 16, null, 0, 0, 0);

        /// <summary>
        /// 소리를 재생합니다
        /// </summary>
        /// <param name="soundData">
        /// 사운드 데이터
        /// </param>
        /// <param name="volume">
        /// 볼륨
        /// </param>
        /// <param name="loop">
        /// 반복
        /// </param>
        /// <param name="pitch">
        /// 피치
        /// </param>
        /// <param name="tempo">
        /// 템포
        /// </param>
        /// <param name="panStereo">
        /// 스테레오
        /// </param>
        /// <param name="minDistance">
        /// 최소 거리
        /// </param>
        /// <param name="maxDistance">
        /// 최대 거리
        /// </param>
        /// <param name="parent">
        /// 부모
        /// </param>
        /// <param name="x">
        /// X 좌표
        /// </param>
        /// <param name="y">
        /// Y 좌표
        /// </param>
        /// <param name="z">
        /// Z 좌표
        /// </param>
        /// <returns></returns>
        [WikiIgnore] public static SoundPlayer PlaySound(SoundData<SoundMetaData> soundData, float volume, bool loop, float pitch, float tempo, float panStereo, float minDistance = 0, float maxDistance = 16, Transform parent = null, float x = 0, float y = 0, float z = 0) => playSound("", "", soundData, volume, loop, pitch, tempo, panStereo, true, minDistance, maxDistance, parent, x, y, z);

        static SoundPlayer playSound(string key, string nameSpace, SoundData<SoundMetaData> soundData, float volume, bool loop, float pitch, float tempo, float panStereo, bool spatial, float minDistance, float maxDistance, Transform parent, float x, float y, float z)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException();
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException();
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException();

            if (soundList.Count >= maxSoundCount)
            {
                for (int i = 0; i < soundList.Count; i++)
                {
                    SoundPlayer soundObject2 = soundList[i];
                    if (!soundObject2.soundData.isBGM)
                    {
                        soundList[i].Remove();
                        break;
                    }
                }
            }

            if (key == null)
                key = "";
            if (nameSpace == null)
                nameSpace = "";
            if (parent == null)
                parent = instance.transform;

            if (nameSpace == "")
                nameSpace = ResourceManager.defaultNameSpace;

            SoundPlayer soundPlayer = (SoundPlayer)ObjectPoolingSystem.ObjectCreate("sound_manager.sound_object", parent, false).monoBehaviour;
            soundPlayer.key = key;
            soundPlayer.nameSpace = nameSpace;
            if (soundData != null)
                soundPlayer.customSoundData = soundData;

            soundPlayer.volume = volume;
            soundPlayer.loop = loop;
            soundPlayer.pitch = pitch;
            soundPlayer.tempo = tempo;

            soundPlayer.panStereo = panStereo;
            soundPlayer.spatial = spatial;

            soundPlayer.minDistance = minDistance;
            soundPlayer.maxDistance = maxDistance;

            soundPlayer.localPosition = new Vector3(x, y, z);

            soundPlayer.Refresh();
            return soundPlayer;
        }

        /// <summary>
        /// 소리를 중지합니다
        /// </summary>
        /// <param name="key">
        /// 중지할 오디오 키
        /// </param>
        /// <param name="nameSpace">
        /// 네임스페이스
        /// </param>
        /// <param name="all">
        /// 전부 삭제
        /// </param>
        /// <exception cref="NotMainThreadMethodException"></exception>
        /// <exception cref="NotPlayModeMethodException"></exception>
        /// <exception cref="NotInitialLoadEndMethodException"></exception>
        [WikiDescription("소리를 중지합니다")]
        public static int StopSound(string key, string nameSpace = "", bool all = true)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException();
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException();
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException();

            if (key == null)
                key = "";
            if (nameSpace == null)
                nameSpace = "";

            if (nameSpace == "")
                nameSpace = ResourceManager.defaultNameSpace;

            int stopCount = 0;
            for (int i = 0; i < soundList.Count; i++)
            {
                SoundPlayer soundObject = soundList[i];
                if (soundObject.key == key && soundObject.nameSpace == nameSpace)
                {
                    soundObject.Remove();
                    if (!all)
                        return 1;

                    stopCount++;
                    i--;
                }
            }

            return stopCount;
        }

        /// <summary>
        /// 모든 오디오를 중지
        /// Stop all audio
        /// </summary>
        [WikiDescription("모든 오디오를 중지")]
        public static int StopSoundAll()
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException();
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException();
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException();

            int stopCount = 0;
            for (int i = 0; i < soundList.Count; i++)
            {
                SoundPlayer soundObject = soundList[i];
                soundObject.Remove();

                stopCount++;
                i--;
            }

            return stopCount;
        }

        /// <summary>
        /// 모든 효과음 또는 BGM 중지
        /// Stop all sounds or bgm
        /// </summary>
        [WikiDescription("모든 효과음 또는 BGM 중지")]
        public static int StopSoundAll(bool bgm)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException();
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException();
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException();

            int stopCount = 0;
            for (int i = 0; i < soundList.Count; i++)
            {
                SoundPlayer soundObject = soundList[i];
                if (bgm && soundObject.soundData.isBGM)
                {
                    soundObject.Remove();

                    stopCount++;
                    i--;
                }
                else if (!bgm && !soundObject.soundData.isBGM)
                {
                    soundObject.Remove();

                    stopCount++;
                    i--;
                }
            }

            return stopCount;
        }



        /// <summary>
        /// 소리를 재생합니다
        /// </summary>
        /// <param name="key">
        /// 오디오 키
        /// </param>
        /// <param name="nameSpace">
        /// 네임스페이스
        /// </param>
        /// <param name="volume">
        /// 볼륨
        /// </param>
        /// <param name="loop">
        /// 반복
        /// </param>
        /// <param name="pitch">
        /// 피치
        /// </param>
        /// <param name="tempo">
        /// 템포
        /// </param>
        /// <param name="panStereo">
        /// 스테레오
        /// </param>
        /// <returns></returns>
        [WikiDescription("소리를 재생합니다")]
        public static NBSPlayer PlayNBS(string key, string nameSpace = "", float volume = 1, bool loop = false, float pitch = 1, float tempo = 1, float panStereo = 0) => playNBS(key, nameSpace, null, volume, loop, pitch, tempo, panStereo, false, 0, 16, null, 0, 0, 0);

        /// <summary>
        /// 소리를 재생합니다
        /// </summary>
        /// <param name="key">
        /// 오디오 키
        /// </param>
        /// <param name="nameSpace">
        /// 네임스페이스
        /// </param>
        /// <param name="volume">
        /// 볼륨
        /// </param>
        /// <param name="loop">
        /// 반복
        /// </param>
        /// <param name="pitch">
        /// 피치
        /// </param>
        /// <param name="tempo">
        /// 템포
        /// </param>
        /// <param name="panStereo">
        /// 스테레오
        /// </param>
        /// <param name="minDistance">
        /// 최소 거리
        /// </param>
        /// <param name="maxDistance">
        /// 최대 거리
        /// </param>
        /// <param name="parent">
        /// 부모
        /// </param>
        /// <param name="x">
        /// X 좌표
        /// </param>
        /// <param name="y">
        /// Y 좌표
        /// </param>
        /// <param name="z">
        /// Z 좌표
        /// </param>
        /// <returns></returns>
        [WikiIgnore] public static NBSPlayer PlayNBS(string key, string nameSpace, float volume, bool loop, float pitch, float tempo, float panStereo, float minDistance = 0, float maxDistance = 16, Transform parent = null, float x = 0, float y = 0, float z = 0) => playNBS(key, nameSpace, null, volume, loop, pitch, tempo, panStereo, true, minDistance, maxDistance, parent, x, y, z);

        /// <summary>
        /// 소리를 재생합니다
        /// </summary>
        /// <param name="nbsData">
        /// 사운드 데이터
        /// </param>
        /// <param name="volume">
        /// 볼륨
        /// </param>
        /// <param name="loop">
        /// 반복
        /// </param>
        /// <param name="pitch">
        /// 피치
        /// </param>
        /// <param name="tempo">
        /// 템포
        /// </param>
        /// <param name="panStereo">
        /// 스테레오
        /// </param>
        /// <returns></returns>
        [WikiIgnore] public static NBSPlayer PlayNBS(SoundData<NBSMetaData> nbsData, float volume = 1, bool loop = false, float pitch = 1, float tempo = 1, float panStereo = 0) => playNBS("", "", nbsData, volume, loop, pitch, tempo, panStereo, false, 0, 16, null, 0, 0, 0);

        /// <summary>
        /// 소리를 재생합니다
        /// </summary>
        /// <param name="nbsData">
        /// 사운드 데이터
        /// </param>
        /// <param name="volume">
        /// 볼륨
        /// </param>
        /// <param name="loop">
        /// 반복
        /// </param>
        /// <param name="pitch">
        /// 피치
        /// </param>
        /// <param name="tempo">
        /// 템포
        /// </param>
        /// <param name="panStereo">
        /// 스테레오
        /// </param>
        /// <param name="minDistance">
        /// 최소 거리
        /// </param>
        /// <param name="maxDistance">
        /// 최대 거리
        /// </param>
        /// <param name="parent">
        /// 부모
        /// </param>
        /// <param name="x">
        /// X 좌표
        /// </param>
        /// <param name="y">
        /// Y 좌표
        /// </param>
        /// <param name="z">
        /// Z 좌표
        /// </param>
        /// <returns></returns>
        [WikiIgnore] public static NBSPlayer PlayNBS(SoundData<NBSMetaData> nbsData, float volume, bool loop, float pitch, float tempo, float panStereo, float minDistance = 0, float maxDistance = 16, Transform parent = null, float x = 0, float y = 0, float z = 0) => playNBS("", "", nbsData, volume, loop, pitch, tempo, panStereo, true, minDistance, maxDistance, parent, x, y, z);

        static NBSPlayer playNBS(string key, string nameSpace, SoundData<NBSMetaData> nbsData, float volume, bool loop, float pitch, float tempo, float panStereo, bool spatial, float minDistance, float maxDistance, Transform parent, float x, float y, float z)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException();
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException();
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException();

            if (nbsList.Count >= maxNBSCount)
            {
                for (int i = 0; i < nbsList.Count; i++)
                {
                    NBSPlayer nbsObject2 = nbsList[i];
                    if (!nbsObject2.soundData.isBGM)
                    {
                        nbsList[i].Remove();
                        break;
                    }
                }
            }

            if (key == null)
                key = "";
            if (nameSpace == null)
                nameSpace = "";
            if (parent == null)
                parent = instance.transform;

            if (nameSpace == "")
                nameSpace = ResourceManager.defaultNameSpace;

            NBSPlayer nbsPlayer = (NBSPlayer)ObjectPoolingSystem.ObjectCreate("sound_manager.nbs_player", parent, false).monoBehaviour;
            nbsPlayer.key = key;
            nbsPlayer.nameSpace = nameSpace;
            if (nbsData != null)
                nbsPlayer.customSoundData = nbsData;

            nbsPlayer.volume = volume;
            nbsPlayer.loop = loop;
            nbsPlayer.pitch = pitch;
            nbsPlayer.tempo = tempo;

            nbsPlayer.panStereo = panStereo;
            nbsPlayer.spatial = spatial;

            nbsPlayer.minDistance = minDistance;
            nbsPlayer.maxDistance = maxDistance;

            nbsPlayer.localPosition = new Vector3(x, y, z);

            nbsPlayer.Refresh();
            return nbsPlayer;
        }

        /// <summary>
        /// NBS 중지
        /// </summary>
        [WikiDescription("NBS 중지")]
        public static int StopNBS(string key, string nameSpace = "", bool all = true)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException();
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException();
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException();

            if (key == null)
                key = "";
            if (nameSpace == null)
                nameSpace = "";

            if (nameSpace == "")
                nameSpace = ResourceManager.defaultNameSpace;

            int stopCount = 0;
            for (int i = 0; i < nbsList.Count; i++)
            {
                NBSPlayer nbsPlayer = nbsList[i];
                if (nbsPlayer.key == key && nbsPlayer.nameSpace == nameSpace)
                {
                    nbsPlayer.Remove();
                    if (!all)
                        return stopCount;

                    stopCount++;
                    i--;
                }
            }

            return stopCount;
        }

        /// <summary>
        /// 모든 NBS 중지
        /// </summary>
        /// <exception cref="NotMainThreadMethodException"></exception>
        /// <exception cref="NotPlayModeMethodException"></exception>
        /// <exception cref="NotInitialLoadEndMethodException"></exception>
        [WikiDescription("모든 NBS 중지")]
        public static int StopNBSAll()
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException();
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException();
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException();

            int stopCount = 0;
            for (int i = 0; i < nbsList.Count; i++)
            {
                NBSPlayer nbsPlayer = nbsList[i];
                nbsPlayer.Remove();

                stopCount++;
                i--;
            }

            return stopCount;
        }

        /// <summary>
        /// 모든 NBS 효과음 또는 NBS 중지
        /// Stop all NBS sounds or NBS
        /// </summary>
        [WikiDescription("모든 NBS 효과음 또는 NBS 중지")]
        public static int StopNBSAll(bool bgm)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException();
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException();
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException();

            int stopCount = 0;
            for (int i = 0; i < nbsList.Count; i++)
            {
                NBSPlayer nbsObject = nbsList[i];
                if (bgm && nbsObject.soundData.isBGM)
                {
                    nbsObject.Remove();

                    stopCount++;
                    i--;
                }
                else if (!bgm && !nbsObject.soundData.isBGM)
                {
                    nbsObject.Remove();

                    stopCount++;
                    i--;
                }
            }

            return stopCount;
        }
    }
}