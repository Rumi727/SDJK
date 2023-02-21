using SCKRM;
using SDJK.Ruleset.SuperHexagon.Renderer;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.Effect
{
    public sealed class PlayerEffect : SuperHexagonEffect
    {
        [SerializeField, NotNull] Field field;

        [SerializeField, NotNull] RegularPolygonRenderer playerBackground;
        [SerializeField, NotNull] RegularPolygonRenderer playerBorder;
        [SerializeField, NotNull] PlayerRenderer player;

        protected override void RealUpdate()
        {
            playerBackground.color = field.backgroundColor;
            playerBorder.color = field.mainColor;
            player.color = field.mainColor;
        }
    }
}
