using SCKRM.Renderer;
using SCKRM.Rhythm;
using SCKRM.Sound;
using SCKRM.UI.SideBar;
using SCKRM.UI.StatusBar;
using SCKRM.UI;
using SCKRM;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SDJK.Mode;

namespace SDJK.Ruleset
{
    /// <summary>
    /// 이 인터페이스를 상속하면 SDJK가 규칙 집합을 자동으로 감지합니다
    /// </summary>
    [WikiDescription("이 인터페이스를 상속하면 SDJK가 규칙 집합을 자동으로 감지합니다")]
    public interface IRuleset
    {
        public int order { get; }
        public string name { get; }

        public NameSpaceIndexTypePathPair icon { get; }
        public string[] compatibleRulesets { get; }

        /// <summary>
        /// 판정이 작은순 부터 큰순으로 리스트를 만들어야합니다
        /// </summary>
        public JudgementMetaData[] judgementMetaDatas { get; }
        public JudgementMetaData missJudgementMetaData { get; }

        public void GameStart(string mapFilePath, string replayFilePath, bool isEditor, IMode[] modes);

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
    [WikiDescription("IRuleset 인터페이스를 사용할때 커스텀하지 않을경우 권장하는 부모 클래스 입니다")]
    public abstract class RulesetBase : IRuleset
    {
        public abstract int order { get; }
        public abstract string name { get; }

        public abstract NameSpaceIndexTypePathPair icon { get; }
        public virtual string[] compatibleRulesets => null;

        public abstract JudgementMetaData[] judgementMetaDatas { get; }
        public abstract JudgementMetaData missJudgementMetaData { get; }

        /// <summary>
        /// Please put base.GameStart() when overriding
        /// </summary>
        /// <param name="mapFilePath"></param>
        public virtual void GameStart(string mapFilePath, string replayFilePath, bool isEditor, IMode[] modes) => IRuleset.GameStartDefaultMethod();
    }

    public struct JudgementMetaData : IEquatable<JudgementMetaData>
    {
        public string nameKey;
        public double sizeSecond;

        public Color color;

        public double hpMultiplier;
        public bool missHp;

        public JudgementMetaData(string nameKey, double sizeSecond, Color color, double hpMultiplier = 1, bool missHp = false)
        {
            this.nameKey = nameKey;
            this.sizeSecond = sizeSecond;

            this.color = color;

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
