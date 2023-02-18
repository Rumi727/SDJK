using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.Renderer
{
    public abstract class PolygonRendererBase : MonoBehaviour
    {
        [SerializeField] float _distance = 0; public float distance { get => _distance; set => _distance = value; }
        [SerializeField] float _width = 1; public float width { get => _width; set => _width = value; }

        [SerializeField] Color _color = Color.white; public virtual Color color { get => _color; set => _color = value; }

        public abstract void Render();
    }
}
