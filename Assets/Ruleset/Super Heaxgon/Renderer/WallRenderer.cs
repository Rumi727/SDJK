using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.Renderer
{
    public sealed class WallRenderer : RegularPolygonRenderer
    {
        [SerializeField] int _index = 0; public int index { get => _index; set => _index = value; }
        public WallVector2 wallVector2 => this.GetWallVector2();

        public override void Render() => this.WallGLRender();
    }
}
