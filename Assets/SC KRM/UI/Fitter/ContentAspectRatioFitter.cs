using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.UI
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class ContentAspectRatioFitter : AspectRatioFitter, IUI
    {
        Canvas _canvas; public Canvas canvas => _canvas = this.GetComponentInParentFieldSave(_canvas, true);
        CanvasSetting _canvasSetting; public CanvasSetting canvasSetting => _canvasSetting = this.GetComponentInParentFieldSave(_canvasSetting, true);

        [SerializeField] RectTransform _parentRectTransform; public RectTransform parentRectTransform
        {
            get
            {
                if (_parentRectTransform == null || _parentRectTransform.gameObject != transform.parent.gameObject)
                    _parentRectTransform = transform.parent as RectTransform;

                return _parentRectTransform;
            }
        }
        [SerializeField] RectTransform _rectTransform; public RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null || _rectTransform.gameObject != gameObject)
                {
                    _rectTransform = transform as RectTransform;
                    if (_rectTransform == null)
                        _rectTransform = gameObject.AddComponent<RectTransform>();
                }

                return _rectTransform;
            }
        }
        [SerializeField] RectTransformTool _rectTransformTool; public RectTransformTool rectTransformTool => _rectTransformTool = this.GetComponentFieldSave(_rectTransformTool);

        [SerializeField] Graphic _graphic; public Graphic graphic => _graphic = this.GetComponentFieldSave(_graphic, ComponentUtility.GetComponentMode.none);



        [SerializeField] RectTransform _targetRectTransform; public RectTransform targetRectTransform => _targetRectTransform;



        protected override void Update()
        {
            base.Update();

            RectTransform targetRectTransform = this.targetRectTransform;
            if (targetRectTransform == null)
                targetRectTransform = rectTransform;

            float xSize = LayoutUtility.GetPreferredSize(targetRectTransform, 0);
            float ySize = LayoutUtility.GetPreferredSize(targetRectTransform, 1);

            float aspectRatio = xSize / ySize;
            if (float.IsNormal(aspectRatio))
                this.aspectRatio = xSize / ySize;
            else
                this.aspectRatio = 1;
        }
    }
}
