using SCKRM.Renderer;
using SCKRM.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.FileDialog.MyPC
{
    [WikiDescription("파일 선택 화면의 내 PC 화면에 있는 버튼을 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/File Dialog/UI/Object Pooling/File Dialog My PC Button")]
    public sealed class FileDialogMyPCButton : UIObjectPooling
    {
        [SerializeField, NotNull] Button _button; public Button button { get => _button; }
        [SerializeField, NotNull] Slider _capacitySlider; public Slider capacitySlider { get => _capacitySlider; }
        [SerializeField, NotNull] Image _capacitySliderFill; public Image capacitySliderFill { get => _capacitySliderFill; }

        [SerializeField, NotNull] CustomSpriteRendererBase _icon; public CustomSpriteRendererBase icon { get => _icon; }
        [SerializeField, NotNull] TMP_Text _nameText; public TMP_Text nameText { get => _nameText; }
        [SerializeField, NotNull] CustomTextRendererBase _capacityText; public CustomTextRendererBase capacityText { get => _capacityText; }

        [WikiDescription("버튼 삭제")]
        public override bool Remove()
        {
            button.onClick.RemoveAllListeners();

            capacitySlider.gameObject.SetActive(true);
            capacityText.gameObject.SetActive(true);

            return base.Remove();
        }
    }
}
