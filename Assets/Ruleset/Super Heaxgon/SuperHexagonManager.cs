using SDJK.Map;
using SDJK.Map.Ruleset.SuperHexagon.Map;
using SDJK.Mode;
using SDJK.Replay;
using SDJK.Replay.Ruleset.SuperHexagon;
using SDJK.Ruleset.SuperHexagon.GameOver;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon
{
    public sealed class SuperHexagonManager : RulesetManagerBase<SuperHexagonRuleset, SuperHexagonMapFile, SuperHexagonReplayFile>
    {
        [SerializeField] SuperHexagonGameOverManager _gameOverManager; public SuperHexagonGameOverManager gameOverManager => _gameOverManager;
        [SerializeField] Field _field; public Field field => _field;

        public override bool isGameOver => gameOverManager.isGameOver;

        public override bool Refresh(SuperHexagonMapFile map, SuperHexagonReplayFile replay, SuperHexagonRuleset ruleset, bool isEditor, IMode[] modes)
        {
            if (base.Refresh(map, replay, ruleset, isEditor, modes))
            {
                field.Refresh();
                return true;
            }

            return false;
        }
    }
}
