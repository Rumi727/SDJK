using Newtonsoft.Json.Schema;
using SCKRM;
using SCKRM.Renderer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Permissions;
using UnityEngine;

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

                isRulesetChanged?.Invoke();
            }
        }
        static int _selectedRulesetIndex;

        public static event Action isRulesetChanged;
        public static event Action isRulesetRefresh;

        [Starten]
        public static void RulesetListRefresh()
        {
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int assemblysIndex = 0; assemblysIndex < assemblys.Length; assemblysIndex++)
            {
                Type[] types = assemblys[assemblysIndex].GetTypes();
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
            }

            selectedRulesetIndex = 0;
            isRulesetRefresh?.Invoke();
        }

        /// <summary>
        /// 현제 선택된 규칙 집합이랑 호환되는 모드인지 확인합니다
        /// </summary>
        /// <param name="ruleset"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [WikiDescription("현제 선택된 규칙 집합이랑 호환되는 모드인지 확인합니다")]
        public static bool IsCompatibleRuleset(this IRuleset ruleset, string mode)
        {
            if (selectedRuleset.name == mode)
                return true;

            if (ruleset.compatibleRuleset == null)
                return false;

            for (int i = 0; i < ruleset.compatibleRuleset.Length; i++)
            {
                if (selectedRuleset.name == ruleset.compatibleRuleset[i])
                    return true;
            }

            return false;
        }

        public static void GameStart(string mapFilePath)
        {
            for (int i = 0; i < rulesetList.Count; i++)
            {
                IRuleset ruleset = rulesetList[i];
                if (ruleset == selectedRuleset)
                    ruleset.GameStart(mapFilePath);
            }
        }
    }

    /// <summary>
    /// 이 인터페이스를 상속하면 SDJK가 규칙 집합을 자동으로 감지합니다
    /// </summary>
    [WikiDescription("이 인터페이스를 상속하면 SDJK가 규칙 집합을 자동으로 감지합니다")]
    public interface IRuleset
    {
        public string name { get; }
        public NameSpaceIndexTypePathPair icon { get; }
        public string[] compatibleRuleset { get; }

        public void GameStart(string mapFilePath);
    }

    /// <summary>
    /// <see cref="IRuleset"/> 인터페이스를 사용할때 커스텀하지 않을경우 권장하는 부모 클래스 입니다
    /// </summary>
    [WikiDescription("IRuleSet 인터페이스를 사용할때 커스텀하지 않을경우 권장하는 부모 클래스 입니다")]
    public abstract class Ruleset : IRuleset
    {
        public string name => GetType().FullName;
        public abstract NameSpaceIndexTypePathPair icon { get; }
        public virtual string[] compatibleRuleset => null;

        public abstract void GameStart(string mapFilePath);
    }
}
