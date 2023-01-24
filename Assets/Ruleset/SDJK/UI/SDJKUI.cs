using SCKRM;
using SDJK.Ruleset.SDJK.Judgement;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public abstract class SDJKUI : SCKRM.UI.UI
    {
        [SerializeField, NotNull] SDJKJudgementManager _judgementManager; public SDJKJudgementManager judgementManager => _judgementManager;

        /// <summary>
        /// Please put base.OnEnable() when overriding
        /// </summary>
        protected override void OnEnable() => judgementManager.judgementAction += JudgementAction;

        protected virtual void JudgementAction(double disSecond, bool isMiss, double accuracy, JudgementMetaData metaData) { }

        /// <summary>
        /// Please put base.OnDisable() when overriding
        /// </summary>
        protected override void OnDisable() => judgementManager.judgementAction -= JudgementAction;
    }
}
