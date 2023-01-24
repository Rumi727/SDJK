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
    public sealed class ComboUI : SDJKUI
    {
        [SerializeField] TMP_Text text;

        float timer = 0;
        bool gray = false;
        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            if (timer >= 0.1f)
            {
                if (gray)
                {
                    text.color = Color.white;
                    gray = false;

                    timer = 0;
                }
                else if (RhythmManager.screenYukiMode)
                {
                    text.color = Color.gray;
                    gray = true;

                    timer = 0;
                }
            }
            else
                timer += Kernel.deltaTime;
        }

        protected override void JudgementAction(double disSecond, bool isMiss, double accuracy, JudgementMetaData metaData)
        {
            int combo = SDJKJudgementManager.instance.combo;
            if (combo <= 0)
                text.text = "";
            else
                text.text = combo.ToString();
        }
    }
}
