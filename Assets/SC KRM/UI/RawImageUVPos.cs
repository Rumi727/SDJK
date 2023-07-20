using SCKRM.UI;
using UnityEngine.UI;
using UnityEngine;

namespace SCKRM
{
    [ExecuteAlways]
    public class RawImageUVPos : UIBase
    {
        RawImage _rawImage; public RawImage rawImage => _rawImage = this.GetComponentFieldSave(_rawImage, ComponentUtility.GetComponentMode.none);

        [SerializeField] Vector2 _position = Vector2.zero; public Vector2 position { get => _position; set => _position = value; }
        [SerializeField] Vector2 _uvOffset = Vector2.zero; public Vector2 uvOffset { get => _uvOffset; set => _uvOffset = value; }

        protected virtual void Update()
        {
            if (rawImage == null || rawImage.texture == null)
                return;

            Vector2 canvasSize = canvas.pixelRect.size;
            Vector2 size = rectTransform.rect.size * transform.localScale;
            Rect rect = rawImage.uvRect;

            rect.x = ((-((float)0).InverseLerpUnclamped(canvasSize.x, position.x) * (canvasSize.x / size.x) * rect.width) + uvOffset.x).Repeat(1);
            rect.y = ((-((float)0).InverseLerpUnclamped(canvasSize.y, position.y) * (canvasSize.y / size.y) * rect.height) + uvOffset.y).Repeat(1);

            rawImage.uvRect = rect;
        }
    }
}
