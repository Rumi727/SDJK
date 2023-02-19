using SCKRM;
using SDJK.Effect;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.Renderer
{
    public sealed class BackgroundColorRenderer : MonoBehaviour
    {
        [SerializeField] BackgroundEffect backgroundEffect;
        [SerializeField] VideoEffect videoEffect;

        [SerializeField] float _sides = 6; public float sides { get => _sides; set => _sides = value; }
        [SerializeField] float _backgroundImageShowAlpha = 0.5f; public float backgroundImageShowAlpha { get => _backgroundImageShowAlpha; set => _backgroundImageShowAlpha = value; }

        [SerializeField] Color _backgroundColor = new Color(0.125f, 0.125f, 0.125f); public Color color { get => _backgroundColor; set => _backgroundColor = value; }
        [SerializeField] Color _backgroundColorAlt = new Color(0.25f, 0.25f, 0.25f); public Color colorAlt { get => _backgroundColorAlt; set => _backgroundColorAlt = value; }

        public void Render()
        {
            int sides = this.sides.CeilToInt();
            for (int i = 0; i < sides.Clamp(2); i++)
            {
                Color color;
                if (i == sides - 1 && sides % 2 != 0)
                    color = this.color.Lerp(colorAlt, 0.5f);
                else if (i % 2 != 0)
                    color = colorAlt;
                else
                    color = this.color;

                bool backgroundImageShow = backgroundEffect != null && backgroundEffect.background != null && !backgroundEffect.background.isRemoved;
                bool videoShow = videoEffect != null && videoEffect.video != null && !videoEffect.video.isRemoved;

                if (backgroundImageShow || videoShow)
                    color.a *= backgroundImageShowAlpha;

                transform.WallGLRender(color, RenderUtility.GetWallVector2(i, 0, int.MaxValue, this.sides, 0));
            }
        }
    }
}
