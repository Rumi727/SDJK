using Cysharp.Threading.Tasks;
using SCKRM.Renderer;
using SCKRM.Scene;
using SDJK.Map;
using SDJK.Ruleset.SDJK.Input;
using SDJK.Ruleset.SDJK.Judgement;
using SDJK.Map.Ruleset.SDJK.Map;
using UnityEngine;
using SDJK.Replay;
using SDJK.Mode;
using System.Collections.Generic;
using SDJK.Mode.Automatic;
using SDJK.Replay.Ruleset.SDJK;
using SDJK.Ruleset.SDJK.GameOver;

namespace SDJK.Ruleset.SDJK
{
    public sealed class SDJKRuleset : RulesetBase
    {
        public override int order { get; } = int.MinValue;

        public override string name { get; } = "sdjk";
        public override string[] compatibleRulesets { get; } = new string[] { "adofai" };

        public override RankMetaData[] rankMetaDatas { get; } = new RankMetaData[]
        {
            new RankMetaData("SS", 0, new Color32(255, 0, 255, 255)),
            new RankMetaData("S", 0.0625, new Color32(0, 220, 255, 255)),
            new RankMetaData("A", 0.125, new Color32(0, 170, 255, 255)),
            new RankMetaData("B", 0.25, new Color32(100, 255, 100, 255)),
            new RankMetaData("C", 0.5, new Color32(230, 230, 0, 255)),
            new RankMetaData("F", 0.9, new Color32(166, 166, 166, 255)),
            new RankMetaData("...", 1, new Color32(255, 255, 255, 255))
        };

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

        public override async void GameStart(string mapFilePath, string replayFilePath, bool isEditor, params IMode[] modes)
        {
            if (modes == null)
                modes = IMode.emptyModes;

            base.GameStart(mapFilePath, replayFilePath, isEditor, modes);

            await SceneManager.LoadScene(3);
            await UniTask.NextFrame();

            SDJKReplayFile replay = null;
            if (replayFilePath != null)
                replay = ReplayLoader.ReplayLoad<SDJKReplayFile>(replayFilePath, out modes);

            SDJKMapFile map = MapLoader.MapLoad<SDJKMapFile>(mapFilePath, modes);

            if (modes.FindMode<AutoModeBase>() != null)
                replay = GetAutoModeReplayFile(map, modes);

            Object.FindObjectOfType<SDJKManager>(true).Refresh(map, replay, this, isEditor, modes);
            Object.FindObjectOfType<SDJKInputManager>(true).Refresh();
            Object.FindObjectOfType<SDJKJudgementManager>(true).Refresh();
            Object.FindObjectOfType<SDJKGameOverManager>(true).Refresh();

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

        static SDJKReplayFile GetAutoModeReplayFile(SDJKMapFile map, params IMode[] modes)
        {
            SDJKReplayFile replay = ReplayLoader.CreateReplay<SDJKReplayFile>(map, modes);

            for (int i = 0; i < map.notes.Count; i++)
            {
                replay.pressBeat.Add(new List<double>());
                replay.pressUpBeat.Add(new List<double>());

                List<SDJKNoteFile> notes = map.notes[i];
                for (int j = 0; j < notes.Count; j++)
                {
                    SDJKNoteFile note = notes[j];
                    if (note.type == SDJKNoteTypeFile.instantDeath || note.type == SDJKNoteTypeFile.auto)
                        continue;

                    replay.pressBeat[i].Add(note.beat);
                    replay.pressUpBeat[i].Add(note.beat + note.holdLength);
                }
            }

            return replay;
        }
    }
}
