using SCKRM;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK
{
    public sealed class CameraRenderImage : MonoBehaviour
    {
        [SerializeField, FieldNotNull] RectTransform targetRectTransform;
        [SerializeField, FieldNotNull] RawImage targetRawImage;
        [SerializeField, FieldNotNull] Camera targetCamera;
        [SerializeField] AspectRatioFitter aspectRatioFitter;

        RenderTexture renderTexture;
        CameraClearFlags cameraClearFlags;
        void OnEnable()
        {
            cameraClearFlags = targetCamera.clearFlags;
            Refresh();
        }

        void Update()
        {
            Rect rect = targetRectTransform.rect;
            if (renderTexture.width != rect.width.RoundToInt() || renderTexture.height != rect.height.RoundToInt())
                Refresh();

            if (aspectRatioFitter != null)
                aspectRatioFitter.aspectRatio = (float)renderTexture.width / renderTexture.height;
        }

        void Refresh()
        {
            Destroy();

            Rect rect = targetRectTransform.rect;
            renderTexture = new RenderTexture(rect.width.RoundToInt(), rect.height.RoundToInt(), 0);

            targetCamera.targetTexture = renderTexture;
            targetRawImage.texture = renderTexture;

            targetCamera.clearFlags = CameraClearFlags.SolidColor;
        }

        void Destroy()
        {
            if (renderTexture != null)
            {
                targetCamera.targetTexture = null;
                targetCamera.clearFlags = cameraClearFlags;

                renderTexture.Release();
                Destroy(renderTexture);
            }
        }

        void OnDisable() => Destroy();
    }
}
