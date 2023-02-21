using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Ruleset.SuperHexagon.Effect
{
    public sealed class GraphicColorEffect : SuperHexagonEffect
    {
        [SerializeField, NotNull] Graphic graphic;
        [SerializeField, NotNull] Field field;
        [SerializeField] bool backgroundColor = false;
        [SerializeField] bool altColor = false;

        protected override void RealUpdate()
        {
            if (backgroundColor)
            {
                if (altColor)
                    graphic.color = field.backgroundColorAlt;
                else
                    graphic.color = field.backgroundColor;
            }
            else
            {
                if (altColor)
                    graphic.color = field.mainColorAlt;
                else
                    graphic.color = field.mainColor;
            }
        }
    }
}
