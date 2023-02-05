using SCKRM.Renderer;
using SCKRM.UI;
using SDJK.Ruleset;

namespace SDJK.MainMenu.UI
{
    public class StatusBarRuleset : UIObjectPooling
    {
        public CustomSpriteRendererBase icon;
        public int index;

        public void OnClick() => RulesetManager.selectedRulesetIndex = index;
    }
}
