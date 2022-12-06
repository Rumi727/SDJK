using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.Rhythm;
using SDJK.Ruleset.SDJK.Judgement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.UI
{
    public sealed class ComboUI : SCKRM.UI.UI
    {
        [SerializeField] TMP_Text text;

        protected override async void Awake()
        {
            if (await UniTask.WaitUntil(() => SDJKJudgementManager.instance != null, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow())
                return;

            SDJKJudgementManager.instance.judgementAction += JudgementAction;
        }

        float timer = 0;
        bool gray = false;
        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            timer += Kernel.deltaTime;

            if (timer >= 0.1f)
            {
                if (gray)
                {
                    text.color = Color.white;
                    gray = false;
                }
                else
                {
                    text.color = Color.gray;
                    gray = true;
                }

                timer = 0;
            }
        }

        void JudgementAction(double disSecond, bool isMiss, JudgementMetaData metaData)
        {
            int combo = SDJKJudgementManager.instance.combo;
            if (combo <= 0)
                text.text = "";
            else
                text.text = combo.ToString();
        }
    }
}
