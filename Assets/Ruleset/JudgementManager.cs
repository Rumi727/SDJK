using SCKRM;
using System.Linq;

namespace SDJK.Ruleset
{
    public static class JudgementManager
    {
        public const double maxScore = 10000000;

        public static bool Judgement(this IRuleset ruleset, double disSecond, bool forceFastMiss, out JudgementMetaData judgementMetaData)
        {
            if (disSecond == 0 && ruleset.judgementMetaDatas.Length > 0)
            {
                judgementMetaData = ruleset.judgementMetaDatas[0];
                return true;
            }

            for (int i = 0; i < ruleset.judgementMetaDatas.Length; i++)
            {
                judgementMetaData = ruleset.judgementMetaDatas[i];

                if (disSecond.Abs() < judgementMetaData.sizeSecond)
                    return true;
            }

            judgementMetaData = ruleset.missJudgementMetaData;
            return forceFastMiss || disSecond >= 0;
        }

        public static double GetScoreAddValue(this IRuleset ruleset, double disSecond, double length, double combo, double comboMultiplier = 1)
        {
            double scoreMultiplier = 1d.Lerp(0, disSecond.Abs() / ruleset.judgementMetaDatas.Last().sizeSecond);

            if (comboMultiplier == 0)
                return 1d / length * scoreMultiplier * maxScore;
            else
            {
                double comboScoreResult = (1d / (1d.ArithmeticSequenceSum(length) / comboMultiplier)) * combo;
                double normalScoreResult = (1d / length).LerpUnclamped(0, comboMultiplier);

                return (comboScoreResult + normalScoreResult) * scoreMultiplier * maxScore;
            }
        }

        /// <returns>
        /// -1 ~ 1
        /// </returns>
        public static double GetAccuracy(this IRuleset ruleset, double disSecond)
        {
            if (disSecond < 0)
                return -0d.Lerp(1, (-disSecond) / ruleset.judgementMetaDatas.Last().sizeSecond); //놀랍게도 이거 (-1).Lerp가 아니라 -(1.Lerp) 판정이다
            else
                return 0d.Lerp(1, disSecond / ruleset.judgementMetaDatas.Last().sizeSecond);
        }

        /// <summary>
        /// 힘들게 만들었지만 사실 안쓴다
        /// 누가 오스 판정 만들면서 쓰갰지 뭐
        /// </summary>
        /// <param name="ruleset"></param>
        /// <param name="disSecond"></param>
        /// <param name="judgementMetaData"></param>
        /// <returns>
        /// -1 ~ 1
        /// </returns>
        public static double GetGenerousAccuracy(this IRuleset ruleset, double disSecond, JudgementMetaData judgementMetaData)
        {
            if (judgementMetaData == ruleset.missJudgementMetaData)
            {
                if (disSecond < 0)
                    return -1;
                else
                    return 1;
            }

            for (int i = 0; i < ruleset.judgementMetaDatas.Length; i++)
            {
                if (judgementMetaData == ruleset.judgementMetaDatas[i])
                {
                    if (i == 0)
                        return 0;
                    else
                    {
                        double t = judgementMetaData.sizeSecond.Lerp(ruleset.judgementMetaDatas[i - 1].sizeSecond, 0.5) / ruleset.judgementMetaDatas.Last().sizeSecond;

                        if (disSecond < 0)
                            return -0d.Lerp(1, t); //놀랍게도 이거 (-1).Lerp가 아니라 -(1.Lerp) 판정이다
                        else
                            return 0d.Lerp(1, t);
                    }
                }
            }

            return 0;
        }

        public static RankMetaData GetRank(this IRuleset ruleset, double accuracyAbs)
        {
            if (accuracyAbs == 0 && ruleset.rankMetaDatas.Length > 0)
                return ruleset.rankMetaDatas[0];

            for (int i = 0; i < ruleset.rankMetaDatas.Length; i++)
            {
                RankMetaData rankMetaData = ruleset.rankMetaDatas[i];

                if (accuracyAbs < rankMetaData.size)
                    return rankMetaData;
            }

            return ruleset.rankMetaDatas.Last();
        }
    }
}
