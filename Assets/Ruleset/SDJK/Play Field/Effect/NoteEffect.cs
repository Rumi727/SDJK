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
        NoteTypeFile type => note.type;

        public override void Refresh(bool force = false) { }

        protected override void RealUpdate()
        {
            if (effectManager == null)
                effectManager = note.effectManager;

            ColorUpdate();
        }

        void ColorUpdate()
        {
            Color color = bar.barEffectFile.noteColor.GetValue(RhythmManager.currentBeatSound);
            if (type == NoteTypeFile.instantDeath)
                color = new Color(1, 0, 0, color.a);
            else if (type == NoteTypeFile.auto)
                color.a *= 0.5f;

            spriteRenderer.color = color;
            holdNoteSpriteRenderer.color = color;
        }
    }
}
