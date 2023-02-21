using SCKRM;
using SDJK.Effect;
using SDJK.Ruleset.SuperHexagon.Renderer;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.Effect
{
    public class SuperHexagonYukiModeEffectPrefab : YukiModeEffectPrefabParent
    {
        [SerializeField, NotNull] RegularPolygonRenderer regularPolygonRenderer;
        SuperHexagonYukiModeEffect superHexagonYukiModeEffect => (SuperHexagonYukiModeEffect)yukiModeEffect;

        public override void Refresh(YukiModeEffect yukiModeEffect, int indexOffset, bool isLeft)
        {
            base.Refresh(yukiModeEffect, indexOffset, isLeft);
            superHexagonYukiModeEffect.glRenderInvoker.yukiModeRendererBases.Add(regularPolygonRenderer);
        }

        protected override void Update()
        {
            base.Update();

            if (yukiModeEffect == null)
                return;

            Field field = superHexagonYukiModeEffect.field;
            if (yukiMode)
            {
                Color color = field.mainColor;
                double div = offsetCurrentBeatReapeat / yukiModeEffect.count;

                regularPolygonRenderer.distance = superHexagonYukiModeEffect.regularPolygonRenderer.distance + ((float)div * superHexagonYukiModeEffect.width);

                regularPolygonRenderer.color = new Color(color.r, color.g, color.b, (float)(1 - div).Clamp01());
                regularPolygonRenderer.sides = (float)field.sides;
            }
            else
            {
                regularPolygonRenderer.distance = superHexagonYukiModeEffect.regularPolygonRenderer.distance;
                
                regularPolygonRenderer.color = new Color(0, 0, 0, 0);
                regularPolygonRenderer.sides = (float)field.sides;
            }
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            regularPolygonRenderer.distance = 0;

            regularPolygonRenderer.color = Color.clear;
            regularPolygonRenderer.sides = 6;

            if (superHexagonYukiModeEffect != null)
                superHexagonYukiModeEffect.glRenderInvoker.polygonRendererBases.Remove(regularPolygonRenderer);

            return true;
        }
    }
}
