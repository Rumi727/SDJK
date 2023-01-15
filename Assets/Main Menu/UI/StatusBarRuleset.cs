using SCKRM;
using SCKRM.Object;
using SCKRM.Renderer;
using SDJK.Ruleset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.MainMenu.UI
{
    public class StatusBarRuleset : ObjectPoolingBase
    {
        public CustomSpriteRendererBase icon;
        public int index;

        public void OnClick() => RulesetManager.selectedRulesetIndex = index;
    }
}
