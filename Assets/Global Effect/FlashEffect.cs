using SCKRM;
using SCKRM.Rhythm;
using SDJK.Map;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Effect
{
    [RequireComponent(typeof(Image))]
    public class FlashEffect : Effect
    {
        [SerializeField] Image _image; public Image image => _image = this.GetComponentFieldSave(_image);
        [SerializeField] FlashOrder _flashOrder = FlashOrder.background; public FlashOrder flashOrder => _flashOrder;

        protected override void RealUpdate()
        {
            if (map == null)
            {
                image.color = Color.clear;
                return;
            }

            if (flashOrder == FlashOrder.background)
                image.color = map.globalEffect.backgroundFlash.GetValue(RhythmManager.currentBeatScreen);
            else if (flashOrder == FlashOrder.field)
                image.color = map.globalEffect.fieldFlash.GetValue(RhythmManager.currentBeatScreen);
            else if (flashOrder == FlashOrder.ui)
                image.color = map.globalEffect.uiFlash.GetValue(RhythmManager.currentBeatScreen);
        }
    }
}
