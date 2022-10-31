using SCKRM.Rhythm;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.Effect
{
    public sealed class BarEffect : SDJKEffect
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] SpriteRenderer backgroundSpriteRenderer;
        [SerializeField] SpriteRenderer keySpriteRenderer;
        [SerializeField] TMP_Text keyText;
        [SerializeField] Bar bar;
        [SerializeField] Transform key;

        PlayField playField => bar.playField;

        public override void Refresh(bool force = false) { }

        void Update()
        {
            if (effectManager == null)
                effectManager = bar.effectManager;

            PosUpdate();
            SizeUpdate();
            ColorUpdate();
        }

        void PosUpdate()
        {
            int index = bar.barIndex;
            float x = -Bar.barWidthWithoutBoardHalf * (map.notes.Count - 1);
            x += Bar.barWidthWithoutBoard * index;

            transform.localPosition = new Vector3(x, 0) + bar.barEffectFile.pos.GetValue(RhythmManager.currentBeatScreen);
        }

        void SizeUpdate()
        {
            backgroundSpriteRenderer.size = new Vector2(Bar.barWidth, (float)playField.fieldHeight);
            spriteRenderer.size = new Vector2(Bar.barWidth, (float)playField.fieldHeight);

            key.localPosition = new Vector3(0, (float)(-(playField.fieldHeight * 0.5f) + Bar.barBottomKeyHeightHalf));
        }

        void ColorUpdate()
        {
            Color color = bar.barEffectFile.color.GetValue(RhythmManager.currentBeatSound);

            spriteRenderer.color = color;
            keySpriteRenderer.color = color;
            keyText.color = color;
        }
    }
}
