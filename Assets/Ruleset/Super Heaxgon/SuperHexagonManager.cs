using Cysharp.Threading.Tasks;
using SCKRM.NBS;
using SCKRM.Resource;
using SCKRM.Rhythm;
using SCKRM.Sound;
using SCKRM.UI;
using SCKRM;
using SDJK.Effect;
using SDJK.MainMenu;
using SDJK.Mode.Automatic;
using SDJK.Mode.Difficulty;
using SDJK.Mode;
using SDJK.Replay;
using SDJK.Ruleset.PauseScreen;
using UnityEngine;
using SDJK.Ruleset.SuperHexagon.GameOver;
using UnityEngine.UI;
using SDJK.Replay.Ruleset.SuperHexagon;
using SDJK.Map.Ruleset.SuperHexagon.Map;
using System.IO;

namespace SDJK.Ruleset.SuperHexagon
{
    public sealed class SuperHexagonManager : ManagerBase<SuperHexagonManager>
    {
        [SerializeField] SuperHexagonGameOverManager _gameOverManager; public SuperHexagonGameOverManager gameOverManager => _gameOverManager;
        [SerializeField] PauseScreenUI _pauseScreen; public PauseScreenUI pauseScreen => _pauseScreen;
        [SerializeField] EffectManager _effectManager; public EffectManager effectManager => _effectManager;
        [SerializeField] Button _replaySaveButton; public Button replaySaveButton => _replaySaveButton;
        [SerializeField] Field _field; public Field field => _field;

        public SuperHexagonRuleset ruleset { get; private set; }
        public SuperHexagonMapFile map { get; private set; }

        public bool isReplay { get; private set; } = false;
        public SuperHexagonReplayFile currentReplay { get; private set; } = null;

        public SuperHexagonReplayFile createdReplay { get; private set; } = null;

        public ISoundPlayer soundPlayer { get; private set; }
        public AudioClip bgmClip { get; private set; }

        public bool isEditor { get; private set; }
        public IMode[] modes { get; private set; }

        public bool isPaused => pauseScreen.isShow;

        bool isClear = false;
        void Update()
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

        void OnDestroy()
        {
            UIManager.BackEventRemove(Pause);

            if (bgmClip != null)
                Destroy(bgmClip, 1);
        }

        public void Refresh(SuperHexagonMapFile map, SuperHexagonReplayFile replay, SuperHexagonRuleset ruleset, bool isEditor, IMode[] modes)
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
                    createdReplay = ReplayLoader.CreateReplay<SuperHexagonReplayFile>(map, modes);
                }

                this.modes = modes;
                this.isEditor = isEditor;

                this.ruleset = ruleset;
                this.map = map;
                effectManager.selectedMap = map;
                effectManager.AllRefresh();

                RhythmManager.Play(map.globalEffect.bpm, map.info.songOffset, map.globalEffect.yukiMode, null, 3);
                BGMPlay().Forget();

                field.Refresh(this);
            }
        }

        public void Clear()
        {
            if (isClear)
                return;

            ReplayFile replay;
            if (isReplay)
            {
                replay = currentReplay;

                if (modes.FindMode<AutoModeBase>() != null)
                {
                    currentReplay.scores.Add(double.MinValue, JudgementManager.maxScore);

                    currentReplay.combos.Add(double.MinValue, map.allJudgmentBeat.Count);
                    currentReplay.maxCombo.Add(double.MinValue, map.allJudgmentBeat.Count);

                    currentReplay.accuracys.Add(double.MinValue, 0);
                    currentReplay.accuracyUnclampeds.Add(double.MinValue, 0);
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

        public void ReplaySave()
        {
            if (isReplay)
                return;

            createdReplay.ReplaySave(map, modes);
        }

        bool isRestart = false;
        public void Restart()
        {
            if (isRestart)
                return;

            isRestart = true;

            if (bgmClip != null)
                Destroy(bgmClip, 1);

            ruleset.GameStart(map.mapFilePath, isReplay ? currentReplay.replayFilePath : null, isEditor, modes);
        }

        bool isQuit = false;
        public void Quit()
        {
            if (isQuit)
                return;

            isQuit = true;

            if (bgmClip != null)
                Destroy(bgmClip, 1);

            MainMenuLoad.Load();
            UIManager.BackEventRemove(Pause);
        }

        public void Pause()
        {
            if (gameOverManager.isGameOver)
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
                bgmClip = await ResourceManager.GetAudio(fullPath, true, false);
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
