using UnityEngine;

namespace SCKRM
{
    [ExecuteAlways]
    [AddComponentMenu("SC KRM/UI/Canvas Rendering Camera As Main Camera")]
    public sealed class CanvasRenderingCameraAsMainCamera : MonoBehaviour
    {
        Canvas _canvas; public Canvas canvas => _canvas = this.GetComponentFieldSave(_canvas);

        void Update()
        {
            if (canvas.worldCamera != UnityEngine.Camera.main)
                canvas.worldCamera = UnityEngine.Camera.main;
        }
    }
}
