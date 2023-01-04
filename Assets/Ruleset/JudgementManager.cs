using SCKRM;
using System.Linq;

namespace SDJK.Ruleset
{
    public static class JudgementManager
    {
        public static bool Judgement(this IRuleset ruleset, double disSecond, bool forceFastMiss, out JudgementMetaData judgementMetaData)
        {
            for (int i = 0; i < ruleset.judgementMetaDatas.Length; i++)
            {
                judgementMetaData = ruleset.judgementMetaDatas[i];

                if (disSecond.Abs() < judgementMetaData.sizeSecond)
                    return true;
            }

            judgementMetaData = ruleset.missJudgementMetaData;
            return forceFastMiss || disSecond >= 0;
        }

        public static double GetScoreAddValue(this IRuleset ruleset, double disSecond, double length, bool allowComboMultiplier = true)
        {
            double scoreMultiplier = 1d.Lerp(0, disSecond.Abs() / ruleset.judgementMetaDatas.Last().sizeSecond);

            if (allowComboMultiplier)
                return length / 1d.ArithmeticSequenceSum(length) / length * scoreMultiplier;
            else
                return 1d / length * scoreMultiplier;
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
    }
}
