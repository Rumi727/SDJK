using SDJK.Ruleset.SDJK.Effect;
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

            PosUpdate();
            ColorUpdate();
        }

        void PosUpdate()
        {
            float y = (float)beat;
            float holdY = (float)holdLength;
            float noteDis = (float)bar.noteDistance;

            transform.localPosition = new Vector3(0, y * noteDis, 0);
            holdNote.localScale = new Vector3(1, holdY * noteDis, 1);
        }

        void ColorUpdate()
        {
            Color color = Color.green;
            if (type == NoteTypeFile.instantDeath)
                color = Color.red;

            spriteRenderer.color = color;
            holdNoteSpriteRenderer.color = color;
        }
    }
}
