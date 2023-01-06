using SCKRM.Rhythm;
using SDJK.Ruleset.SDJK.Effect;
using SDJK.Map.Ruleset.SDJK.Map;
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
        SDJKNoteTypeFile type => note.type;

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
            if (type == SDJKNoteTypeFile.instantDeath)
                color = new Color(1, 0, 0, color.a);
            else if (type == SDJKNoteTypeFile.auto)
                color.a *= 0.5f;

            spriteRenderer.color = color;
            holdNoteSpriteRenderer.color = color;
        }
    }
}
