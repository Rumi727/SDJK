#nullable enable
using SCKRM.UI;
using System;
using UnityEngine;

namespace SCKRM.Logo
{
    [ExecuteAlways]
    public class LogoRotation : UIBase
    {
        public MainLogo? mainLogo => _mainLogo;
        [SerializeField, FieldNotNull] MainLogo? _mainLogo;

        public AnimationCurve animationCurve { get => _animationCurve; set => _animationCurve = value; }
        [SerializeField] AnimationCurve _animationCurve = new AnimationCurve();

        public float zAutoRotationSpeed { get => _zAutoRotationSpeed; set => _zAutoRotationSpeed = value; }
        [SerializeField, Min(0)] float _zAutoRotationSpeed = 0.5f;

        DrivenRectTransformTracker tracker;

        protected override void OnDisable()
        {
            if (!Kernel.isPlaying)
                tracker.Clear();
        }

        [NonSerialized] float zRotation = 0;
        void Update()
        {
            if (mainLogo == null)
                return;

            if (Kernel.isPlaying)
            {
                if (mainLogo.aniProgress <= 0)
                    zRotation = 0;
                else
                {
                    zRotation -= zAutoRotationSpeed * Kernel.fpsUnscaledSmoothDeltaTime;
                    zRotation = zRotation.Repeat(360);
                }
            }

            if (!Kernel.isPlaying)
            {
                tracker.Clear();
                tracker.Add(this, rectTransform, DrivenTransformProperties.Rotation);
            }

            rectTransform.localEulerAngles = new Vector3(0, 0, zRotation - animationCurve.Evaluate(mainLogo.aniProgress));
        }
    }
}
