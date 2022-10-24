using SCKRM;
using SCKRM.Object;
using SDJK.Ruleset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.MainMenu.UI
{
    public class StatusBarRulesets : MonoBehaviour
    {
        void Awake() => RulesetManager.isRulesetRefresh += Refresh;

        List<StatusBarRuleset> statusBarRulesets = new List<StatusBarRuleset>();
        void Refresh()
        {
            for (int i = 0; i < statusBarRulesets.Count; i++)
                statusBarRulesets[i].Remove();

            statusBarRulesets.Clear();

            for (int i = 0; i < RulesetManager.rulesetList.Count; i++)
            {
                IRuleset ruleset = RulesetManager.rulesetList[i];
                StatusBarRuleset statusBarRuleset = (StatusBarRuleset)ObjectPoolingSystem.ObjectCreate("status_bar_rulesets_ruleset", transform).monoBehaviour;
                statusBarRuleset.icon.nameSpaceIndexTypePathPair = ruleset.icon;
                statusBarRuleset.index = i;
            }
        }
    }
}
