using SCKRM.Renderer;
using SDJK.Mode;
using System;

namespace SDJK.Ruleset.OsuMania
{
    public sealed class OsuManiaRuleset : RulesetBase
    {
        public override int order { get; } = int.MaxValue;

        public override string name { get; } = "osu!mania";
        public override string displayName { get; } = "osu!mania";

        public override NameSpaceIndexTypePathPair icon => new NameSpaceIndexTypePathPair("sdjk", "ruleset/sdjk", "icon");
        public override string discordIconKey => "ruleset_sdjk";

        public override bool hidden => true;

        public override RankMetaData[] rankMetaDatas => null;

        public override JudgementMetaData[] judgementMetaDatas => null;
        public override JudgementMetaData missJudgementMetaData { get; }

        public override void GameStart(string mapFilePath, string replayFilePath, bool isEditor, params IMode[] modes) => throw new NotImplementedException();
    }
}
