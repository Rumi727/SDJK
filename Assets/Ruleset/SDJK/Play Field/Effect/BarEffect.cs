using SCKRM;
using SCKRM.Input;
using SCKRM.Rhythm;
using SDJK.Ruleset.SDJK.Input;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.Effect
{
    public sealed class BarEffect : SDJKEffect
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] SpriteRenderer backgroundSpriteRenderer;
        [SerializeField] SpriteRenderer keySpriteRenderer;
        [SerializeField] TextMeshPro keyText;
        [SerializeField] Bar bar;
        [SerializeField] Transform key;
        [SerializeField] int sortingOrder = 1000;

        PlayField playField => bar.playField;
        SDJKInputManager inputManager => SDJKInputManager.instance;

        public override void Refresh(bool force = false) { }

        bool isKeyEnable = false;
        void Update()
        {
            if (effectManager == null)
                effectManager = bar.effectManager;

            isKeyEnable = inputManager.GetKey(bar.barIndex, InputType.Alway);
            if (inputManager.GetKey(bar.barIndex, InputType.Down))
                transform.SetAsLastSibling();

            PosUpdate();
            SizeUpdate();
            ColorUpdate();
        }

        void LateUpdate() => SortingOrderUpdate();

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

        Color inputColor = Color.white;
        void ColorUpdate()
        {
            inputColor = inputColor.MoveTowards(Color.white, 0.075f * Kernel.fpsDeltaTime);
            if (isKeyEnable)
                inputColor = new Color(0.2f, 0.2f, 0.2f);

            Color color = inputColor * bar.barEffectFile.color.GetValue(RhythmManager.currentBeatSound);

            spriteRenderer.color = color;
            keySpriteRenderer.color = color;
            keyText.color = color;
        }

        void SortingOrderUpdate()
        {
            int order = sortingOrder + transform.GetSiblingIndex();

            spriteRenderer.sortingOrder = order;
            keySpriteRenderer.sortingOrder = order;
            keyText.sortingOrder = order;
        }
    }
}
