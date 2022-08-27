using SCKRM.Window;
using UnityEngine;

namespace SCKRM
{
    [AddComponentMenu("SC KRM/Screen/Screen Manager")]
    [WikiDescription("화면을 관리하는 클래스 입니다")]
    public sealed class ScreenManager : Manager<ScreenManager>
    {
        [WikiDescription("화면 폭")] public static int width { get; private set; }
        [WikiDescription("화면 높이")] public static int height { get; private set; }

        [WikiDescription("현재 모니터 해상도")] public static Resolution currentResolution { get; private set; }
        [WikiDescription("모든 모니터 해상도")] public static Resolution[] resolutions { get; private set; }

        void Awake()
        {
            if (SingletonCheck(this))
                ResolutionRefresh();
        }

        void Update()
        {
            width = Screen.width;
            height = Screen.height;
        }

        [WikiDescription("해상도 새로고침")]
        public static void ResolutionRefresh()
        {
            currentResolution = Screen.currentResolution;
            resolutions = Screen.resolutions;
        }
    }
}
