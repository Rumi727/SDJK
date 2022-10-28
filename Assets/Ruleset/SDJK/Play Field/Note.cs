using SCKRM.Object;
using SDJK.Effect;
using SDJK.Ruleset.SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public sealed class Note : ObjectPooling
    {
        public Bar bar { get; private set; }
        public int barIndex => bar.barIndex;

        public double beat { get; private set; }
        public double holdLength { get; private set; }

        public SDJKMapFile map => bar.map;
        public EffectManager effectManager => bar.effectManager;

        [SerializeField] SpriteRenderer _spriteRenderer; public SpriteRenderer spriteRenderer => _spriteRenderer;

        [SerializeField] Transform _holdNote; public Transform holdNote => _holdNote;
        [SerializeField] SpriteRenderer _holdNoteSpriteRenderer; public SpriteRenderer holdNoteSpriteRenderer => _holdNoteSpriteRenderer;

        public void Refresh(Bar bar, double beat, double holdLength)
        {
            this.bar = bar;
            this.beat = beat;
            this.holdLength = holdLength;

            PosUpdate();
        }

        void PosUpdate()
        {
            float y = (float)beat;
            float holdY = (float)holdLength;

            transform.localPosition = new Vector3(0, y, 0);
            holdNote.localScale = new Vector3(1, holdY, 1);
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            bar = null;
            beat = 0;
            holdLength = 0;

            transform.localPosition = Vector3.zero;
            holdNote.localScale = new Vector3(1, 0, 1);

            return true;
        }
    }
}
