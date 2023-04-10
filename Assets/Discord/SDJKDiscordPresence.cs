using SCKRM.Discord;
using SDJK.Effect;
using UnityEngine;

namespace SDJK
{
    public sealed class SDJKDiscordPresence : MonoBehaviour
    {
        [SerializeField] EffectManager _effectManager;
        public EffectManager effectManager => _effectManager;

        void Update() => DiscordManager.UpdateActivity();
    }
}
