using SCKRM;
using SCKRM.Object;
using SCKRM.UI.StatusBar;
using SDJK.Ruleset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.MainMenu.UI
{
    public class StatusBarRulesets : MonoBehaviour
    {
        [SerializeField] RectTransform line;
        [SerializeField] Graphic lineGraphic;

        void Awake()
        {
            RulesetManager.rulesetRefresh += Refresh;
            if (RulesetManager.isRulesetRefreshEnd)
                Refresh();
        }

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

        void Update()
        {
            RectTransform statusBar = StatusBarManager.instance.rectTransform;
            float statusBarYPos = statusBar.anchoredPosition.y;
            float statusBarYSize = statusBar.rect.height;

            lineGraphic.color = new Color(1, 1, 1, 0).Lerp(Color.white, 1 - (statusBarYPos / statusBarYSize));
            line.anchoredPosition = line.anchoredPosition.Lerp(new Vector2(RulesetManager.selectedRulesetIndex * statusBarYSize, line.anchoredPosition.y), 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);
        }
    }
}
