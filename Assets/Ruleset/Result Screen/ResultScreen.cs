using Newtonsoft.Json.Linq;
using SCKRM;
using SCKRM.Easing;
using SCKRM.Input;
using SCKRM.Object;
using SCKRM.Resource;
using SCKRM.SaveLoad;
using SCKRM.UI;
using SDJK.Map;
using SDJK.Mode;
using SDJK.Replay;
using SDJK.Ruleset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDJlK.Ruleset.ResultScreen
{
    public sealed class ResultScreen : UIObjectPooling
    {
        public static ResultScreen Show(IRuleset ruleset, MapFile map, ReplayFile replay, Action backEvent, string prefab = "rulset.result_screen")
        {
            if (isShow)
                return null;

            ResultScreen resultScreen = (ResultScreen)ObjectPoolingSystem.ObjectCreate(prefab).monoBehaviour;
            resultScreen.Refresh(ruleset, map, replay, backEvent);

            return resultScreen;
        }

        [SerializeField] float lerpAni = 0.2f;
        [SerializeField] float alphaAni = 0.002f;

        [SerializeField, NotNull] CanvasGroup canvasGroup;

        [SerializeField, NotNull] Image rankBackground;
        [SerializeField, NotNull] Image rankColor;
        [SerializeField, NotNull] TMP_Text rankText;

        [SerializeField, NotNull] TMP_Text songName;
        [SerializeField, NotNull] TMP_Text artist;

        [SerializeField, NotNull] TMP_Text score;

        [SerializeField, NotNull] TMP_Text difficultyLabel;
        [SerializeField, NotNull] TMP_Text author;

        [SerializeField, NotNull] TMP_Text accuracy;
        [SerializeField, NotNull] TMP_Text maxCombo;

        [SerializeField, NotNull] Transform modes;
        [SerializeField] string modePrefab = "rulset.result_screen.mode";

        IRuleset ruleset;
        MapFile map = null;
        ReplayFile replay = null;
        Action backEvent;

        static bool isShow = false;

        List<ResultScreenMode> resultScreenModes = new List<ResultScreenMode>();
        public void Refresh(IRuleset ruleset, MapFile map, ReplayFile replay, Action backEvent)
        {
            this.ruleset = ruleset;
            this.map = map;
            this.replay = replay;
            this.backEvent = backEvent;

            songName.text = map.info.songName;
            artist.text = map.info.artist;

            difficultyLabel.text = map.info.difficultyLabel;
            author.text = map.info.author;

            ModeObjectRemove();

            for (int i = 0; i < replay.modes.Length; i++)
            {
                ReplayModeFile modeFile = replay.modes[i];

                ResultScreenMode mode = (ResultScreenMode)ObjectPoolingSystem.ObjectCreate(modePrefab, modes).monoBehaviour;
                resultScreenModes.Add(mode);

                IMode modeInstance = (IMode)Activator.CreateInstance(modeFile.modeType);
                mode.customSpriteRendererBase.nameSpaceIndexTypePathPair = modeInstance.icon;
                mode.customSpriteRendererBase.Refresh();

                string tooltipText = ResourceManager.SearchLanguage(modeInstance.displayName.path, modeInstance.displayName.nameSpace);
                if (modeFile.modeConfig != null)
                {
                    modeInstance.modeConfig = (IModeConfig)modeFile.modeConfig.ToObject(modeFile.modeConfigType);

                    bool first = true;
                    for (int j = 0; j < modeInstance.modeConfigSlc.propertyInfos.Length; j++)
                    {
                        SaveLoadClass.SaveLoadVariable<PropertyInfo> propertyInfo = modeInstance.modeConfigSlc.propertyInfos[j];
                        SaveLoadUIConfigBaseAttribute config = (SaveLoadUIConfigBaseAttribute)Attribute.GetCustomAttribute(propertyInfo.variableInfo, typeof(SaveLoadUIConfigBaseAttribute));

                        if (first)
                        {
                            tooltipText += "\n<size=7>\n</size>";
                            first = false;
                        }
                        else
                            tooltipText += "\n";

                        tooltipText += ResourceManager.SearchLanguage(config.name.path, config.name.nameSpace) + ": " + propertyInfo.variableInfo.GetValue(modeInstance.modeConfigSlc.instance);
                    }

                    for (int j = 0; j < modeInstance.modeConfigSlc.fieldInfos.Length; j++)
                    {
                        SaveLoadClass.SaveLoadVariable<FieldInfo> fieldInfo = modeInstance.modeConfigSlc.fieldInfos[j];
                        SaveLoadUIConfigBaseAttribute config = (SaveLoadUIConfigBaseAttribute)Attribute.GetCustomAttribute(fieldInfo.variableInfo, typeof(SaveLoadUIConfigBaseAttribute));

                        if (first)
                        {
                            tooltipText += "\n<size=7>\n</size>";
                            first = false;
                        }
                        else
                            tooltipText += "\n";

                        tooltipText += "\n" + ResourceManager.SearchLanguage(config.name.path, config.name.nameSpace) + ": " + fieldInfo.variableInfo.GetValue(modeInstance.modeConfigSlc.instance);
                    }
                }

                mode.tooltip.text = tooltipText;
            }

            InputManager.SetInputLock("ruleset.result_screen", true);
            UIManager.BackEventAdd(Hide);
            isShow = true;
        }

        void ModeObjectRemove()
        {
            for (int i = 0; i < resultScreenModes.Count; i++)
            {
                ResultScreenMode resultScreenMode = resultScreenModes[i];
                if (resultScreenMode != null)
                    resultScreenMode.Remove();
            }

            resultScreenModes.Clear();
        }

        public void Hide()
        {
            InputManager.SetInputLock("ruleset.result_screen", false);
            UIManager.BackEventRemove(Hide);

            backEvent?.Invoke();
            isShow = false;
        }

        double scoreAnimation = 0;
        double accuracyAnimation = 0;
        double maxComboAnimation = 0;
        float alphaValue = 0;
        void Update()
        {
            if (ruleset == null || map == null || replay == null)
                return;

            scoreAnimation = scoreAnimation.Lerp(replay.scores.GetValue(double.MaxValue), lerpAni * Kernel.fpsUnscaledSmoothDeltaTime);
            score.text = scoreAnimation.RoundToInt().ToString();

            accuracyAnimation = accuracyAnimation.Lerp(replay.accuracys.GetValue(double.MaxValue), lerpAni * Kernel.fpsUnscaledSmoothDeltaTime);
            accuracy.text = 100d.Lerp(0d, accuracyAnimation).Round(2) + "%";

            maxComboAnimation = maxComboAnimation.Lerp(replay.maxCombo.GetValue(double.MaxValue), lerpAni * Kernel.fpsUnscaledSmoothDeltaTime);
            maxCombo.text = maxComboAnimation.RoundToInt().ToString();

            {
                float fillAmout = (float)(scoreAnimation / JudgementManager.maxScore);

                rankBackground.fillAmount = 1 - fillAmout;
                rankColor.fillAmount = fillAmout;
                rankText.text = ruleset.GetRank(accuracyAnimation).name;
            }

            if (isShow)
                alphaValue = alphaValue.MoveTowards(1, alphaAni * Kernel.fpsUnscaledSmoothDeltaTime);
            else
            {
                alphaValue = alphaValue.MoveTowards(0, alphaAni * Kernel.fpsUnscaledSmoothDeltaTime);
                if (alphaValue <= 0)
                    Remove();
            }

            canvasGroup.alpha = alphaValue;
        }

        protected override void OnDestroy()
        {
            isShow = false;
            UIManager.BackEventRemove(Hide);
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            ruleset = null;
            map = null;
            replay = null;

            scoreAnimation = 0;
            accuracyAnimation = 0;
            maxComboAnimation = 0;

            ModeObjectRemove();

            isShow = false;
            return true;
        }
    }
}
