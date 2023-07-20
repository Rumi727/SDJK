using Cysharp.Threading.Tasks;
using SCKRM.Renderer;
using SCKRM.Scene;
using UnityEngine;
using SDJK.Mode;
using SDJK.Replay.Ruleset.SuperHexagon;
using SDJK.Map.Ruleset.SuperHexagon.Map;
using SDJK.Replay;
using SDJK.Map;
using SDJK.Ruleset.SuperHexagon.Judgement;
using SDJK.Ruleset.SuperHexagon.GameOver;
using SCKRM.Sound;
using SCKRM;

namespace SDJK.Ruleset.SuperHexagon
{
    public sealed class SuperHexagonRuleset : RulesetBase
    {
        public override int order { get; } = 0;

        public override string name { get; } = "super_hexagon";
        public override string displayName { get; } = "Super Hexagon";

        //public override string[] compatibleRulesets { get; } = new string[] { "sdjk", "adofai", "osu!mania" };

        public override RankMetaData[] rankMetaDatas { get; } = new RankMetaData[]
        {
            new RankMetaData("HEXAGON", 0, Color.white),
            new RankMetaData("PENTAGON", 1d - (45d / 60d), Color.white),
            new RankMetaData("SQUARE", 1d - (30d / 60d), Color.white),
            new RankMetaData("TRIANGLE", 1d - (20d / 60d), Color.white),
            new RankMetaData("LINE", 1d - (10d / 60d), Color.white),
            new RankMetaData("POINT", 1, Color.white)
        };

        public override JudgementMetaData[] judgementMetaDatas { get; } = new JudgementMetaData[] { new JudgementMetaData(perfect, double.MaxValue, Color.white, 0.75) };
        public override JudgementMetaData missJudgementMetaData { get; } = new JudgementMetaData(miss, double.MaxValue, Color.white, 1, true);

        public const string perfect = "ruleset.super_hexagon.perfect";
        public const string miss = "ruleset.super_hexagon.miss";

        public override NameSpaceIndexTypePathPair icon => new NameSpaceIndexTypePathPair("sdjk", "ruleset/super_hexagon", "icon");
        public override string discordIconKey => "ruleset_super_hexagon";

        public override async void GameStart(string mapFilePath, string replayFilePath, bool isEditor, params IMode[] modes)
        {
            if (modes == null)
                modes = IMode.emptyModes;

            base.GameStart(mapFilePath, replayFilePath, isEditor, modes);

            SuperHexagonMapFile map = null;
            SuperHexagonReplayFile replay = null;

            await SceneManager.LoadScene(4, () => UniTask.RunOnThreadPool(MapLoad));
            await UniTask.NextFrame();

            void MapLoad()
            {
                replay = null;
                if (replayFilePath != null)
                    replay = ReplayLoader.ReplayLoad<SuperHexagonReplayFile>(replayFilePath, out modes);

                map = MapLoader.MapLoad<SuperHexagonMapFile>(mapFilePath, false, modes);
            }

            if (!Kernel.isPlaying)
                return;

            /*if (modes.FindMode<AutoModeBase>() != null)
                replay = GetAutoModeReplayFile(map, modes);*/

            Object.FindObjectOfType<SuperHexagonManager>(true).Refresh(map, replay, this, isEditor, modes);
            Object.FindObjectOfType<SuperHexagonJudgementManager>(true).Refresh();
            Object.FindObjectOfType<SuperHexagonGameOverManager>(true).Refresh();

            SoundManager.PlaySound("ruleset.super_hexagon.start", "sdjk");
            SoundManager.PlaySound("ruleset.super_hexagon.begin", "sdjk");
        }

        /*static SuperHexagonReplayFile GetAutoModeReplayFile(SuperHexagonMapFile map, params IMode[] modes)
        {
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
        }*/
    }
}
