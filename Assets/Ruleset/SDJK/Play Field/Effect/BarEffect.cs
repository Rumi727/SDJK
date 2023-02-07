using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.Input;
using SCKRM.Rhythm;
using SDJK.Ruleset.SDJK.Input;
using SDJK.Ruleset.SDJK.Judgement;
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
        [SerializeField] Transform spriteMask;
        [SerializeField] BoxCollider boxCollider;
        [SerializeField] float backgroundAlpha = 0.8f;

        PlayField playField => bar.playField;
        SDJKJudgementManager judgementManager => SDJKJudgementManager.instance;

        async UniTaskVoid Awake()
        {
            if (await UniTask.WaitUntil(() => judgementManager != null, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow())
                return;

            judgementManager.pressAction[bar.barIndex] += Press;
            judgementManager.pressUpAction[bar.barIndex] += PressUp;
        }

        async UniTaskVoid OnDestroy()
        {
            if (await UniTask.WaitUntil(() => judgementManager != null, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow())
                return;

            judgementManager.pressAction[bar.barIndex] -= Press;
            judgementManager.pressUpAction[bar.barIndex] -= PressUp;
        }

        void Press()
        {
            inputColor = new Color(0.2f, 0.2f, 0.2f);
            transform.SetAsLastSibling();
            isKeyEnable = true;
        }

        void PressUp() => isKeyEnable = false;

        bool isKeyEnable = false;
        protected override void RealUpdate()
        {
            if (effectManager == null)
                effectManager = bar.effectManager;
            
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
            transform.localEulerAngles = bar.barEffectFile.rotation.GetValue(RhythmManager.currentBeatScreen);
        }

        void SizeUpdate()
        {
            float fieldHeight = (float)playField.fieldHeight;
            backgroundSpriteRenderer.size = new Vector2(Bar.barWidth, fieldHeight);
            spriteRenderer.size = new Vector2(Bar.barWidth, fieldHeight);
            spriteMask.localScale = new Vector3(spriteMask.localScale.x, fieldHeight, spriteMask.localScale.z);
            boxCollider.size = new Vector3(Bar.barWidth, fieldHeight, 1);

            key.localPosition = new Vector3(0, -(fieldHeight * 0.5f) + Bar.barBottomKeyHeightHalf);
            transform.localScale = bar.barEffectFile.scale.GetValue(RhythmManager.currentBeatScreen);
        }

        Color inputColor = Color.white;
        void ColorUpdate()
        {
            inputColor = inputColor.MoveTowards(Color.white, 0.075f * Kernel.fpsDeltaTime);
            if (isKeyEnable)
                inputColor = new Color(0.2f, 0.2f, 0.2f);

            Color color = inputColor * bar.barEffectFile.color.GetValue(RhythmManager.currentBeatScreen);

            backgroundSpriteRenderer.color = new Color(0, 0, 0, color.a * backgroundAlpha);
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
