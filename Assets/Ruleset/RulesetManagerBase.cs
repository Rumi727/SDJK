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
using SDJK.Replay;
using SDJK.Replay.Ruleset.SDJK;
using SDJK.Ruleset.PauseScreen;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Ruleset
{
    public abstract class RulesetManagerBase : ManagerBase<RulesetManagerBase>
    {
        [SerializeField] PauseScreenUI _pauseScreen; public PauseScreenUI pauseScreen => _pauseScreen;
        [SerializeField] EffectManager _effectManager; public EffectManager effectManager => _effectManager;
        [SerializeField] Button _replaySaveButton; public Button replaySaveButton => _replaySaveButton;

        public IRuleset ruleset { get; private set; }
        public MapFile map { get; private set; }

        public bool isReplay { get; private set; } = false;
        public ReplayFile currentReplay { get; private set; } = null;

        public ReplayFile createdReplay { get; private set; } = null;

        public ISoundPlayer soundPlayer { get; private set; }
        public AudioClip bgmClip { get; private set; }

        public bool isEditor { get; private set; }
        public IMode[] modes { get; private set; }

        public bool isPaused => pauseScreen.isShow;
        public abstract bool isGameOver { get; }

        bool isClear = false;
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
                IMode speedMode;
                if ((speedMode = modes.FindMode<FastModeBase>()) != null)
                    RhythmManager.speed *= (float)((FastModeBase.Data)speedMode.modeConfig).speed;
                else if ((speedMode = modes.FindMode<SlowModeBase>()) != null)
                    RhythmManager.speed *= (float)((SlowModeBase.Data)speedMode.modeConfig).speed;
            }

            if (map.info.clearBeat <= RhythmManager.currentBeatSound)
                Clear();
        }

        protected virtual void OnDestroy()
        {
            UIManager.BackEventRemove(Pause);

            if (bgmClip != null)
                Destroy(bgmClip, 1);
        }

        public virtual bool Refresh(MapFile map, ReplayFile replay, IRuleset ruleset, bool isEditor, IMode[] modes)
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
                    currentReplay = null;

                    replaySaveButton.interactable = true;
                    createdReplay = ReplayLoader.CreateReplay<SDJKReplayFile>(map, modes);
                }

                this.modes = modes;
                this.isEditor = isEditor;

                this.ruleset = ruleset;
                this.map = map;

                effectManager.selectedRuleset = ruleset;
                effectManager.selectedModes = modes;

                effectManager.selectedMap = map;
                effectManager.AllRefresh();

                RhythmManager.Play(map.globalEffect.bpm, map.info.songOffset, map.globalEffect.yukiMode, null, 2);
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

            isRestart = true;

            if (bgmClip != null)
                Destroy(bgmClip, 1);

            ruleset.GameStart(map.mapFilePath, isReplay ? currentReplay.replayFilePath : null, isEditor, modes);
        }

        bool isQuit = false;
        public virtual void Quit()
        {
            if (isQuit)
                return;

            isQuit = true;

            if (bgmClip != null)
                Destroy(bgmClip, 1);

            MainMenuLoader.Load();
            UIManager.BackEventRemove(Pause);
        }

        public virtual void Pause()
        {
            if (isGameOver)
                return;
            else if (isPaused)
                return;

            pauseScreen.Show();
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
