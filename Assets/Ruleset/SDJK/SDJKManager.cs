using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.NBS;
using SCKRM.Object;
using SCKRM.Resource;
using SCKRM.Rhythm;
using SCKRM.Sound;
using SCKRM.UI;
using SDJK.Effect;
using SDJK.MainMenu;
using SDJK.Map.Ruleset.SDJK.Map;
using SDJK.Mode;
using SDJK.Replay;
using SDJK.Ruleset.PauseScreen;
using SDJK.Ruleset.SDJK.Effect;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Ruleset.SDJK
{
    public sealed class SDJKManager : ManagerBase<SDJKManager>
    {
        [SerializeField] SDJKGameOverManager _gameOverManager; public SDJKGameOverManager gameOverManager => _gameOverManager;
        [SerializeField] PauseScreenUI _pauseScreen; public PauseScreenUI pauseScreen => _pauseScreen;
        [SerializeField] EffectManager _effectManager; public EffectManager effectManager => _effectManager;
        [SerializeField] Button _replaySaveButton; public Button replaySaveButton => _replaySaveButton;

        public SDJKRuleset ruleset { get; private set; }
        public SDJKMapFile map { get; private set; }

        public bool isReplay { get; private set; } = false;
        public SDJKReplayFile currentReplay { get; private set; } = null;

        public SDJKReplayFile createdReplay { get; private set; } = null;

        public ISoundPlayer soundPlayer { get; private set; }
        public AudioClip bgmClip { get; private set; }

        public bool isEditor { get; private set; }
        public IMode[] modes { get; private set; }

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

        public void Refresh(SDJKMapFile map, SDJKReplayFile replay, SDJKRuleset ruleset, bool isEditor, IMode[] modes)
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
                effectManager.selectedMap = map;
                effectManager.AllRefresh();

                for (int i = 0; i < map.effect.fieldEffect.Count; i++)
                {
                    PlayField playField = (PlayField)ObjectPoolingSystem.ObjectCreate("ruleset.sdjk.play_field", transform).monoBehaviour;
                    playField.Refresh(i);
                }

                RhythmManager.Play(map.globalEffect.bpm, map.info.songOffset, map.globalEffect.yukiMode, null, 3);
                BGMPlay().Forget();
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
