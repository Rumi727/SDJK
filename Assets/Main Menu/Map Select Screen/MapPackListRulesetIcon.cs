using SCKRM.Renderer;
using SCKRM.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.MainMenu
{
    public sealed class MapPackListRulesetIcon : UIObjectPoolingBase
    {
        [SerializeField] CanvasGroup _canvasGroup; public CanvasGroup canvasGroup => _canvasGroup;

        [SerializeField] CustomSpriteRendererBase _icon; public CustomSpriteRendererBase icon => _icon;
        [SerializeField] Image _iconBackground; public Image iconBackground => _iconBackground;

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            canvasGroup.alpha = 1;

            icon.nameSpace = "";
            icon.type = "";
            icon.path = "";
            icon.index = 0;

            icon.Refresh();
            return true;
        }
    }
}
