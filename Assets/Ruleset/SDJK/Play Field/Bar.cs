using SCKRM.Input;
using SCKRM.Object;
using SCKRM.Rhythm;
using SDJK.Effect;
using SDJK.Ruleset.SDJK.Effect;
using SDJK.Ruleset.SDJK.Map;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public sealed class Bar : ObjectPooling
    {
        public const float barWidth = 2.5f;
        public const float barWidthWithoutBoard = 2.25f;
        public const float barWidthWithoutBoardHalf = 1.125f;
        public const float barBoardWidth = 0.25f;

        public const float barTopWithoutBoard = 7.75f;
        public const float barBottomWithoutKeyAndBoard = -5.5f;

        [SerializeField] BarEffect _barEffect; public BarEffect barEffect => _barEffect;
        [SerializeField] Transform _notes; public Transform notes => _notes;
        [SerializeField] TMP_Text _keyText; public TMP_Text keyText => _keyText;

        public SDJKMapFile map => (SDJKMapFile)effectManager.selectedMap;
        public EffectManager effectManager { get; private set; }

        public int barIndex { get; private set; }

        public override void OnCreate() => barEffect.effectManager = effectManager;

        void Update() => notes.localPosition = new Vector3(0, (float)-RhythmManager.currentBeatScreen + barBottomWithoutKeyAndBoard);

        public void Refresh(EffectManager effectManager, int barIndex)
        {
            this.effectManager = effectManager;
            this.barIndex = barIndex;

            const string inputKeyOr = "ruleset.sdjk.{0}.{1}";
            string inputKey = inputKeyOr.Replace("{0}", map.notes.Count.ToString()).Replace("{1}", barIndex.ToString());

            if (InputManager.controlSettingList.TryGetValue(inputKey, out List<KeyCode> value) && value.Count > 0)
                keyText.text = value[0].ToString();

            NoteRefresh();
        }

        List<Note> createdNotes = new List<Note>();
        void NoteAllRemove()
        {
            for (int i = 0; i < createdNotes.Count; i++)
                createdNotes[i].Remove();

            createdNotes.Clear();
        }

        void NoteRefresh()
        {
            NoteAllRemove();

            if (map.notes.Count > barIndex)
            {
                List<NoteFile> noteFiles = map.notes[barIndex];
                for (int i = 0; i < noteFiles.Count; i++)
                {
                    NoteFile noteFile = noteFiles[i];
                    Note note = (Note)ObjectPoolingSystem.ObjectCreate("ruleset.sdjk.play_field.bar.note", notes).monoBehaviour;

                    note.Refresh(this, noteFile.beat, noteFile.holdLength);
                    createdNotes.Add(note);
                }
            }
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            effectManager = null;
            keyText.text = "";

            NoteAllRemove();
            return true;
        }
    }
}
