using SCKRM;

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
    }
}
