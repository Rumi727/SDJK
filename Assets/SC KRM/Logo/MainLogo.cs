#nullable enable
using SCKRM.UI;
using UnityEngine;

namespace SCKRM.Logo
{
    public class MainLogo : UIBase
    {
        public MainLogoState state { get => _state; set => _state = value; }
        [SerializeField] MainLogoState _state = 0;

        public float aniSpeed { get => _aniSpeed; set => _aniSpeed = value; }
        [SerializeField, Min(0)] float _aniSpeed = 0.01f;

        public float aniProgress { get => _aniProgress.Clamp01(); set => _aniProgress = value; }
        [SerializeField, Range(0, 1)] float _aniProgress = 0;

        void Update()
        {
            if (state == MainLogoState.start)
                aniProgress += aniSpeed * Kernel.fpsUnscaledSmoothDeltaTime;
            else if (state == MainLogoState.end)
                aniProgress -= aniSpeed * Kernel.fpsUnscaledSmoothDeltaTime;
        }
    }
}
