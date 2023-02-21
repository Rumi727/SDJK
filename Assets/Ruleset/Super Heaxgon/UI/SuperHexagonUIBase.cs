using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.UI;
using SDJK.Ruleset.SuperHexagon.Judgement;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.UI
{
    public abstract class SuperHexagonUIBase : UIBase
    {
        [SerializeField, NotNull] SuperHexagonJudgementManager _judgementManager; public SuperHexagonJudgementManager judgementManager => _judgementManager;

        /// <summary>
        /// Please put base.OnEnable() when overriding
        /// </summary>
        protected override async void OnEnable()
        {
            if (await UniTask.WaitUntil(() => judgementManager != null, PlayerLoopTiming.PreUpdate, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow())
                return;

            judgementManager.judgementAction += JudgementAction;
        }

        protected virtual void JudgementAction(bool isMiss) { }

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
