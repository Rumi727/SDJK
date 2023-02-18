using SDJK.Ruleset.SDJK.Effect;
using SDJK.Ruleset.SuperHexagon.Renderer;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon
{
    public sealed class PlayerEffect : SuperHexagonEffect
    {
        [SerializeField] Field field;

        [SerializeField] RegularPolygonRenderer playerBackground;
        [SerializeField] RegularPolygonRenderer playerBorder;
        [SerializeField] PlayerRenderer player;

        protected override void RealUpdate()
        {
            playerBackground.sides = (float)field.sides;
            playerBorder.sides = (float)field.sides;

            playerBackground.color = field.backgroundColor;
            playerBorder.color = field.mainColor;
            player.color = field.mainColor;

            playerBackground.width = (float)(field.zoom - 0.15);
            playerBorder.distance = (float)(field.zoom - 0.15);
            player.distance = (float)(field.zoom + 0.5);
        }
    }
}
