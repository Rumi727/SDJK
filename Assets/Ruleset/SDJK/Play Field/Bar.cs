using SCKRM;
using SCKRM.Input;
using SCKRM.Object;
using SCKRM.Rhythm;
using SDJK.Effect;
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

        public const float barBottomKeyHeight = 2.5f;
        public const float barBottomKeyHeightHalf = 1.25f;

        [SerializeField] Transform _notes; public Transform notes => _notes;
        [SerializeField] TMP_Text _keyText; public TMP_Text keyText => _keyText;
        [SerializeField] Transform _spriteMask; public Transform spriteMask => _spriteMask;

        public PlayField playField { get; private set; }

        public SDJKMapFile map => (SDJKMapFile)effectManager.selectedMap;
        public EffectManager effectManager => SDJKManager.instance.effectManager;

        public BarEffectFile barEffectFile { get; private set; }
        public int barIndex { get; private set; }
        public double noteDistance { get; private set; }

        List<Note> createdNotes = new List<Note>();

        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            double currentBeat = RhythmManager.currentBeatScreen;
            double globalNoteDistance = map.effect.globalNoteDistance.GetValue(currentBeat);
            double fieldNoteDistance = playField.fieldEffectFile.noteDistance.GetValue(currentBeat);
            double localNoteDistance = barEffectFile.noteDistance.GetValue(currentBeat);

            noteDistance = globalNoteDistance * fieldNoteDistance * localNoteDistance;
            NotePosAndHideUpdate();
        }

        void NotePosAndHideUpdate()
        {
            for (int i = 0; i < createdNotes.Count; i++)
            {
                Note note = createdNotes[i];
                if (note == null || note.isRemoved)
                {
                    createdNotes.RemoveAt(i);
                    i--;

                    continue;
                }

                double y = note.GetYPos(note.GetNoteDis(), out double holdYSize, out bool allowRemove);
                if (allowRemove)
                {
                    note.Remove();
                    createdNotes.RemoveAt(i);
                    i--;

                    continue;
                }

                double fieldHeight = playField.fieldHeight * 0.5;
                bool top = y <= fieldHeight - barBottomKeyHeight;
                bool bottom = y + holdYSize.Max(Note.noteYSize) >= -fieldHeight - barBottomKeyHeight;
                bool active = top && bottom;

                if ((top && bottom) != note.gameObject.activeSelf)
                    note.gameObject.SetActive(top && bottom);

                if (active)
                    note.PosAndHoldScaleUpdate(y, holdYSize);
            }
        }

        public void Refresh(PlayField playField, int barIndex)
        {
            this.playField = playField;
            this.barIndex = barIndex;

            barEffectFile = playField.fieldEffectFile.barEffect[barIndex];

            const string inputKeyOr = "ruleset.sdjk.{0}.{1}";
            string inputKey = inputKeyOr.Replace("{0}", map.notes.Count.ToString()).Replace("{1}", barIndex.ToString());

            if (InputManager.controlSettingList.TryGetValue(inputKey, out List<KeyCode> value) && value.Count > 0)
                keyText.text = value[0].ToString();

            NoteRefresh();
        }

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

                    note.Refresh(this, noteFile, i);
                    createdNotes.Add(note);
                }
            }
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            keyText.text = "";

            NoteAllRemove();
            return true;
        }
    }
}
