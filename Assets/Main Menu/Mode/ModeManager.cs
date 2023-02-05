using SCKRM;
using SCKRM.Reflection;
using SCKRM.SaveLoad;
using SDJK.Ruleset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;

namespace SDJK.Mode
{
    public static class ModeManager
    {
        public static List<IMode> modeList { get; } = new List<IMode>();
        public static List<IMode> selectedModeList { get; } = new List<IMode>();

        public static event Action isModeRefresh;
        public static bool isModeRefreshEnd { get; private set; } = false;

        [Awaken]
        static void Awaken() => RulesetManager.isRulesetChanged += () => selectedModeList.Clear();

        [Starten]
        public static void ModeListRefresh()
        {
            Type[] types = ReflectionManager.types;
            for (int typesIndex = 0; typesIndex < types.Length; typesIndex++)
            {
                Type type = types[typesIndex];
                if (type.IsPublic && type.IsClass && !type.IsAbstract && !type.IsSpecialName)
                {
                    Type[] interfaces = type.GetInterfaces();
                    for (int interfaceIndex = 0; interfaceIndex < interfaces.Length; interfaceIndex++)
                    {
                        Type interfaceType = interfaces[interfaceIndex];
                        if (interfaceType == typeof(IMode))
                        {
                            modeList.Add((IMode)Activator.CreateInstance(type));
                            break;
                        }
                    }
                }
            }

            //정렬
            {
                List<IMode> elements = modeList.OrderBy(x => x.order).ToList();

                modeList.Clear();
                modeList.AddRange(elements);
            }

            selectedModeList.Clear();
            isModeRefreshEnd = true;
            isModeRefresh?.Invoke();
        }

        /// <summary>
        /// 선택한 규칙 집합이랑 호환되는 모드인지 확인합니다
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="targetModeNames"></param>
        /// <returns></returns>
        [WikiDescription("선택한 규칙 집합이랑 호환되는 모드인지 확인합니다")]
        public static bool IsCompatibleMode(this IMode mode, string targetRulesetName)
        {
            if (mode.targetRuleset == targetRulesetName)
                return true;

            return false;
        }

        public static void SelectMode(IMode mode)
        {
            if (!selectedModeList.Contains(mode))
            {
                for (int i = 0; i < selectedModeList.Count; i++)
                {
                    IMode tempMode = selectedModeList[i];
                    if (mode.incompatibleModes != null)
                    {
                        for (int j = 0; j < mode.incompatibleModes.Length; j++)
                        {
                            Type tempModeType = tempMode.GetType();
                            Type modeType = mode.incompatibleModes[j];

                            if (tempModeType == modeType || tempModeType.IsSubclassOf(modeType))
                                DeselectMode(tempMode);
                        }
                    }
                }

                selectedModeList.Add(mode);
            }
        }

        public static void DeselectMode(IMode mode)
        {
            selectedModeList.Remove(mode);
            if (mode.modeConfigSlc != null)
            {
                {
                    SaveLoadClass.SaveLoadVariable<PropertyInfo>[] slvs = mode.modeConfigSlc.propertyInfos;
                    for (int j = 0; j < slvs.Length; j++)
                    {
                        SaveLoadClass.SaveLoadVariable<PropertyInfo> slv = slvs[j];
                        slv.variableInfo.SetValue(mode.modeConfigSlc.instance, slv.defaultValue);
                    }
                }

                {
                    SaveLoadClass.SaveLoadVariable<FieldInfo>[] slvs = mode.modeConfigSlc.fieldInfos;
                    for (int j = 0; j < slvs.Length; j++)
                    {
                        SaveLoadClass.SaveLoadVariable<FieldInfo> slv = slvs[j];
                        slv.variableInfo.SetValue(mode.modeConfigSlc.instance, slv.defaultValue);
                    }
                }
            }
        }
    }
}
