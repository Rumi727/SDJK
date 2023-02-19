using SDJK.Ruleset.SuperHexagon.Renderer;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.Effect
{
    public sealed class PlayerEffect : SuperHexagonEffect
    {
        [SerializeField] Field field;

        [SerializeField] RegularPolygonRenderer playerBackground;
        [SerializeField] RegularPolygonRenderer playerBorder;
        [SerializeField] PlayerRenderer player;

        protected override void RealUpdate()
        {
            playerBackground.color = field.backgroundColor;
            playerBorder.color = field.mainColor;
            player.color = field.mainColor;
        }
    }
}
