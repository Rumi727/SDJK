using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.UI
{
    [ExecuteAlways]
    [AddComponentMenu("SC KRM/UI/Color/Color Readability"), RequireComponent(typeof(Graphic))]
    public sealed class ColorReadability : UIAni
    {
        [SerializeField] CanvasRenderer _targetCanvasRenderer; public CanvasRenderer targetCanvasRenderer => _targetCanvasRenderer;
        [SerializeField] Graphic _targetGraphic; public Graphic targetGraphic => _targetGraphic;

        Color color = Color.white;
        void Update()
        {
            if (targetCanvasRenderer != null && targetGraphic != null && graphic != null && targetCanvasRenderer != graphic)
                color = GetReadbilityColor(targetGraphic.color * targetCanvasRenderer.GetColor());

            if (!lerp || !Kernel.isPlaying)
                graphic.color = color;
            else
                graphic.color = graphic.color.Lerp(color, lerpValue * Kernel.fpsUnscaledDeltaTime);
        }

        public static Color GetReadbilityColor(float color)
        {
            if (color <= 0.5f)
                return Color.white;
            else
                return Color.black;
        }

        public static Color GetReadbilityColor(Color color)
        {
            float average = (color.r + color.g + color.b) / 3;

            if (average <= 0.5f)
                return Color.white;
            else
                return Color.black;
        }
    }
}