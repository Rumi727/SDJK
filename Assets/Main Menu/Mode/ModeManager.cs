using SCKRM;
using SCKRM.Reflection;
using SCKRM.SaveLoad;
using SDJK.Ruleset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SDJK.Mode
{
    public static class ModeManager
    {
        public static List<IMode> modeList { get; } = new List<IMode>();
        public static List<IMode> allModeList { get; } = new List<IMode>();
        public static List<IMode> selectedModeList { get; private set; } = new List<IMode>();


        public static event Action modeRefresh;
        public static event Action modeChanged;

        public static bool isModeRefreshEnd { get; private set; } = false;

        [Awaken]
        static void Awaken()
        {
            RulesetManager.rulesetChanged += () =>
            {
                selectedModeList.Clear();
                modeList.Clear();

                for (int i = 0; i < allModeList.Count; i++)
                {
                    IMode mode = allModeList[i];
                    if (mode.targetRuleset == RulesetManager.selectedRuleset.name)
                        modeList.Add(mode);
                }

                //정렬
                {
                    List<IMode> elements = modeList.OrderBy(x => x.order).ToList();

                    modeList.Clear();
                    modeList.AddRange(elements);
                }
            };
        }

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
                            IMode mode = (IMode)Activator.CreateInstance(type);
                            if (mode.targetRuleset == RulesetManager.selectedRuleset.name)
                                modeList.Add(mode);

                            allModeList.Add(mode);
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
            modeRefresh?.Invoke();
        }

        public static void SelectMode(IMode mode)
        {
            if (!selectedModeList.Contains(mode))
            {
                if (mode.incompatibleModes != null)
                {
                    for (int i = 0; i < selectedModeList.Count; i++)
                    {
                        IMode tempMode = selectedModeList[i];

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
                selectedModeList = selectedModeList.OrderBy(x => x.order).ToList();

                modeChanged?.Invoke();
            }
        }

        public static void DeselectMode(IMode mode)
        {
            selectedModeList.Remove(mode);
            modeChanged?.Invoke();

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
