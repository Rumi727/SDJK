using SCKRM.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SCKRM
{
    [ExecuteAlways, AddComponentMenu("SC KRM/UI/Resoluiton Limit"), RequireComponent(typeof(CanvasScaler))]
    public sealed class ResolutionLimit : UI.UI
    {
        CanvasScaler _canvasScaler; public CanvasScaler canvasScaler => _canvasScaler = this.GetComponentFieldSave(_canvasScaler);

        [SerializeField, Min(0)] Vector2 _min = Vector2.zero; public Vector2 min => _min;
        [SerializeField, Min(0)] Vector2 _max = Vector2.zero; public Vector2 max => _max;
        [SerializeField, Min(0)] float _guiSize = 1; public float guiSize => _guiSize;

        protected override void Awake() => canvasSetting.customGuiSize = true;

        protected override void OnEnable() => Canvas.preWillRenderCanvases += Refresh;
        protected override void OnDisable() => Canvas.preWillRenderCanvases -= Refresh;

        void Refresh()
        {
            canvasSetting.customGuiSize = true;

            float maxX = max.x;
            float maxY = max.y;

            if (maxX <= 0)
                maxX = float.MaxValue;
            if (maxY <= 0)
                maxY = float.MaxValue;

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            canvasScaler.referenceResolution = new Vector2((ScreenManager.width / (UIManager.currentGuiSize * guiSize)).Clamp(min.x, maxX), (ScreenManager.height / (UIManager.currentGuiSize * guiSize)).Clamp(min.y, maxY));
        }
    }
}
