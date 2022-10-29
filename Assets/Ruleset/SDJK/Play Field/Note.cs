using SCKRM.Object;
using SDJK.Effect;
using SDJK.Ruleset.SDJK.Map;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public sealed class Note : ObjectPooling
    {
        public Bar bar { get; private set; }
        public int barIndex => bar.barIndex;

        public NoteFile noteFile { get; private set; }

        public double beat => noteFile.beat;
        public double holdLength => noteFile.holdLength;
        public NoteTypeFile type => noteFile.type;

        public SDJKMapFile map => bar.map;
        public EffectManager effectManager => bar.effectManager;

        [SerializeField] SpriteRenderer _spriteRenderer; public SpriteRenderer spriteRenderer => _spriteRenderer;

        [SerializeField] Transform _holdNote; public Transform holdNote => _holdNote;
        [SerializeField] SpriteRenderer _holdNoteSpriteRenderer; public SpriteRenderer holdNoteSpriteRenderer => _holdNoteSpriteRenderer;


        void Update()
        {
            if (bar != null)
                PosUpdate();
        }

        public void Refresh(Bar bar, NoteFile noteFile)
        {
            this.bar = bar;
            this.noteFile = noteFile;

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
            if (type == NoteTypeFile.normal)
                spriteRenderer.color = Color.green;
            else if (type == NoteTypeFile.instantDeath)
                spriteRenderer.color = Color.red;
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            bar = null;
            noteFile = default;

            transform.localPosition = Vector3.zero;
            holdNote.localScale = new Vector3(1, 0, 1);

            return true;
        }
    }
}
