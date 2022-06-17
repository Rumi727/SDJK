using SCKRM.Renderer;
using SCKRM.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.FileDialog.ShortcurBar
{
    [AddComponentMenu("SC KRM/File Dialog/UI/Object Pooling/Shortcur Bar Button")]
    public sealed class FileDialogShortcutBarButton : UIObjectPooling
    {
        [SerializeField, NotNull] Button _button; public Button button { get => _button; }
        [SerializeField, NotNull] CustomAllSpriteRenderer _icon; public CustomAllSpriteRenderer icon { get => _icon; }
        [SerializeField, NotNull] CustomAllTextRenderer _text; public CustomAllTextRenderer text { get => _text; }

        public override bool Remove()
        {
            button.onClick.RemoveAllListeners();
            return base.Remove();
        }
    }
}
