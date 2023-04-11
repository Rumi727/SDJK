using SCKRM;
using SDJK.Effect;
using SDJK.Map;
using SDJK.Mode;
using SDJK.Ruleset;
using UnityEngine;

namespace SDJK.MainMenu
{
    public class MainMenuEffectManager : MonoBehaviour
    {
        [SerializeField] EffectManager effectManager;

        void OnEnable()
        {
            RulesetManager.rulesetChanged += RulesetChange;
            ModeManager.modeRefresh += ModeChange;
        }

        void OnDisable()
        {
            RulesetManager.rulesetChanged += RulesetChange;
            ModeManager.modeRefresh += ModeChange;
        }

        void RulesetChange() => effectManager.selectedRuleset = RulesetManager.selectedRuleset;
        void ModeChange() => effectManager.selectedModes = ModeManager.selectedModeList.ToArray();

        MapFile lastMap;
        void Update()
        {
            if (MapManager.selectedMap != null && BGMManager.bgm != null && BGMManager.bgm.soundPlayer != null && lastMap != MapManager.selectedMap)
            {
                effectManager.selectedMapPack = MapManager.selectedMapPack;
                effectManager.selectedMap = MapManager.selectedMap;

                effectManager.soundPlayer = BGMManager.bgm.soundPlayer;

                effectManager.AllRefresh(false);
                lastMap = MapManager.selectedMap;
            }
        }
    }
}
