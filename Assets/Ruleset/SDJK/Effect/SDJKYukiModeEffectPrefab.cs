using SCKRM.Rhythm;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.Effect
{
    public class SDJKYukiModeEffectPrefab: YukiModeEffectPrefabParent
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        SDJKYukiModeEffect sdjkYukiModeEffect;

        protected override void Update()
        {
            base.Update();

            if (yukiModeEffect == null)
                return;

            if (sdjkYukiModeEffect == null)
                sdjkYukiModeEffect = (SDJKYukiModeEffect)yukiModeEffect;

            Bar bar = sdjkYukiModeEffect.bar;
            Vector3 dir;
            if (isLeft)
                dir = Vector3.left;
            else
                dir = Vector3.right;

            Vector3 parentPos = transform.parent.position;
            if (yukiMode && !Physics.Raycast(parentPos, dir, yukiModeEffect.width + Bar.barWidthWithoutBoardHalf))
            {
                Color color = bar.barEffectFile.color.GetValue(RhythmManager.currentBeatScreen);
                double div = offsetCurrentBeatReapeat / yukiModeEffect.count;

                if (isLeft)
                    transform.localPosition = new Vector2(-(float)(yukiModeEffect.width * (float)div) - Bar.barWidthWithoutBoardHalf, 0);
                else
                    transform.localPosition = new Vector2((float)(yukiModeEffect.width * (float)div) + Bar.barWidthWithoutBoardHalf, 0);

                spriteRenderer.size = new Vector2(Bar.barBoardWidth, (float)bar.playField.fieldHeight);
                spriteRenderer.color = new Color(color.r, color.g, color.b, (float)(1 - div));
            }
            else
                spriteRenderer.color = new Color(0, 0, 0, 0);
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            transform.localPosition = Vector2.zero;
            spriteRenderer.color = Color.clear;

            return true;
        }
    }
}
