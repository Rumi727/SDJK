using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.NBS;
using SCKRM.Resource;
using SCKRM.Rhythm;
using SCKRM.Sound;
using SCKRM.UI;
using SDJK.Effect;
using SDJK.MainMenu;
using SDJK.Map;
using SDJK.Mode;
using SDJK.Mode.Automatic;
using SDJK.Mode.Difficulty;
using SDJK.Mode.Fun;
using SDJK.Replay;
using SDJK.Ruleset.UI.PauseScreen;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Ruleset
{
    public abstract class RulesetManagerBase<TRuleset, TMapFile, TReplayFile> : ManagerBase<RulesetManagerBase<TRuleset, TMapFile, TReplayFile>> where TRuleset : IRuleset where TMapFile : MapFile where TReplayFile : ReplayFile, new()
    {
        [SerializeField] PauseScreenUI _pauseScreen; public PauseScreenUI pauseScreen => _pauseScreen;
        [SerializeField] EffectManager _effectManager; public EffectManager effectManager => _effectManager;
        [SerializeField] Button _replaySaveButton; public Button replaySaveButton => _replaySaveButton;

        public TRuleset ruleset { get; private set; }
        public TMapFile map { get; private set; }

        public bool isReplay { get; private set; } = false;
        public TReplayFile currentReplay { get; private set; } = null;

        public TReplayFile createdReplay { get; set; }

        public ISoundPlayer soundPlayer { get; private set; }
        public AudioClip bgmClip { get; private set; }

        public bool isEditor { get; private set; }
        public IMode[] modes { get; private set; }

        public bool isPaused => pauseScreen.isShow;
        public abstract bool isGameOver { get; }

        public double accelerationDeceleration { get; set; } = 1;

        public Dictionary<string, HitsoundEffect.HitsoundInfo> loadedHitsounds { get; } = new Dictionary<string, HitsoundEffect.HitsoundInfo>();

        bool isClear = false;
        double realAccelerationDeceleration = 1;
        protected virtual void Update()
        {
            if (!RhythmManager.isPlaying || map == null)
                return;

            if (soundPlayer != null)
            {
                soundPlayer.pitch = (float)map.globalEffect.pitch.GetValue(RhythmManager.currentBeatSound);
                soundPlayer.volume = (float)map.globalEffect.volume.GetValue(RhythmManager.currentBeatSound);
            }

            RhythmManager.speed = (float)map.globalEffect.tempo.GetValue(RhythmManager.currentBeatSound);

            //모드
            {
                IMode mode;
                if ((mode = modes.FindMode<FastModeBase>()) != null)
                    RhythmManager.speed *= (float)((FastModeBase.Config)mode.modeConfig).speed;
                else if ((mode = modes.FindMode<SlowModeBase>()) != null)
                    RhythmManager.speed *= (float)((SlowModeBase.Config)mode.modeConfig).speed;

                if (RhythmManager.currentBeatSound >= 0)
                {
                    if ((mode = modes.FindMode<AccelerationModeBase>()) != null)
                    {
                        AccelerationModeBase.Config config = (AccelerationModeBase.Config)mode.modeConfig;
                        double value = config.coefficient * Kernel.deltaTimeDouble;

                        accelerationDeceleration += value;
                        realAccelerationDeceleration -= value;

                        realAccelerationDeceleration = realAccelerationDeceleration.Clamp(0, config.max);
                        accelerationDeceleration = accelerationDeceleration.Clamp(0, config.max);

                        realAccelerationDeceleration = realAccelerationDeceleration.Lerp(accelerationDeceleration, 0.0625f * Kernel.fpsUnscaledDeltaTime);
                        RhythmManager.speed *= realAccelerationDeceleration;
                    }
                    else if ((mode = modes.FindMode<DecelerationModeBase>()) != null)
                    {
                        DecelerationModeBase.Config config = (DecelerationModeBase.Config)mode.modeConfig;
                        double value = config.coefficient * Kernel.deltaTimeDouble;

                        accelerationDeceleration -= value;
                        realAccelerationDeceleration -= value;

                        realAccelerationDeceleration = realAccelerationDeceleration.Clamp(config.min);
                        accelerationDeceleration = accelerationDeceleration.Clamp(config.min);

                        realAccelerationDeceleration = realAccelerationDeceleration.Lerp(accelerationDeceleration, 0.0625f * Kernel.fpsUnscaledDeltaTime);
                        RhythmManager.speed *= realAccelerationDeceleration;
                    }
                    else
                        accelerationDeceleration = 1;
                }
            }

            if (map.info.clearBeat <= RhythmManager.currentBeatSound)
                Clear();
        }

        protected virtual void OnDestroy()
        {
            UIManager.BackEventRemove(Pause);
            AudioDestroy();
        }

        protected virtual void OnApplicationFocus(bool focus)
        {
            if (!focus && !isReplay)
                Pause();
        }

        public virtual bool Refresh(TMapFile map, TReplayFile replay, TRuleset ruleset, bool isEditor, IMode[] modes)
        {
            if (SingletonCheck(this))
            {
                UIManager.BackEventAdd(Pause);

                if (replay != null)
                {
                    isReplay = true;
                    currentReplay = replay;

                    replaySaveButton.interactable = false;
                }
                else
                {
                    isReplay = false;
                    replaySaveButton.interactable = true;

                    createdReplay = ReplayLoader.CreateReplay<TReplayFile>(map, modes);
                    currentReplay = createdReplay;

                    Debug.Log(currentReplay);
                    Debug.Log("Asdf");
                }

                this.modes = modes;
                this.isEditor = isEditor;

                this.ruleset = ruleset;
                this.map = map;

                effectManager.selectedRuleset = ruleset;
                effectManager.selectedModes = modes;

                effectManager.selectedMap = map;
                effectManager.AllRefresh();

                RhythmManager.Play(map.globalEffect.bpm, map.info.songOffset, map.globalEffect.yukiMode, map.info.clearBeat, null, 2);
                BGMPlay().Forget();

                return true;
            }

            return false;
        }

        public virtual void Clear()
        {
            if (isClear)
                return;

            ReplayFile replay;
            if (isReplay)
            {
                replay = currentReplay;

                if (modes.FindMode<AutoModeBase>() != null)
                {
                    currentReplay.scores.Add(double.MinValue, JudgementUtility.maxScore);

                    currentReplay.combos.Add(double.MinValue, map.allJudgmentBeat.Count);
                    currentReplay.maxCombo.Add(double.MinValue, map.allJudgmentBeat.Count);

                    currentReplay.accuracyAbses.Add(double.MinValue, 0);
                    currentReplay.accuracys.Add(double.MinValue, 0);
                }
            }
            else
            {
                ReplaySave();
                replay = createdReplay;
            }

            ResultScreen.Show(ruleset, map, replay, Quit);
            isClear = true;
        }

        public virtual void ReplaySave()
        {
            if (isReplay)
                return;

            createdReplay.ReplaySave(map, modes);
        }

        bool isRestart = false;
        public virtual void Restart()
        {
            if (isRestart)
                return;

            RhythmManager.Stop();

            SoundManager.StopSoundAll();
            SoundManager.StopNBSAll();

            isRestart = true;
            AudioDestroy();

            ruleset.GameStart(map.mapFilePath, isReplay ? currentReplay.replayFilePath : null, isEditor, modes);
        }

        bool isQuit = false;
        public virtual void Quit()
        {
            if (isQuit)
                return;

            RhythmManager.Stop();

            SoundManager.StopSoundAll();
            SoundManager.StopNBSAll();

            isQuit = true;
            AudioDestroy();

            MainMenuLoader.Load();
            UIManager.BackEventRemove(Pause);
        }

        public virtual void Pause()
        {
            if (isGameOver || isPaused || isClear)
                return;

            pauseScreen.Show();
        }

        public void HitsoundPlay(HitsoundFile hitsound) => HitsoundEffect.CustomHitsoundPlay(loadedHitsounds, map, this, hitsound);

        void AudioDestroy()
        {
            if (bgmClip != null)
            {
                Destroy(bgmClip, 1);
                bgmClip = null;
            }

            foreach (var item in loadedHitsounds)
            {
                if (item.Value.soundData != null && item.Value.soundData.sounds != null)
                {
                    for (int j = 0; j < item.Value.soundData.sounds.Length; j++)
                    {
                        SoundMetaData soundMetaData = item.Value.soundData.sounds[j];
                        if (soundMetaData.audioClip != null)
                            Destroy(soundMetaData.audioClip);
                    }
                }
            }
        }

        async UniTaskVoid BGMPlay()
        {
            string path = PathUtility.Combine(map.mapFilePathParent, map.info.songFile);
            if (ResourceManager.FileExtensionExists(path, out string fullPath, ResourceManager.audioExtension))
            {
                bgmClip = await ResourceManager.GetAudio(fullPath, true, true);
                SoundMetaData soundMetaData = ResourceManager.CreateSoundMetaData(1, 1, 0, bgmClip);
                SoundData<SoundMetaData> soundData = ResourceManager.CreateSoundData("", true, soundMetaData);

                if (!Kernel.isPlaying || this == null)
                    return;

                soundPlayer = SoundManager.PlaySound(soundData);
            }
            else if (File.Exists(path + ".nbs"))
            {
                NBSFile nbsFile = NBSManager.ReadNBSFile(path + ".nbs");
                NBSMetaData nbsMetaData = ResourceManager.CreateNBSMetaData(1, 1, nbsFile);
                SoundData<NBSMetaData> soundData = ResourceManager.CreateSoundData("", true, nbsMetaData);

                soundPlayer = SoundManager.PlayNBS(soundData);
            }

            effectManager.soundPlayer = soundPlayer;
            RhythmManager.SoundPlayerChange(soundPlayer);
        }
    }
}
