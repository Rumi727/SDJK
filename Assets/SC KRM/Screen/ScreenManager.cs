using UnityEngine;

namespace SCKRM
{
    [AddComponentMenu("SC KRM/Screen/Screen Manager")]
    public sealed class ScreenManager : Manager<ScreenManager>
    {
        public static int width { get; private set; }
        public static int height { get; private set; }

        public static Resolution currentResolution { get; private set; }

        void Awake() => SingletonCheck(this);

        void Update()
        {
            width = Screen.width;
            height = Screen.height;

            currentResolution = Screen.currentResolution;
        }
    }
}
