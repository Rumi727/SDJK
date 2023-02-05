using SCKRM.Input;
using SCKRM.Sound;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SCKRM.UI
{
    [ExecuteAlways]
    [AddComponentMenu("SC KRM/UI/Kerenl/Volume Control/Volume Control")]
    public sealed class VolumeControl : UIAni, IPointerDownHandler, IEndDragHandler
    {
        enum Type
        {
            main,
            bgm,
            sound
        }

        [SerializeField] Type type;
        [SerializeField] RectTransform sliderRectTransform;
        [SerializeField] Slider slider;
        [SerializeField] TMP_Text nameText;
        [SerializeField] TMP_Text valueText;
        bool isDrag = false;

        void Update()
        {
            sliderRectTransform.offsetMin = new Vector2(nameText.rectTransform.sizeDelta.x + 6, sliderRectTransform.offsetMin.y);
            sliderRectTransform.offsetMax = new Vector2(-valueText.rectTransform.sizeDelta.x - 6, sliderRectTransform.offsetMax.y);

            if (!Kernel.isPlaying)
                return;

            if (!isDrag)
            {
                if (lerp)
                {
                    switch (type)
                    {
                        case Type.main:
                            slider.value = slider.value.Lerp(SoundManager.SaveData.mainVolume, lerpValue * Kernel.fpsUnscaledSmoothDeltaTime);
                            break;
                        case Type.bgm:
                            slider.value = slider.value.Lerp(SoundManager.SaveData.bgmVolume, lerpValue * Kernel.fpsUnscaledSmoothDeltaTime);
                            break;
                        case Type.sound:
                            slider.value = slider.value.Lerp(SoundManager.SaveData.soundVolume, lerpValue * Kernel.fpsUnscaledSmoothDeltaTime);
                            break;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case Type.main:
                            slider.value = SoundManager.SaveData.mainVolume;
                            break;
                        case Type.bgm:
                            slider.value = SoundManager.SaveData.bgmVolume;
                            break;
                        case Type.sound:
                            slider.value = SoundManager.SaveData.soundVolume;
                            break;
                    }
                }
            }

            switch (type)
            {
                case Type.main:
                    valueText.text = SoundManager.SaveData.mainVolume.ToString();
                    break;
                case Type.bgm:
                    valueText.text = SoundManager.SaveData.bgmVolume.ToString();
                    break;
                case Type.sound:
                    valueText.text = SoundManager.SaveData.soundVolume.ToString();
                    break;
            }
        }

        public void OnValueChanged()
        {
            if (isDrag)
            {
                switch (type)
                {
                    case Type.main:
                        SoundManager.SaveData.mainVolume = (int)slider.value;
                        break;
                    case Type.bgm:
                        SoundManager.SaveData.bgmVolume = (int)slider.value;
                        break;
                    case Type.sound:
                        SoundManager.SaveData.soundVolume = (int)slider.value;
                        break;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDrag = true;
            VolumeControlManager.OnBeginDrag();
            InputManager.SetInputLock("volumecontrol", true);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDrag = false;
            VolumeControlManager.OnEndDrag();
            InputManager.SetInputLock("volumecontrol", false);
        }
    }
}
