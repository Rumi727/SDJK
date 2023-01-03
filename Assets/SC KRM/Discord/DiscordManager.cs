using DiscordPresence;
using System;
using System.Diagnostics;

namespace SCKRM.Discord
{
    [WikiDescription("디스코드 API를 관리하는 클래스 입니다")]
    public sealed class DiscordManager : Manager<DiscordManager>
    {
        public static PresenceManager presenceManager => PresenceManager.instance;
        public static DiscordRpc.RichPresence presence => presenceManager.presence;

        void Awake() => SingletonCheck(this);

#if UNITY_STANDALONE
        static float timer = 0;
        void Update()
        {
            if (timer >= 4)
            {
                updateAction?.Invoke();
                timer = 0;
            }
            else
                timer += Kernel.unscaledDeltaTime;
        }
#endif

        static Action updateAction;
        [Conditional("UNITY_EDITOR"), Conditional("UNITY_STANDALONE")]
        public static void UpdatePresence(string detail, string state = null, long start = -1, long end = -1, string largeKey = null, string largeText = null,
            string smallKey = null, string smallText = null, string partyId = null, int size = -1, int max = -1, string match = null, string join = null,
            string spectate = null)
        {
            updateAction = () => PresenceManager.UpdatePresence(detail, state, start, end, largeKey, largeText, smallKey, smallText, partyId, size, max, match, join, spectate);
        }
    }
}
