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

        protected override void JudgementAction(double disSecond, bool isMiss, JudgementMetaData metaData)
        {
            int combo = SDJKJudgementManager.instance.combo;
            if (combo <= 0)
                text.text = "";
            else
                text.text = combo.ToString();
        }
    }
}
