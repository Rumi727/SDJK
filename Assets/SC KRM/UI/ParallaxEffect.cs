using SCKRM.Input;
using SCKRM.UI;
using UnityEngine;

namespace SCKRM
{
    [ExecuteAlways, RequireComponent(typeof(CanvasSetting))]
    public sealed class ParallaxEffect : UIBase
    {
        public float size { get => _size; set => _size = value; } [SerializeField] float _size = 8;
        public Rect safeScreenOffset { get => _safeScreenOffset; set => _safeScreenOffset = value; } [SerializeField] Rect _safeScreenOffset = Rect.zero;

        void Update()
        {
            if (!canvasSetting.forceSafeScreenEnable)
                canvasSetting.forceSafeScreenEnable = true;

            if (Kernel.isPlaying)
            {
                Vector2 pos = InputManager.mousePosition / ScreenManager.size;
                pos.x -= 0.5f;
                pos.y -= 0.5f;
                pos *= 2;

                Rect rect = new Rect(-size, -size, size * 2, size * 2);
                rect.position += size * pos;

                rect.min += safeScreenOffset.min;
                rect.max += safeScreenOffset.max;

                rect.position = rect.position.Clamp(new Vector2(-size * 2, -size * 2), Vector2.zero);
                canvasSetting.safeScreenOffset = rect;
            }
            else
                canvasSetting.safeScreenOffset = safeScreenOffset;
        }

        protected override void OnDisable() => canvasSetting.safeScreenOffset = safeScreenOffset;
    }
}
