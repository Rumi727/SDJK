using Cysharp.Threading.Tasks;
using SCKRM.Renderer;
using SCKRM.UI.Overlay.MessageBox;
using SDJK.Mode;

namespace SDJK.Ruleset.ADOFAI
{
    public sealed class ADOFAIRuleset : RulesetBase
    {
        public override int order { get; } = int.MaxValue;
        public override string name { get; } = "adofai";

        public override NameSpaceIndexTypePathPair icon { get; } = new NameSpaceIndexTypePathPair("sdjk", "ruleset/adofai", "icon");

        public override RankMetaData[] rankMetaDatas { get; }

        public override JudgementMetaData[] judgementMetaDatas => null;
        public override JudgementMetaData missJudgementMetaData { get; }

        public override void GameStart(string mapFilePath, string replayFilePath, bool isEditor, params IMode[] modes) => MessageBoxManager.Show("sc-krm:gui.ok", 0, "sdjk:ruleset.adofai.unplayable", "sc-krm:0:gui/icon/exclamation_mark").Forget();
    }
}
