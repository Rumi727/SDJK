using SCKRM.Object;
using SDJK.Map;
using SDJK.Map.Ruleset.SDJK.Map;
using SDJK.Mode;
using SDJK.Replay;
using SDJK.Replay.Ruleset.SDJK;
using SDJK.Ruleset.SDJK.GameOver;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public sealed class SDJKManager : RulesetManagerBase
    {
        [SerializeField] SDJKGameOverManager _gameOverManager; public SDJKGameOverManager gameOverManager => _gameOverManager;

        public new SDJKRuleset ruleset { get; private set; } = null;
        public new SDJKMapFile map { get; private set; } = null;

        public new SDJKReplayFile currentReplay { get; private set; } = null;
        public new SDJKReplayFile createdReplay { get; private set; } = null;

        public override bool isGameOver => gameOverManager.isGameOver;

        public override bool Refresh(MapFile map, ReplayFile replay, IRuleset ruleset, bool isEditor, IMode[] modes)
        {
            if (map is not SDJKMapFile || (replay != null && replay is not SDJKReplayFile) || ruleset is not SDJKRuleset)
                return false;

            if (base.Refresh(map, replay, ruleset, isEditor, modes))
            {
                this.ruleset = (SDJKRuleset)base.ruleset;
                this.map = (SDJKMapFile)base.map;

                currentReplay = (SDJKReplayFile)base.currentReplay;
                createdReplay = (SDJKReplayFile)base.createdReplay;

                for (int i = 0; i < this.map.effect.fieldEffect.Count; i++)
                {
                    PlayField playField = (PlayField)ObjectPoolingSystem.ObjectCreate("ruleset.sdjk.play_field", transform).monoBehaviour;
                    playField.Refresh(i);
                }

                return true;
            }

            return false;
        }
    }
}
