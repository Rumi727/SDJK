#nullable enable
using SCKRM.UI;
using UnityEngine;

namespace SCKRM.Logo
{
    [ExecuteAlways]
    public class LogoPos : UIBase
    {
        public MainLogo? mainLogo => _mainLogo;
        [SerializeField, FieldNotNull] MainLogo? _mainLogo;

        public AnimationCurve xAnimationCurve { get => _xAnimationCurve; set => _xAnimationCurve = value; }
        [SerializeField] AnimationCurve _xAnimationCurve = new AnimationCurve();

        public AnimationCurve yAnimationCurve { get => _yAnimationCurve; set => _yAnimationCurve = value; }
        [SerializeField] AnimationCurve _yAnimationCurve = new AnimationCurve();

        DrivenRectTransformTracker tracker;

        protected override void OnDisable()
        {
            if (!Kernel.isPlaying)
                tracker.Clear();
        }

        void Update()
        {
            if (mainLogo == null)
                return;

            if (!Kernel.isPlaying)
            {
                tracker.Clear();
                tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPosition);
            }

            Vector2 pos = rectTransform.anchoredPosition;
            pos.x = xAnimationCurve.Evaluate(mainLogo.aniProgress);
            pos.y = yAnimationCurve.Evaluate(mainLogo.aniProgress);

            rectTransform.anchoredPosition = pos;
        }
    }
}
