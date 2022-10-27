using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.Effect
{
    public sealed class BarEffect : SDJKEffect
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] Bar bar;

        public override void Refresh(bool force = false) { }

        void Update()
        {
            if (effectManager == null)
                effectManager = bar.effectManager;

            int index = bar.barIndex;
            float x = -Bar.barWidthWithoutBoardHalf * (map.notes.Count - 1);
            x += Bar.barWidthWithoutBoard * index;

            transform.position = new Vector3(x, 0);
        }
    }
}
