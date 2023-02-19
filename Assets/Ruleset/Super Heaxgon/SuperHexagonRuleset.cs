using Cysharp.Threading.Tasks;
using SCKRM.Renderer;
using SCKRM.Scene;
using UnityEngine;
using SDJK.Mode;
using SDJK.Replay.Ruleset.SuperHexagon;
using SDJK.Map.Ruleset.SuperHexagon.Map;
using SDJK.Replay;
using SDJK.Map;
using SDJK.Mode.Automatic;
using SCKRM.UI.Overlay.MessageBox;
using SDJK.Ruleset.SuperHexagon.Judgement;
using SDJK.Ruleset.SuperHexagon.GameOver;

namespace SDJK.Ruleset.SuperHexagon
{
    public sealed class SuperHexagonRuleset : RulesetBase
    {
        public override int order { get; } = 0;

        public override string name { get; } = "super_hexagon";
        public override string[] compatibleRulesets { get; } = new string[] { "sdjk", "adofai" };

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

        public override JudgementMetaData[] judgementMetaDatas { get; } = new JudgementMetaData[] { new JudgementMetaData(perfect, double.MaxValue, Color.white, 0.75) };
        public override JudgementMetaData missJudgementMetaData { get; } = new JudgementMetaData(miss, double.MaxValue, Color.white, 1, true);

        public const string perfect = "ruleset.super_hexagon.perfect";
        public const string miss = "ruleset.super_hexagon.miss";

        public override NameSpaceIndexTypePathPair icon => new NameSpaceIndexTypePathPair("sdjk", "ruleset/super_hexagon", "icon");

        public override async void GameStart(string mapFilePath, string replayFilePath, bool isEditor, params IMode[] modes)
        {
            if (modes == null)
                modes = IMode.emptyModes;

            base.GameStart(mapFilePath, replayFilePath, isEditor, modes);

            await SceneManager.LoadScene(4);
            await UniTask.NextFrame();

            SuperHexagonReplayFile replay = null;
            if (replayFilePath != null)
                replay = ReplayLoader.ReplayLoad<SuperHexagonReplayFile>(replayFilePath, out modes);

            SuperHexagonMapFile map = MapLoader.MapLoad<SuperHexagonMapFile>(mapFilePath, modes);

            if (modes.FindMode<AutoModeBase>() != null)
                replay = GetAutoModeReplayFile(map, modes);

            Object.FindObjectOfType<SuperHexagonManager>(true).Refresh(map, replay, this, isEditor, modes);
            Object.FindObjectOfType<SuperHexagonJudgementManager>(true).Refresh();
            Object.FindObjectOfType<SuperHexagonGameOverManager>(true).Refresh();
        }

        static SuperHexagonReplayFile GetAutoModeReplayFile(SuperHexagonMapFile map, params IMode[] modes)
        {
            return ReplayLoader.CreateReplay<SuperHexagonReplayFile>(map, modes);

            /*for (int i = 0; i < map.notes.Count; i++)
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

            return replay;*/
        }
    }
}
