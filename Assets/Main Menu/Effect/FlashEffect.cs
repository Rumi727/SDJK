using SCKRM;
using SCKRM.Rhythm;
using SCKRM.UI;
using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.MainMenu
{
    [RequireComponent(typeof(Image))]
    public class FlashEffect : SCKRM.UI.UI
    {
        [SerializeField] Image _image; public Image image => _image = this.GetComponentFieldSave(_image);
        [SerializeField] FlashOrder _flashOrder = FlashOrder.background; public FlashOrder flashOrder => _flashOrder;

        void Update()
        {
            Map.Map map = MapManager.selectedMap;
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
