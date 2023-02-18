using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon
{
    public struct WallVector2
    {
        public Vector2 leftTop;
        public Vector2 rightTop;

        public Vector2 leftBottom;
        public Vector2 rightBottom;

        public WallVector2(Vector2 leftTop, Vector2 rightTop, Vector2 leftBottom, Vector2 rightBottom)
        {
            this.leftTop = leftTop;
            this.rightTop = rightTop;

            this.leftBottom = leftBottom;
            this.rightBottom = rightBottom;
        }
    }
}
