using SCKRM;
using SDJK.Ruleset.SuperHexagon.Renderer;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.Effect
{
    public sealed class BackgroundColorEffect : SuperHexagonEffect
    {
        [SerializeField, FieldNotNull] Field field;
        [SerializeField, FieldNotNull] BackgroundColorRenderer background;

        protected override void RealUpdate()
        {
            background.sides = (float)field.sides;

            if (field.isBackgroundAltReversal)
            {
                background.color = field.backgroundColorAlt;
                background.colorAlt = field.backgroundColor;
            }
            else
            {
                background.color = field.backgroundColor;
                background.colorAlt = field.backgroundColorAlt;
            }
        }
    }
}
