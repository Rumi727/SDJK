using Cysharp.Threading.Tasks;
using SDJK.Ruleset.SDJK.Judgement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public abstract class SDJKUI : SCKRM.UI.UI
    {
        protected override async void Awake()
        {
            if (await UniTask.WaitUntil(() => SDJKJudgementManager.instance != null, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow())
                return;

            SDJKJudgementManager.instance.judgementAction += JudgementAction;
        }

        protected abstract void JudgementAction(double disSecond, bool isMiss, JudgementMetaData metaData);
    }
}
