using SCKRM;
using System.Collections.Generic;

namespace SDJK.Ruleset
{
    public abstract class JudgementManagerBase : ManagerBase<JudgementManagerBase>
    {
        public int combo { get; set; }
        public int maxCombo { get; set; }

        public double score { get; set; }

        /// <summary>
        /// 0 ~ 1 (0에 가까울수록 정확함)
        /// </summary>
        public double accuracyAbs { get; set; } = 0;
        public List<double> accuracyAbsList { get; } = new List<double>();

        /// <summary>
        /// -1 ~ 1 (0에 가까울수록 정확함)
        /// </summary>
        public double accuracy { get; set; } = 0;
        public List<double> accuracys { get; } = new List<double>();

        public double rankProgress { get; set; } = 0;

        public double health
        {
            get => _health;
            set => _health = value.Clamp(0, maxHealth);
        }
        double _health = maxHealth;

        public const double maxHealth = 100;



        public virtual bool Refresh() => SingletonCheck(this);
        public virtual void TimeChanged() { }
    }
}
