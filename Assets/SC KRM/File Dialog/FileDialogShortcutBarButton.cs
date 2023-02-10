using SCKRM.Renderer;
using SCKRM.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.FileDialog.ShortcurBar
{
    [WikiDescription("파일 선택 화면의 바로가기 버튼을 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/File Dialog/UI/Object Pooling/Shortcur Bar Button")]
    public sealed class FileDialogShortcutBarButton : UIObjectPoolingBase
    {
        [SerializeField, NotNull] Button _button; public Button button { get => _button; }
        [SerializeField, NotNull] CustomSpriteRendererBase _icon; public CustomSpriteRendererBase icon { get => _icon; }
        [SerializeField, NotNull] CustomTextRendererBase _text; public CustomTextRendererBase text { get => _text; }

        [WikiDescription("버튼 삭제")]
        public override bool Remove()
        {
            button.onClick.RemoveAllListeners();
            return base.Remove();
        }
    }
}
