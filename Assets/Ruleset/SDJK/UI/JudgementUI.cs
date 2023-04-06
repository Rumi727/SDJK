using SCKRM;
using SCKRM.Renderer;
using SCKRM.Rhythm;
using TMPro;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.UI
{
    public sealed class JudgementUI : SDJKUI
    {
        [SerializeField, NotNull] CustomImageRenderer image;
        [SerializeField, NotNull] TMP_Text delayText;
        [SerializeField] string delayTextSuffix;
        [SerializeField] float jumpValue;
        [SerializeField] float gravityValue;
        [SerializeField] float defaultY;

        float yVelocity = 0;
        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

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

        protected override void JudgementAction(double disSecond, bool isMiss, double accuracy, double generousAccuracy, JudgementMetaData metaData)
        {
            if (!graphic.enabled)
                graphic.enabled = true;

            string lastPath = image.path;
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
                case SDJKRuleset.instantDeath:
                    image.path = "death";
                    break;
            }

            if (lastPath != image.path)
                image.Refresh();

            delayText.text = (disSecond * 1000).Round(2) + delayTextSuffix;

            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, defaultY);
            yVelocity = jumpValue;
        }
    }
}
