using SCKRM;
using SCKRM.Renderer;
using SCKRM.Rhythm;
using SCKRM.Sound;
using SCKRM.UI;
using SCKRM.UI.SideBar;
using SCKRM.UI.StatusBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.EventSystems;

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
        /// 선택한 규칙 집합이랑 호환되는 모드인지 확인합니다
        /// </summary>
        /// <param name="ruleset"></param>
        /// <param name="targetRulesetNames"></param>
        /// <returns></returns>
        [WikiDescription("선택한 규칙 집합이랑 호환되는 모드인지 확인합니다")]
        public static bool IsCompatibleRuleset(this IRuleset ruleset, params string[] targetRulesetNames)
        {
            if (targetRulesetNames.Contains(ruleset.name))
                return true;

            if (ruleset.compatibleRuleset == null)
                return false;

            for (int i = 0; i < ruleset.compatibleRuleset.Length; i++)
            {
                if (targetRulesetNames.Contains(ruleset.compatibleRuleset[i]))
                    return true;
            }

            return false;
        }

        public static IRuleset GameStart(string mapFilePath, bool isEditor)
        {
            for (int i = 0; i < rulesetList.Count; i++)
            {
                IRuleset ruleset = rulesetList[i];
                if (ruleset == selectedRuleset)
                {
                    ruleset.GameStart(mapFilePath, isEditor);
                    return ruleset;
                }
            }

            return null;
        }

        public static IRuleset FindRuleset(string name) => rulesetList.Find(x => x.name == name);
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

        /// <summary>
        /// 판정이 작은순 부터 큰순으로 리스트를 만들어야합니다
        /// </summary>
        public JudgementMetaData[] judgementMetaDatas { get; }
        public JudgementMetaData missJudgementMetaData { get; }

        public void GameStart(string mapFilePath, bool isEditor);

        public static void GameStartDefaultMethod()
        {
            StatusBarManager.statusBarForceHide = true;
            SideBarManager.sideBarForceHide = true;

            EventSystem.current.SetSelectedGameObject(null);
            SideBarManager.AllHide();

            UIManager.BackEventAllRemove();

            RhythmManager.Stop();
            SoundManager.StopSoundAll(true);
            SoundManager.StopNBSAll(true);

            Kernel.gameSpeed = 1;
        }
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

        public abstract JudgementMetaData[] judgementMetaDatas { get; }
        public abstract JudgementMetaData missJudgementMetaData { get; }

        /// <summary>
        /// Please put base.GameStart() when overriding
        /// </summary>
        /// <param name="mapFilePath"></param>
        public virtual void GameStart(string mapFilePath, bool isEditor) => IRuleset.GameStartDefaultMethod();
    }

    public struct JudgementMetaData : IEquatable<JudgementMetaData>
    {
        public string nameKey;
        public double sizeSecond;

        public double hpMultiplier;
        public bool missHp;

        public JudgementMetaData(string nameKey, double sizeSecond, double hpMultiplier = 1, bool missHp = false)
        {
            this.nameKey = nameKey;
            this.sizeSecond = sizeSecond;

            this.hpMultiplier = hpMultiplier;
            this.missHp = missHp;
        }

        public static bool operator ==(JudgementMetaData left, JudgementMetaData right) => left.Equals(right);
        public static bool operator !=(JudgementMetaData left, JudgementMetaData right) => !left.Equals(right);

        public override bool Equals(object obj)
        {
            if (obj is JudgementMetaData)
                return ((JudgementMetaData)obj).Equals(this);
            else
                return false;
        }

        public bool Equals(JudgementMetaData other) => nameKey == other.nameKey;

        public override int GetHashCode() => nameKey.GetHashCode();
    }
}
