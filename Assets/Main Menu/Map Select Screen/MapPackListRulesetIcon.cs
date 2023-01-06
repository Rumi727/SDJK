using SCKRM.Renderer;
using SCKRM.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.MainMenu
{
    public sealed class MapPackListRulesetIcon : UIObjectPooling
    {
        [SerializeField] CanvasGroup _canvasGroup; public CanvasGroup canvasGroup => _canvasGroup;
        [SerializeField] CustomAllSpriteRenderer _icon; public CustomAllSpriteRenderer icon => _icon;

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
