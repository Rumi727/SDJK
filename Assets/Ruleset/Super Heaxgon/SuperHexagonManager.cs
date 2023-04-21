using SDJK.Map;
using SDJK.Map.Ruleset.SuperHexagon.Map;
using SDJK.Mode;
using SDJK.Replay;
using SDJK.Replay.Ruleset.SuperHexagon;
using SDJK.Ruleset.SuperHexagon.GameOver;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon
{
    public sealed class SuperHexagonManager : RulesetManagerBase
    {
        [SerializeField] SuperHexagonGameOverManager _gameOverManager; public SuperHexagonGameOverManager gameOverManager => _gameOverManager;

        public new SuperHexagonRuleset ruleset { get; private set; } = null;
        public new SuperHexagonMapFile map { get; private set; } = null;

        public new SuperHexagonReplayFile currentReplay { get; private set; } = null;
        public new SuperHexagonReplayFile createdReplay { get; private set; } = null;

        public override bool isGameOver => gameOverManager.isGameOver;

        public override bool Refresh(MapFile map, ReplayFile replay, IRuleset ruleset, bool isEditor, IMode[] modes)
        {
            if (map is not SuperHexagonMapFile || (replay != null && replay is not SuperHexagonReplayFile) || ruleset is not SuperHexagonRuleset)
                return false;

            if (base.Refresh(map, replay, ruleset, isEditor, modes))
            {
                this.ruleset = (SuperHexagonRuleset)base.ruleset;
                this.map = (SuperHexagonMapFile)base.map;

                currentReplay = (SuperHexagonReplayFile)base.currentReplay;
                createdReplay = (SuperHexagonReplayFile)base.createdReplay;

                return true;
            }

            return false;
        }
    }
}
