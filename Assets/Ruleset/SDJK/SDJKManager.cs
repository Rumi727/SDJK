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
using SDJK.Ruleset.SDJK.Map;
using System.IO;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public sealed class SDJKManager : MonoBehaviour
    {
        [SerializeField] EffectManager _effectManager; public EffectManager effectManager => _effectManager;

        public SDJKMapFile map { get; private set; }

        public ISoundPlayer soundPlayer { get; private set; }
        public AudioClip bgmClip { get; private set; }

        void Awake() => UIManager.BackEventAdd(MainMenuLoad.Load);

        void Update()
        {
            if (soundPlayer != null)
                soundPlayer.volume = (float)map.globalEffect.volume.GetValue();
        }

        void OnDestroy()
        {
            UIManager.BackEventRemove(MainMenuLoad.Load);

            if (bgmClip != null)
                Destroy(bgmClip);
        }

        public void Refresh(SDJKMapFile map)
        {
            this.map = map;
            effectManager.selectedMap = map;
            effectManager.AllRefresh();

            for (int i = 0; i < map.fieldCount; i++)
            {
                PlayField playField = (PlayField)ObjectPoolingSystem.ObjectCreate("ruleset.sdjk.play_field", transform).monoBehaviour;
                playField.Refresh(i, effectManager);
            }

            BGMPlay().Forget();
        }

        async UniTaskVoid BGMPlay()
        {
            string path = PathTool.Combine(map.mapFilePathParent, map.info.songFile);
            if (ResourceManager.FileExtensionExists(path, out string fullPath, ResourceManager.audioExtension))
            {
                bgmClip = await ResourceManager.GetAudio(fullPath, true, true);
                SoundMetaData soundMetaData = ResourceManager.CreateSoundMetaData(1, 1, 0, bgmClip);
                SoundData<SoundMetaData> soundData = ResourceManager.CreateSoundData("", true, soundMetaData);

                soundPlayer = SoundManager.PlaySound(soundData, 1, true);
            }
            else if (File.Exists(path + ".nbs"))
            {
                NBSFile nbsFile = NBSManager.ReadNBSFile(path + ".nbs");
                NBSMetaData nbsMetaData = ResourceManager.CreateNBSMetaData(1, 1, nbsFile);
                SoundData<NBSMetaData> soundData = ResourceManager.CreateSoundData("", true, nbsMetaData);

                soundPlayer = SoundManager.PlayNBS(soundData, 1, true);
            }

            effectManager.soundPlayer = soundPlayer;
            RhythmManager.Play(map.globalEffect.bpm, map.info.songOffset, map.globalEffect.dropPart, soundPlayer);
        }
    }
}
