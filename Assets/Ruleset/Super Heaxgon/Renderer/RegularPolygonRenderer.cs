using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.Renderer
{
    public class RegularPolygonRenderer : PolygonRendererBase
    {
        [SerializeField] float _sides = 6; public float sides { get => _sides; set => _sides = value; }
        [SerializeField] float _min = 1; public float min { get => _min; set => _min = value; }

        public override void Render() => this.RegularPolygonGLRender();
    }
}
