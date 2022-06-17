using SCKRM.Renderer;
using SCKRM.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.FileDialog.MyPC
{
    [AddComponentMenu("SC KRM/File Dialog/UI/Object Pooling/File Dialog My PC Button")]
    public sealed class FileDialogMyPCButton : UIObjectPooling
    {
        [SerializeField, NotNull] Button _button; public Button button { get => _button; }
        [SerializeField, NotNull] Slider _capacitySlider; public Slider capacitySlider { get => _capacitySlider; }
        [SerializeField, NotNull] Image _capacitySliderFill; public Image capacitySliderFill { get => _capacitySliderFill; }

        [SerializeField, NotNull] CustomAllSpriteRenderer _icon; public CustomAllSpriteRenderer icon { get => _icon; }
        [SerializeField, NotNull] TMP_Text _nameText; public TMP_Text nameText { get => _nameText; }
        [SerializeField, NotNull] CustomAllTextRenderer _capacityText; public CustomAllTextRenderer capacityText { get => _capacityText; }

        public override bool Remove()
        {
            button.onClick.RemoveAllListeners();

            capacitySlider.gameObject.SetActive(true);
            capacityText.gameObject.SetActive(true);

            return base.Remove();
        }
    }
}
