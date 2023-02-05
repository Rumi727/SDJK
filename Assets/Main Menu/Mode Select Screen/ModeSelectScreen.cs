using SCKRM;
using SCKRM.Input;
using SCKRM.Object;
using SCKRM.SaveLoad.UI;
using SCKRM.UI;
using SDJK.Mode;
using SDJK.Ruleset;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.MainMenu.ModeSelectScreen
{
    public sealed class ModeSelectScreen : SCKRM.UI.UI
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] GameObject layout;
        [SerializeField] string modeTogglePrefab = "main_menu.mode_select_screen.mode_toggle";
        [SerializeField] Transform modeListContent;
        [SerializeField] SaveLoadUI modeConfigContent;
        [SerializeField] string inputLockKey = "mode_select_screen";

        protected override void Awake()
        {
            RulesetManager.isRulesetChanged += ModeListRefresh;
            ModeManager.isModeRefresh += ModeListRefresh;

            if (ModeManager.isModeRefreshEnd)
                ModeListRefresh();
        }

        bool isShow = false;
        void Update()
        {
            if (isShow)
            {
                canvasGroup.alpha = canvasGroup.alpha.MoveTowards(1, 0.15f * Kernel.fpsUnscaledSmoothDeltaTime);

                if (!layout.activeSelf)
                    layout.SetActive(true);
            }
            else
            {
                canvasGroup.alpha = canvasGroup.alpha.MoveTowards(0, 0.15f * Kernel.fpsUnscaledSmoothDeltaTime);

                if (canvasGroup.alpha <= 0 && layout.activeSelf)
                    layout.SetActive(false);
            }
        }

        List<ModeToggle> modeToggles = new List<ModeToggle>();
        void ModeListRefresh()
        {
            if (!ModeManager.isModeRefreshEnd)
                return;

            for (int i = 0; i < modeToggles.Count; i++)
            {
                ModeToggle modeToggle = modeToggles[i];
                if (modeToggle != null)
                    modeToggle.Remove();
            }

            modeToggles.Clear();

            ModeConfigRefresh(null);

            for (int i = 0; i < ModeManager.modeList.Count; i++)
            {
                IMode mode = ModeManager.modeList[i];
                if (mode.targetRuleset != RulesetManager.selectedRuleset.name)
                    continue;

                ModeToggle modeToggle = (ModeToggle)ObjectPoolingSystem.ObjectCreate(modeTogglePrefab, modeListContent).monoBehaviour;
                modeToggles.Add(modeToggle);

                modeToggle.nameText.nameSpacePathReplacePair = mode.displayName;
                modeToggle.nameTooltip.nameSpacePathPair = mode.info;

                modeToggle.toggle.isOn = ModeManager.selectedModeList.Contains(mode);
                modeToggle.toggle.onValueChanged.AddListener(x =>
                {
                    if (x)
                        ModeManager.SelectMode(mode);
                    else
                        ModeManager.DeselectMode(mode);

                    ModeConfigRefresh(mode);
                });
            }
        }

        void ModeConfigRefresh(IMode mode)
        {
            if (mode == null || mode.modeConfig == null)
            {
                modeConfigContent.saveLoadClassName = "";
                modeConfigContent.Refresh();

                return;
            }

            modeConfigContent.saveLoadClassName = mode.modeConfigSlc.name;
            modeConfigContent.Refresh(mode.modeConfigSlc);
        }

        public void Show()
        {
            if (isShow)
                return;

            isShow = true;
            UIManager.BackEventAdd(Hide);
            InputManager.SetInputLock(inputLockKey, true);
        }

        public void Hide()
        {
            if (!isShow)
                return;

            isShow = false;
            UIManager.BackEventRemove(Hide);
            InputManager.SetInputLock(inputLockKey, false);
        }
    }
}
