using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.Renderer;
using SDJK.Ruleset.SDJK.Judgement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.UI
{
    public class JudgementUI : SCKRM.UI.UI
    {
        [SerializeField] CustomImageRenderer image;
        [SerializeField] TMP_Text delayText;
        [SerializeField] string delayTextSuffix;
        [SerializeField] float jumpValue;
        [SerializeField] float gravityValue;
        [SerializeField] float defaultY;

        protected override async void Awake()
        {
            if (await UniTask.WaitUntil(() => SDJKJudgementManager.instance != null, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow())
                return;

            SDJKJudgementManager.instance.judgementAction += JudgementAction;
        }

        float yVelocity = 0;
        void Update()
        {
            if (rectTransform.anchoredPosition.y + yVelocity <= defaultY)
            {
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, defaultY);
                yVelocity = 0;
            }
            else
            {
                rectTransform.anchoredPosition += new Vector2(0, yVelocity) * Kernel.fpsDeltaTime;
                yVelocity -= gravityValue * Kernel.fpsDeltaTime;
            }
        }

        void JudgementAction(double disSecond, bool isMiss, JudgementMetaData metaData)
        {
            if (!graphic.enabled)
                graphic.enabled = true;

            switch (metaData.nameKey)
            {
                case SDJKRuleset.sick:
                    image.path = "sick";
                    break;
                case SDJKRuleset.perfect:
                    image.path = "perfect";
                    break;
                case SDJKRuleset.great:
                    image.path = "great";
                    break;
                case SDJKRuleset.good:
                    image.path = "good";
                    break;
                case SDJKRuleset.early:
                    image.path = "early";
                    break;
                case SDJKRuleset.miss:
                    image.path = "miss";
                    break;
            }

            image.Refresh();
            delayText.text = (disSecond * 1000).RoundToInt() + delayTextSuffix;

            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, defaultY);
            yVelocity = jumpValue;
        }
    }
}
