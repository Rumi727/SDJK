using SCKRM;
using SCKRM.Rhythm;
using SDJK.Ruleset.SDJK.Effect;
using SDJK.Ruleset.SDJK.Judgement;
using SDJK.Ruleset.SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public class NoteEffect : SDJKEffect
    {
        [SerializeField] Note note;
        [SerializeField] Transform holdNote;
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] SpriteRenderer holdNoteSpriteRenderer;

        Bar bar => note.bar;

        double beat => note.beat;
        double holdLength => note.holdLength;
        NoteTypeFile type => note.type;

        public override void Refresh(bool force = false) { }

        void Update()
        {
            if (effectManager == null)
                effectManager = note.effectManager;

            if (PosUpdate())
                return;

            ColorUpdate();
        }

        bool PosUpdate()
        {
            float y = (float)beat;
            float holdYPos = 0;
            float holdYSize = (float)holdLength;
            float noteDis = (float)bar.noteDistance;

            double lastJudgementBeat = SDJKJudgementManager.instance.lastJudgementBeat[bar.barIndex];
            if (beat + holdLength <= lastJudgementBeat)
            {
                note.Remove();
                return true;
            }
            else if (beat <= lastJudgementBeat)
            {
                float cutY = (float)(RhythmManager.currentBeatScreen - beat);

                y += cutY;
                holdYSize -= cutY;
            }

            transform.localPosition = new Vector3(0, y * noteDis, 0);
            holdNote.localPosition = new Vector3(0, holdYPos * noteDis, 0);
            holdNote.localScale = new Vector3(1, (holdYSize * noteDis).Clamp(0), 1);

            return false;
        }

        void ColorUpdate()
        {
            Color color = bar.barEffectFile.noteColor.GetValue(RhythmManager.currentBeatSound);
            if (type == NoteTypeFile.instantDeath)
                color = Color.red;

            spriteRenderer.color = color;
            holdNoteSpriteRenderer.color = color;
        }
    }
}
