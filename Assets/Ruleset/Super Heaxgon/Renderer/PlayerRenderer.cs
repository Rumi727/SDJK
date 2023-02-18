using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.Renderer
{
    public sealed class PlayerRenderer : PolygonRendererBase
    {
        [SerializeField] float _rotation = 0; public float rotation { get => _rotation; set => _rotation = value; }
        public PlayerVector2 playerVector2 => this.GetPlayerColliderPos();

        public override void Render() => this.PlayerGLRender();
    }
}
