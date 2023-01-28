using Cysharp.Threading.Tasks;
using SCKRM.Renderer;
using SCKRM.UI.Overlay.MessageBox;

namespace SDJK.Ruleset.ADOFAI
{
    public sealed class ADOFAIRuleset : Ruleset
    {
        public override int order => int.MaxValue;
        public override string name => "adofai";

        public override NameSpaceIndexTypePathPair icon => new NameSpaceIndexTypePathPair("sdjk", "ruleset/adofai", "icon");
        public override JudgementMetaData[] judgementMetaDatas => null;
        public override JudgementMetaData missJudgementMetaData { get; }

        public override void GameStart(string mapFilePath, string replayFilePath, bool isEditor) => MessageBoxManager.Show("sc-krm:gui.ok", 0, "sdjk:ruleset.adofai.unplayable", "sc-krm:0:gui/icon/exclamation_mark").Forget();
    }
}
