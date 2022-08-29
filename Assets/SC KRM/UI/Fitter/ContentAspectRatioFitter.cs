using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.UI
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class ContentAspectRatioFitter : AspectRatioFitter, IUI
    {
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

        [SerializeField] Graphic _graphic; public Graphic graphic => _graphic = this.GetComponentFieldSave(_graphic, ComponentTool.GetComponentMode.none);



        protected override void Update()
        {
            base.Update();

            float xSize = LayoutUtility.GetPreferredSize(rectTransform, 0);
            float ySize = LayoutUtility.GetPreferredSize(rectTransform, 1);

            float aspectRatio = xSize / ySize;
            if (float.IsNormal(aspectRatio))
                this.aspectRatio = xSize / ySize;
            else
                this.aspectRatio = 1;
        }
    }
}
