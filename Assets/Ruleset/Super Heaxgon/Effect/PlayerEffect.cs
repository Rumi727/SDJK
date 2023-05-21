using SCKRM;
using SDJK.Ruleset.SuperHexagon.Renderer;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.Effect
{
    public sealed class PlayerEffect : SuperHexagonEffect
    {
        [SerializeField, FieldNotNull] Field field;

        [SerializeField, FieldNotNull] RegularPolygonRenderer playerBackground;
        [SerializeField, FieldNotNull] RegularPolygonRenderer playerBorder;
        [SerializeField, FieldNotNull] PlayerRenderer player;

        protected override void RealUpdate()
        {
            playerBackground.color = field.backgroundColor;
            playerBorder.color = field.mainColor;
            player.color = field.mainColor;
        }
    }
}
