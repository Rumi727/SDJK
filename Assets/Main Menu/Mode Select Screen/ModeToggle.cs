using SCKRM.Renderer;
using SCKRM.Tooltip;
using SCKRM.UI;
using SDJK.Mode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SDJK.MainMenu.ModeSelectScreen
{
    public sealed class ModeToggle : UIObjectPooling
    {
        public IMode mode;

        [SerializeField] CustomSpriteRendererBase _icon; public CustomSpriteRendererBase icon => _icon;

        [SerializeField] CustomTextRendererBase _nameText; public CustomTextRendererBase nameText => _nameText;
        [SerializeField] Tooltip _nameTooltip; public Tooltip nameTooltip => _nameTooltip;

        [SerializeField] Toggle _toggle; public Toggle toggle => _toggle;
        [SerializeField] UnityEvent<bool> _onValueChanged; public UnityEvent<bool> onValueChanged => _onValueChanged;

        bool invokeLock = false;
        void Update()
        {
            invokeLock = true;
            toggle.isOn = ModeManager.selectedModeList.FindMode(mode.GetType()) != null;
            invokeLock = false;
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            toggle.isOn = false;
            onValueChanged.RemoveAllListeners();

            return true;
        }

        public void OnValueChangedInvoke(bool value)
        {
            if (invokeLock)
                return;

            onValueChanged.Invoke(value);
        }
    }
}
