using System.Linq;
using UnityEngine;

namespace SCKRM.UI.Layout
{
    [ExecuteAlways]
    [AddComponentMenu("SC KRM/UI/Size Fitter/Child Size Fitter")]
    [RequireComponent(typeof(RectTransform))]
    public sealed class ChildSizeFitter : LayoutChild
    {
        [SerializeField] bool _xSize = false;
        public bool xSize { get => _xSize; set => _xSize = value; }
        [SerializeField] bool _ySize = false;
        public bool ySize { get => _ySize; set => _ySize = value; }

        [SerializeField] Vector2 _offset = Vector2.zero;
        public Vector2 offset { get => _offset; set => _offset = value; }

        [SerializeField, Min(0)] float _min = 0;
        public float min { get => _min; set => _min = value; }
        [SerializeField, Min(0)] float _max = 0;
        public float max { get => _max; set => _max = value; }



        DrivenRectTransformTracker tracker;



        protected override void OnDisable()
        {
            if (!Kernel.isPlaying)
                tracker.Clear();
        }

        public override void LayoutRefresh()
        {
            base.LayoutRefresh();

            if (childRectTransforms == null)
                return;

            float x = 0;
            float y = 0;

            for (int i = 0; i < childRectTransforms.Count; i++)
            {
                RectTransform childRectTransform = childRectTransforms[i];
                if (childRectTransform == null)
                {
                    spacingCancel();
                    continue;
                }
                else if (ignore.Contains(childRectTransform))
                {
                    spacingCancel();
                    continue;
                }
                else if (disabledObjectIgnore && !childRectTransform.gameObject.activeInHierarchy)
                {
                    spacingCancel();
                    continue;
                }

                x += childRectTransform.sizeDelta.x + spacing;
                y += childRectTransform.sizeDelta.y + spacing;

                if (i == childRectTransforms.Count - 1)
                    spacingCancel();

                void spacingCancel()
                {
                    x -= spacing;
                    y -= spacing;
                }
            }

            targetSize = new Vector2(x + offset.x, y + offset.y);
            if (max <= 0)
            {
                targetSize.x = targetSize.x.Clamp(min);
                targetSize.y = targetSize.y.Clamp(min);
            }
            else
            {
                targetSize.x = targetSize.x.Clamp(min, max);
                targetSize.y = targetSize.y.Clamp(min, max);
            }
        }

        Vector2 targetSize;
        public override void SizeUpdate(bool useAni = true)
        {
            if (!Kernel.isPlaying)
            {
                tracker.Clear();

                if (xSize)
                    tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaX);
                if (ySize)
                    tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaY);
            }

            if (!lerp || !Kernel.isPlaying || !useAni)
            {
                if (xSize)
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetSize.x);
                if (ySize)
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetSize.y);
            }
            else
            {
                if (xSize)
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.rect.width.Lerp(targetSize.x, lerpValue * Kernel.fpsUnscaledSmoothDeltaTime));
                if (ySize)
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.rect.height.Lerp(targetSize.y, lerpValue * Kernel.fpsUnscaledSmoothDeltaTime));
            }
        }
    }
}