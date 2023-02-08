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
using System.IO;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public sealed class SDJKManager : ManagerBase<SDJKManager>
    {
        [SerializeField] EffectManager _effectManager; public EffectManager effectManager => _effectManager;

        public SDJKRuleset ruleset { get; private set; }
        public SDJKMapFile map { get; private set; }

        public bool isReplay { get; private set; } = false;
        public SDJKReplayFile currentReplay { get; private set; } = null;

        public SDJKReplayFile createdReplay { get; private set; } = new SDJKReplayFile();

        public ISoundPlayer soundPlayer { get; private set; }
        public AudioClip bgmClip { get; private set; }

        public bool isEditor { get; private set; }
        public IMode[] modes { get; private set; }

        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            if (soundPlayer != null)
            {
                soundPlayer.pitch = (float)map.globalEffect.pitch.GetValue(RhythmManager.currentBeatSound);
                soundPlayer.speed = (float)map.globalEffect.tempo.GetValue(RhythmManager.currentBeatSound);
                soundPlayer.volume = (float)map.globalEffect.volume.GetValue(RhythmManager.currentBeatSound);

                //모드
                {
                    IMode speedMode;
                    if ((speedMode = modes.FindMode<FastModeBase>()) != null)
                        soundPlayer.speed *= (float)((FastModeBase.Data)speedMode.modeConfig).speed;
                    else if ((speedMode = modes.FindMode<SlowModeBase>()) != null)
                        soundPlayer.speed *= (float)((SlowModeBase.Data)speedMode.modeConfig).speed;
                }
            }
        }

        void OnDestroy()
        {
            UIManager.BackEventRemove(Quit);

            if (bgmClip != null)
                Destroy(bgmClip, 1);
        }

        public void Refresh(SDJKMapFile map, SDJKReplayFile replay, SDJKRuleset ruleset, bool isEditor, IMode[] modes)
        {
            if (SingletonCheck(this))
            {
                UIManager.BackEventAdd(Quit);

                if (replay != null)
                {
                    isReplay = true;
                    currentReplay = replay;
                }
                else
                {
                    isReplay = false;
                    currentReplay = null;
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

        public void Restart()
        {
            if (bgmClip != null)
                Destroy(bgmClip, 1);

            ruleset.GameStart(map.mapFilePath, isReplay ? currentReplay.replayFilePath : null, isEditor, modes);
            if (!isReplay)
                createdReplay.ReplaySave(map, modes, $"{map.mapFilePath}.{ruleset.name}-lastReplay");
        }

        public void Quit()
        {
            if (bgmClip != null)
                Destroy(bgmClip, 1);

            if (!isReplay)
                createdReplay.ReplaySave(map, modes, $"{map.mapFilePath}.{ruleset.name}-lastReplay");

            MainMenuLoad.Load();
            UIManager.BackEventRemove(Quit);
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
