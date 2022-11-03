using SCKRM;
using SCKRM.Input;
using SDJK.Effect;
using SDJK.Ruleset.SDJK.Map;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public sealed class SDJKInputManager : Manager<SDJKInputManager>
    {
        [SerializeField] EffectManager _effectManager; public EffectManager effectManager => _effectManager;
        public SDJKMapFile map => (SDJKMapFile)effectManager.selectedMap;



        public void Refresh() => SingletonCheck(this);



        public const string inputKey = "ruleset.sdjk.";
        public string GetInputKey(int keyIndex) => inputKey + map.notes.Count + "." + keyIndex;

        public bool GetKey(int keyIndex, InputType inputType = InputType.Down) => InputManager.GetKey(GetInputKey(keyIndex), inputType);
    }
}
