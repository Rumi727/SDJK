using UnityEngine;
using UnityEngine.UI;

namespace HSVPicker
{
    [RequireComponent(typeof(Image))]
    public class ColorImage : MonoBehaviour
    {
        public ColorPicker picker;

        private Image image;

        private void Awake()
        {
            image = GetComponent<Image>();
            picker.internalOnValueChanged.AddListener(ColorChanged);
        }

        private void OnDestroy()
        {
            picker.internalOnValueChanged.RemoveListener(ColorChanged);
        }

        private void ColorChanged(Color newColor)
        {
            image.color = newColor;
        }
    }

}