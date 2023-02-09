using SCKRM.Object;
using SCKRM.SaveLoad;
using SCKRM;
using SDJK.Map;
using SDJK.Mode;
using SDJK.Replay;
using System.Reflection;
using System;
using UnityEngine;
using System.Collections.Generic;
using SCKRM.Resource;

namespace SDJK.Ruleset.ResultScreen
{
    public sealed class ReplayResultUIModes : ReplayResultUIBase
    {
        [SerializeField, NotNull] Transform targetTransform;
        [SerializeField] string modePrefab = "ruleset.result_screen.mode";

        List<ReplayResultUIMode> resultScreenModes = new List<ReplayResultUIMode>();
        public override void Refresh(IRuleset ruleset, MapFile map, ReplayFile replay)
        {
            base.Refresh(ruleset, map, replay);

            ModeObjectRemove();

            for (int i = 0; i < replay.modes.Length; i++)
            {
                ReplayModeFile modeFile = replay.modes[i];

                ReplayResultUIMode mode = (ReplayResultUIMode)ObjectPoolingSystem.ObjectCreate(modePrefab, targetTransform).monoBehaviour;
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
        }

        void ModeObjectRemove()
        {
            for (int i = 0; i < resultScreenModes.Count; i++)
            {
                ReplayResultUIMode resultScreenMode = resultScreenModes[i];
                if (resultScreenMode != null)
                    resultScreenMode.Remove();
            }

            resultScreenModes.Clear();
        }

        public override void Remove() => ModeObjectRemove();
    }
}
