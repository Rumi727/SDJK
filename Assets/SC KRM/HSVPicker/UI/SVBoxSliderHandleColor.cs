using SCKRM;
using SCKRM.UI;
using UnityEngine;
using UnityEngine.UI.Extensions.ColorPicker;

namespace HSVPicker
{
    public class SVBoxSliderHandleColor : UIAniBase
    {
        public ColorPicker colorPicker;
        public Type type;

        protected override void OnEnable()
        {
            colorPicker?.internalOnValueChanged.AddListener(OnValueChanged);
            OnValueChanged(colorPicker.CurrentColor);
        }

        protected override void OnDisable() => colorPicker?.internalOnValueChanged.RemoveListener(OnValueChanged);

        Color color = Color.white;
        public void OnValueChanged(Color color)
        {
            if (graphic != null)
            {
                if (type == Type.a)
                    this.color = ColorReadability.GetReadbilityColor(color.a);
                else if (type == Type.h)
                    this.color = ColorReadability.GetReadbilityColor(HSVUtil.ConvertHsvToRgb(colorPicker.H * 360, 1, 1, 1));
                else
                    this.color = ColorReadability.GetReadbilityColor(color);
            }
        }



        void Update()
        {
            if (!lerp || !Kernel.isPlaying)
                graphic.color = color;
            else
                graphic.color = graphic.color.Lerp(color, lerpValue * Kernel.fpsUnscaledSmoothDeltaTime);
        }

        public enum Type
        {
            all,
            a,
            h
        }
    }
}