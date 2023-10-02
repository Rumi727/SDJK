using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(SlicedFilledImage))]
    public class SyncSlicedFilledImageToSlider : UIBase
    {
        SlicedFilledImage _slicedFilledImage;
        public SlicedFilledImage slicedFilledImage => _slicedFilledImage = this.GetComponentFieldSave(_slicedFilledImage);


        [SerializeField, FieldNotNull] Slider _slider;
        public Slider slider => _slider;


        void LateUpdate()
        {
            if (slider == null)
                return;

            slicedFilledImage.fillAmount = slider.value / slider.maxValue;
        }
    }
}