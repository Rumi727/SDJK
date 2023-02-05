using SCKRM.Renderer;
using SCKRM.Tooltip;
using SCKRM.UI;
using SDJK.Mode;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.MainMenu.ModeSelectScreen
{
    public sealed class ModeToggle : UIObjectPooling
    {
        public IMode mode;

        [SerializeField] CustomTextRendererBase _nameText; public CustomTextRendererBase nameText => _nameText;
        [SerializeField] Tooltip _nameTooltip; public Tooltip nameTooltip => _nameTooltip;

        [SerializeField] Toggle _toggle; public Toggle toggle => _toggle;

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            toggle.isOn = false;
            toggle.onValueChanged.RemoveAllListeners();

            return true;
        }
    }
}
