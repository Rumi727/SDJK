using SCKRM.Rhythm;
using SDJK.Ruleset.SDJK.Effect;
using SDJK.Map.Ruleset.SDJK.Map;
using UnityEngine;
using SDJK.Effect;

namespace SDJK.Ruleset.SDJK
{
    public class NoteEffect : SDJKEffect
    {
        public override EffectManager effectManager => bar.effectManager;

        [SerializeField] Note note;
        [SerializeField] Transform holdNote;
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] SpriteRenderer holdNoteSpriteRenderer;

        Bar bar => note.bar;
        SDJKNoteTypeFile type => note.type;

        protected override void RealUpdate() => ColorUpdate();

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
