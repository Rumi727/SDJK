using SDJK.Ruleset.SDJK.Effect;
using SDJK.Ruleset.SuperHexagon.Renderer;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon
{
    public sealed class BackgroundColorEffect : SuperHexagonEffect
    {
        [SerializeField] Field field;
        [SerializeField] BackgroundColorRenderer background;

        protected override void RealUpdate()
        {
            background.sides = (float)field.sides;

            background.color = field.backgroundColor;
            background.colorAlt = field.backgroundColorAlt;
        }
    }
}
