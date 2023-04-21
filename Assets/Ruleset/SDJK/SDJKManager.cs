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
    public sealed class SDJKManager : RulesetManagerBase<SDJKRuleset, SDJKMapFile, SDJKReplayFile>
    {
        [SerializeField] SDJKGameOverManager _gameOverManager; public SDJKGameOverManager gameOverManager => _gameOverManager;
        public override bool isGameOver => gameOverManager.isGameOver;

        public override bool Refresh(SDJKMapFile map, SDJKReplayFile replay, SDJKRuleset ruleset, bool isEditor, IMode[] modes)
        {
            if (base.Refresh(map, replay, ruleset, isEditor, modes))
            {
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
