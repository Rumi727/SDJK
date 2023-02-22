using SCKRM;
using SCKRM.Input;
using SCKRM.Object;
using SCKRM.SaveLoad.UI;
using SCKRM.UI;
using SDJK.Mode;
using SDJK.Ruleset;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SDJK.MainMenu.ModeSelectScreen
{
    public sealed class ModeSelectScreen : SCKRM.UI.UIBase
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] GameObject layout;
        [SerializeField] string modeTogglePrefab = "main_menu.mode_select_screen.mode_toggle";
        [SerializeField] string modeConfigSaveLoadUIPrefab = "main_menu.mode_select_screen.mode_config";
        [SerializeField] string spacePrefab = "save_load.ui.space";
        [SerializeField] Transform modeListContent;
        [SerializeField] Transform modeConfigListContent;
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

        protected override void OnDestroy()
        {
            RulesetManager.isRulesetChanged -= ModeListRefresh;
            ModeManager.isModeRefresh -= ModeListRefresh;
        }

        List<IObjectPooling> modeListObjectPooling = new List<IObjectPooling>();
        void ModeListRefresh()
        {
            if (!ModeManager.isModeRefreshEnd)
                return;

            for (int i = 0; i < modeListObjectPooling.Count; i++)
            {
                IObjectPooling modeToggle = modeListObjectPooling[i];
                if (!modeToggle.isRemoved && !modeToggle.IsDestroyed())
                    modeToggle.Remove();
            }

            modeListObjectPooling.Clear();

            ModeConfigRefresh();

            for (int i = 0; i < ModeManager.modeList.Count; i++)
            {
                IMode mode = ModeManager.modeList[i];
                if (mode.targetRuleset != RulesetManager.selectedRuleset.name)
                    continue;

                if (i > 0 && mode.order.Distance(ModeManager.modeList[i - 1].order) >= 1000)
                    modeListObjectPooling.Add(ObjectPoolingSystem.ObjectCreate(spacePrefab, modeListContent).objectPooling);

                ModeToggle modeToggle = (ModeToggle)ObjectPoolingSystem.ObjectCreate(modeTogglePrefab, modeListContent).monoBehaviour;
                modeListObjectPooling.Add(modeToggle);

                modeToggle.mode = mode;

                modeToggle.icon.nameSpaceIndexTypePathPair = mode.icon;
                modeToggle.icon.Refresh();

                modeToggle.nameText.nameSpacePathReplacePair = mode.displayName;
                modeToggle.nameText.Refresh();

                modeToggle.nameTooltip.nameSpacePathPair = mode.info;

                modeToggle.toggle.isOn = ModeManager.selectedModeList.Contains(mode);
                modeToggle.onValueChanged.AddListener(x =>
                {
                    if (x)
                        ModeManager.SelectMode(mode);
                    else
                        ModeManager.DeselectMode(mode);

                    ModeConfigRefresh();
                });
            }
        }

        List<SaveLoadUI> saveLoadUIs = new List<SaveLoadUI>();
        void ModeConfigRefresh()
        {
            for (int i = 0; i < saveLoadUIs.Count; i++)
            {
                SaveLoadUI saveLoadUI = saveLoadUIs[i];
                if (saveLoadUI != null)
                    saveLoadUI.Remove();
            }

            saveLoadUIs.Clear();

            List<IMode> modes = ModeManager.selectedModeList.ToList();
            for (int i = 0; i < modes.Count; i++)
            {
                IMode mode = modes[i];
                if (mode == null || mode.modeConfig == null)
                {
                    modes.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < modes.Count; i++)
            {
                IMode mode = modes[i];

                SaveLoadUI saveLoadUI = (SaveLoadUI)ObjectPoolingSystem.ObjectCreate(modeConfigSaveLoadUIPrefab, modeConfigListContent).monoBehaviour;
                saveLoadUIs.Add(saveLoadUI);

                saveLoadUI.saveLoadClassName = mode.modeConfigSlc.name;
                saveLoadUI.isLineShow = i < modes.Count - 1;

                saveLoadUI.Refresh(mode.modeConfigSlc);
            }
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
