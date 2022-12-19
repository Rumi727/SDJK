using Cysharp.Threading.Tasks;
using SCKRM.Renderer;
using SCKRM.Scene;
using SDJK.Map;
using SDJK.Ruleset.SDJK.Input;
using SDJK.Ruleset.SDJK.Judgement;
using SDJK.Ruleset.SDJK.Map;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public sealed class SDJKRuleset : Ruleset
    {
        public override JudgementMetaData[] judgementMetaDatas { get; } = new JudgementMetaData[]
        {
            new JudgementMetaData(sick, 0.01),
            new JudgementMetaData(perfect, 0.02),
            new JudgementMetaData(great, 0.04, 0.75),
            new JudgementMetaData(good, 0.08, 0.5),
            new JudgementMetaData(early, 0.16, 0.5, true)
        };
        public override JudgementMetaData missJudgementMetaData { get; } = new JudgementMetaData(miss, double.MaxValue, 1, true);
        public JudgementMetaData instantDeathJudgementMetaData { get; } = new JudgementMetaData(instantDeath, double.MaxValue, double.MaxValue, true);

        public const string sick = "ruleset.sdjk.sick";
        public const string perfect = "ruleset.sdjk.perfect";
        public const string great = "ruleset.sdjk.great";
        public const string good = "ruleset.sdjk.good";
        public const string early = "ruleset.sdjk.early";
        public const string miss = "ruleset.sdjk.miss";
        public const string instantDeath = "ruleset.sdjk.instantDeath";

        public override NameSpaceIndexTypePathPair icon => new NameSpaceIndexTypePathPair("sdjk", "ruleset/sdjk", "icon");

        public override async void GameStart(string mapFilePath)
        {
            base.GameStart(mapFilePath);

            await SceneManager.LoadScene(3);
            await UniTask.NextFrame();

            SDJKMapFile map = MapLoader.MapLoad<SDJKMapFile>(mapFilePath);
            Object.FindObjectOfType<SDJKManager>().Refresh(map, this);
            Object.FindObjectOfType<SDJKInputManager>().Refresh();
            Object.FindObjectOfType<SDJKJudgementManager>().Refresh();
        }
    }
}
