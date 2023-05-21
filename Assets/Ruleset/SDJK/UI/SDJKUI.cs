using Cysharp.Threading.Tasks;
using SCKRM;
using SDJK.Ruleset.SDJK.Judgement;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.UI
{
    public abstract class SDJKUI : SCKRM.UI.UIBase
    {
        [SerializeField, FieldNotNull] SDJKJudgementManager _judgementManager; public SDJKJudgementManager judgementManager => _judgementManager;

        /// <summary>
        /// Please put base.OnEnable() when overriding
        /// </summary>
        protected override async void OnEnable()
        {
            if (await UniTask.WaitUntil(() => judgementManager != null, PlayerLoopTiming.PreUpdate, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow())
                return;

            judgementManager.judgementAction += JudgementAction;
        }

        protected virtual void JudgementAction(double disSecond, bool isMiss, double accuracy, double generousAccuracy, JudgementMetaData metaData) { }

        /// <summary>
        /// Please put base.OnDisable() when overriding
        /// </summary>
        protected override async void OnDisable()
        {
            if (await UniTask.WaitUntil(() => judgementManager != null, PlayerLoopTiming.PreUpdate, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow())
                return;

            judgementManager.judgementAction -= JudgementAction;
        }
    }
}
