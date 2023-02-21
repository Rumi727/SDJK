using SCKRM;
using SDJK.Ruleset.SuperHexagon.Renderer;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.Effect
{
    public sealed class BackgroundColorEffect : SuperHexagonEffect
    {
        [SerializeField, NotNull] Field field;
        [SerializeField, NotNull] BackgroundColorRenderer background;

        protected override void RealUpdate()
        {
            background.sides = (float)field.sides;

            background.color = field.backgroundColor;
            background.colorAlt = field.backgroundColorAlt;
        }
    }
}
