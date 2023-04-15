using SCKRM;
using SCKRM.Reflection;
using SDJK.Mode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SDJK.Ruleset
{
    public static class RulesetManager
    {
        public static List<IRuleset> rulesetList { get; } = new List<IRuleset>();

        public static IRuleset selectedRuleset { get; private set; } = null;
        public static int selectedRulesetIndex
        {
            get => _selectedRulesetIndex;
            set
            {
                selectedRuleset = rulesetList[value];
                _selectedRulesetIndex = value;

                rulesetChanged?.Invoke();
            }
        }
        static int _selectedRulesetIndex;

        public static event Action rulesetChanged;
        public static event Action rulesetRefresh;

        public static bool isRulesetRefreshEnd { get; private set; } = false;

        [Starten]
        public static void RulesetListRefresh()
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
                        if (interfaceType == typeof(IRuleset))
                        {
                            rulesetList.Add((IRuleset)Activator.CreateInstance(type));
                            break;
                        }
                    }
                }
            }

            //정렬
            {
                List<IRuleset> elements = rulesetList.OrderBy(x => x.order).ToList();

                rulesetList.Clear();
                rulesetList.AddRange(elements);
            }

            selectedRulesetIndex = 0;
            isRulesetRefreshEnd = true;
            rulesetRefresh?.Invoke();
        }

        /// <summary>
        /// 선택한 규칙 집합이랑 호환되는 규칙 집합인지 확인합니다
        /// </summary>
        /// <param name="ruleset"></param>
        /// <param name="targetRulesetNames"></param>
        /// <returns></returns>
        [WikiDescription("선택한 규칙 집합이랑 호환되는 규칙 집합인지 확인합니다")]
        public static bool IsCompatibleRuleset(this IRuleset ruleset, params string[] targetRulesetNames)
        {
            if (targetRulesetNames.Contains(ruleset.name))
                return true;

            if (ruleset.compatibleRulesets == null)
                return false;

            for (int i = 0; i < ruleset.compatibleRulesets.Length; i++)
            {
                if (targetRulesetNames.Contains(ruleset.compatibleRulesets[i]))
                    return true;
            }

            return false;
        }

        public static IRuleset GameStart(string mapFilePath, string replayFilePath, bool isEditor, params IMode[] modes)
        {
            if (modes == null)
                modes = IMode.emptyModes;

            for (int i = 0; i < rulesetList.Count; i++)
            {
                IRuleset ruleset = rulesetList[i];
                if (ruleset == selectedRuleset)
                {
                    ruleset.GameStart(mapFilePath, replayFilePath, isEditor, modes);
                    return ruleset;
                }
            }

            return null;
        }

        public static IRuleset FindRuleset(string name) => rulesetList.Find(x => x.name == name);
    }
}
