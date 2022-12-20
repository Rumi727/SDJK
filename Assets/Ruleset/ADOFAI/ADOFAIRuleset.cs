using SCKRM.Renderer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Ruleset.ADOFAI
{
    public sealed class ADOFAIRuleset : Ruleset
    {
        public override NameSpaceIndexTypePathPair icon => new NameSpaceIndexTypePathPair("sdjk", "ruleset/adofai", "icon");
        public override JudgementMetaData[] judgementMetaDatas => null;
        public override JudgementMetaData missJudgementMetaData { get; }

        public override void GameStart(string mapFilePath, bool isEditor) { }
    }
}
