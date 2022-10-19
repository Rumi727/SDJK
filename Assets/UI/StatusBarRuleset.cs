using SCKRM;
using SCKRM.Object;
using SCKRM.Renderer;
using SDJK.Ruleset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.UI
{
    public class StatusBarRuleset : ObjectPooling
    {
        public CustomAllSpriteRenderer icon;
        public int index;

        public void OnClick() => RulesetManager.selectedRulesetIndex = index;
    }
}
