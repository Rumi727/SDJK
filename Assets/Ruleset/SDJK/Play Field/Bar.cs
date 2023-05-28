using SCKRM;
using SCKRM.Input;
using SCKRM.Object;
using SCKRM.Renderer;
using SCKRM.Rhythm;
using SDJK.Effect;
using SDJK.Map.Ruleset.SDJK.Map;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public sealed class Bar : ObjectPoolingBase
    {
        public const float barWidth = 2.5f;
        public const float barWidthWithoutBoard = 2.25f;
        public const float barWidthWithoutBoardHalf = 1.125f;
        public const float barBoardWidth = 0.25f;

        public const float barBottomKeyHeight = 2.5f;
        public const float barBottomKeyHeightHalf = 1.25f;

        [SerializeField, FieldNotNull] Transform _notes; public Transform notes => _notes;
        [SerializeField, FieldNotNull] TMP_Text _keyText; public TMP_Text keyText => _keyText;
        [SerializeField, FieldNotNull] Transform _spriteMask; public Transform spriteMask => _spriteMask;

        [SerializeField, FieldNotNull] CustomSpriteRendererBase _backgroundCustomSpriteRendererBase; public CustomSpriteRendererBase backgroundCustomSpriteRendererBase => _backgroundCustomSpriteRendererBase;
        [SerializeField, FieldNotNull] CustomSpriteRendererBase _customSpriteRendererBase; public CustomSpriteRendererBase customSpriteRendererBase => _customSpriteRendererBase;
        [SerializeField, FieldNotNull] CustomSpriteRendererBase _keyCustomSpriteRendererBase; public CustomSpriteRendererBase keyCustomSpriteRendererBase => _keyCustomSpriteRendererBase;

        public PlayField playField { get; private set; }

        public SDJKMapFile map => (SDJKMapFile)effectManager.selectedMap;
        public EffectManager effectManager => SDJKManager.instance.effectManager;

        public SDJKBarEffectFile barEffectFile { get; private set; }
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

            string tag = map.notes.Count + "." + barIndex;
            string inputKey = "ruleset.sdjk." + tag;

            if (InputManager.controlSettingList.TryGetValue(inputKey, out List<KeyCode> value) && value.Count > 0)
                keyText.text = value[0].KeyCodeToString();

            NoteRefresh();

            backgroundCustomSpriteRendererBase.spriteTag = tag;
            customSpriteRendererBase.spriteTag = tag;
            keyCustomSpriteRendererBase.spriteTag = tag;

            for (int i = 0; i < createdNotes.Count; i++)
            {
                Note note = createdNotes[i];

                note.customSpriteRendererBase.spriteTag = tag;
                note.holdNoteCustomSpriteRendererBase.spriteTag = tag;
            }
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
                TypeList<SDJKNoteFile> noteFiles = map.notes[barIndex];
                for (int i = 0; i < noteFiles.Count; i++)
                {
                    SDJKNoteFile noteFile = noteFiles[i];
                    Note note = (Note)ObjectPoolingSystem.ObjectCreate("ruleset.sdjk.play_field.bar.note", notes).monoBehaviour;

                    note.Refresh(this, noteFile, i);
                    createdNotes.Add(note);
                }
            }
        }

        public override void Remove()
        {
            base.Remove();

            keyText.text = "";
            NoteAllRemove();
        }
    }
}
