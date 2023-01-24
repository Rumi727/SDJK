using SCKRM;
using SDJK.Ruleset.SDJK.Judgement;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public abstract class SDJKUI : SCKRM.UI.UI
    {
        [SerializeField, NotNull] SDJKJudgementManager _judgementManager; public SDJKJudgementManager judgementManager => _judgementManager;

        protected override void OnEnable() => judgementManager.judgementAction += JudgementAction;

        protected virtual void JudgementAction(double disSecond, bool isMiss, double accuracy, JudgementMetaData metaData) { }

        protected override void OnDisable() => judgementManager.judgementAction -= JudgementAction;
    }
}
