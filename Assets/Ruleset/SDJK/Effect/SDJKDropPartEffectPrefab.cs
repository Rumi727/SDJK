using SCKRM;
using SCKRM.Rhythm;
using SCKRM.UI;
using SDJK.Effect;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Ruleset.SDJK.Effect
{
    public class SDJKDropPartEffectPrefab: DropPartEffectPrefabParent
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        SDJKDropPartEffect sdjkDropPartEffect;

        protected override void Update()
        {
            base.Update();

            if (dropPartEffect == null)
                return;

            if (sdjkDropPartEffect == null)
                sdjkDropPartEffect = (SDJKDropPartEffect)dropPartEffect;

            Bar bar = sdjkDropPartEffect.bar;
            Vector3 dir;
            if (isLeft)
                dir = Vector3.left;
            else
                dir = Vector3.right;

            Vector3 parentPos = transform.parent.position;
            if (dropPart && !Physics.Raycast(parentPos, dir, dropPartEffect.width + Bar.barWidthWithoutBoardHalf))
            {
                Color color = bar.barEffectFile.color.GetValue(RhythmManager.currentBeatScreen);
                double div = offsetCurrentBeatReapeat / dropPartEffect.count;

                if (isLeft)
                    transform.localPosition = new Vector2(-(float)(dropPartEffect.width * (float)div) - Bar.barWidthWithoutBoardHalf, 0);
                else
                    transform.localPosition = new Vector2((float)(dropPartEffect.width * (float)div) + Bar.barWidthWithoutBoardHalf, 0);

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
