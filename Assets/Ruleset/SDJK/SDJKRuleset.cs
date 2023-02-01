using Cysharp.Threading.Tasks;
using SCKRM.Renderer;
using SCKRM.Scene;
using SDJK.Map;
using SDJK.Ruleset.SDJK.Input;
using SDJK.Ruleset.SDJK.Judgement;
using SDJK.Map.Ruleset.SDJK.Map;
using UnityEngine;
using SDJK.Replay;

namespace SDJK.Ruleset.SDJK
{
    public sealed class SDJKRuleset : RulesetBase
    {
        public override int order => int.MinValue;

        public override string name => "sdjk";
        public override string[] compatibleRulesets => new string[] { "adofai" };

        public override JudgementMetaData[] judgementMetaDatas { get; } = new JudgementMetaData[]
        {
            new JudgementMetaData(sick, 0.008, new Color32(0, 220, 255, 255)),
            new JudgementMetaData(perfect, 0.032, new Color32(0, 170, 255, 255), 0.75),
            new JudgementMetaData(great, 0.064, new Color32(100, 255, 100, 255), 0.5),
            new JudgementMetaData(good, 0.128, new Color32(230, 230, 0, 255), 0.25),
            new JudgementMetaData(early, 0.256, new Color32(166, 166, 166, 255), 0.5, true)
        };
        public override JudgementMetaData missJudgementMetaData { get; } = new JudgementMetaData(miss, double.MaxValue, new Color32(255, 50, 50, 255), 1, true);
        public JudgementMetaData instantDeathJudgementMetaData { get; } = new JudgementMetaData(instantDeath, double.MaxValue, new Color32(255, 50, 50, 255), double.MaxValue, true);

        public const string sick = "ruleset.sdjk.sick";
        public const string perfect = "ruleset.sdjk.perfect";
        public const string great = "ruleset.sdjk.great";
        public const string good = "ruleset.sdjk.good";
        public const string early = "ruleset.sdjk.early";
        public const string miss = "ruleset.sdjk.miss";
        public const string instantDeath = "ruleset.sdjk.instantDeath";

        public override NameSpaceIndexTypePathPair icon => new NameSpaceIndexTypePathPair("sdjk", "ruleset/sdjk", "icon");

        public override async void GameStart(string mapFilePath, string replayFilePath, bool isEditor)
        {
            base.GameStart(mapFilePath, replayFilePath, isEditor);

            await SceneManager.LoadScene(3);
            await UniTask.NextFrame();

            SDJKMapFile map = MapLoader.MapLoad<SDJKMapFile>(mapFilePath);
            SDJKReplayFile replay = null;
            if (replayFilePath != null)
                replay = ReplayLoader.ReplayLoad<SDJKReplayFile>(replayFilePath);

            Object.FindObjectOfType<SDJKManager>(true).Refresh(map, replay, this, isEditor);
            Object.FindObjectOfType<SDJKInputManager>(true).Refresh();
            Object.FindObjectOfType<SDJKJudgementManager>(true).Refresh();

            //나중에 다시 필요할 수도...
            /*System.Collections.Generic.List<SCKRM.Rhythm.BeatValuePairAni<double>> beats = new System.Collections.Generic.List<SCKRM.Rhythm.BeatValuePairAni<double>>();
            for (int i = 0; i < map.allJudgmentBeat[map.allJudgmentBeat.Count - 1]; i++)
            {
                if (map.globalEffect.yukiMode.GetValue(i))
                {
                    SCKRM.Rhythm.BeatValuePairAni<double> asdf = new SCKRM.Rhythm.BeatValuePairAni<double>() { beat = i, length = 0, value = 0.98f, easingFunction = SCKRM.Easing.EasingFunction.Ease.Linear };
                    map.globalEffect.cameraZoom.Add(asdf);
                    beats.Add(asdf);
                }
            }

            for (int i = 0; i < map.effect.fieldEffect[0].barEffect.Count; i++)
                map.effect.fieldEffect[0].barEffect[i].noteSpeed.Add(double.MinValue, 1);

            for (double i = 0; i < map.allJudgmentBeat[map.allJudgmentBeat.Count - 1]; i += 0.125f)
            {
                if (map.globalEffect.yukiMode.GetValue(i))
                {
                    for (int j = 0; j < map.notes.Count; j++)
                    {
                        SDJKNoteFile? note = map.notes[j].Find(x => SCKRM.MathTool.Distance(x.beat, i) <= SCKRM.MathTool.epsilonFloatWithAccuracy);
                        if (note != null)
                        {
                            SCKRM.Rhythm.BeatValuePairList<double> effect = map.effect.fieldEffect[0].barEffect[j].noteSpeed;
                            if (note.Value.holdLength > 0)
                            {
                                effect.Add(i, 0.5, true);
                                effect.Add(i + 0.125, 1, true);
                            }
                            else if (i >= 496 && i <= 560)
                            {
                                effect.Add(i, Random.Range(0.5f, 1.5f), true);
                                effect.Add(i + 0.125, 1, true);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < map.effect.fieldEffect[0].barEffect.Count; i++)
                map.effect.fieldEffect[0].barEffect[i].noteSpeed.Add(560, 1);

            map.globalEffect.cameraZoom.Sort((a, b) => a.beat.CompareTo(b.beat));

            for (int i = 0; i < beats.Count; i++)
            {
                SCKRM.Rhythm.BeatValuePairAni<double> asdf = new SCKRM.Rhythm.BeatValuePairAni<double>() { beat = beats[i].beat, length = 2, value = 1, easingFunction = SCKRM.Easing.EasingFunction.Ease.EaseOutExpo };
                map.globalEffect.cameraZoom.Insert(map.globalEffect.cameraZoom.IndexOf(beats[i]) + 1, asdf);
            }

            System.IO.File.WriteAllText(mapFilePath, SCKRM.Json.JsonManager.ObjectToJson(map));*/
        }
    }
}
