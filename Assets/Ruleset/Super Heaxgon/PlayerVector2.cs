using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon
{
    public struct PlayerVector2
    {
        public Vector2 top;
        public Vector2 bottom;

        public PlayerVector2(Vector2 top, Vector2 bottom)
        {
            this.top = top;
            this.bottom = bottom;
        }
    }
}
