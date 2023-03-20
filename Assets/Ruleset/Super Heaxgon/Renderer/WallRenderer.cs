using SCKRM;
using SDJK.Effect;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.Renderer
{
    public sealed class WallRenderer : RegularPolygonRenderer
    {
        [SerializeField] Color _colorAlt = Color.white; public Color colorAlt { get => _colorAlt; set => _colorAlt = value; }

        [SerializeField] int _index = 0; public int index { get => _index; set => _index = value; }
        public WallVector2 wallVector2 => this.GetWallVector2();

        public override void Render()
        {
            Color color;
            if (index == sides - 1 && sides % 2 != 0)
                color = this.color.Lerp(colorAlt, 0.5f);
            else if (index % 2 != 0)
                color = colorAlt;
            else
                color = this.color;

            transform.WallGLRender(color, RenderUtility.GetWallVector2(index, distance, width, sides, min));
        }
    }
}
