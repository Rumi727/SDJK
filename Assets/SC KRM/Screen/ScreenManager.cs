using SCKRM.Window;
using UnityEngine;

namespace SCKRM
{
    [AddComponentMenu("SC KRM/Screen/Screen Manager")]
    [WikiDescription("화면을 관리하는 클래스 입니다")]
    public sealed class ScreenManager : ManagerBase<ScreenManager>
    {
        static int _width; [WikiDescription("화면 폭")] public static int width
        {
            get
            {
                if (Kernel.isPlaying)
                    return _width;
                else
                    return Screen.width;
            }
        }
        static int _height; [WikiDescription("화면 높이")] public static int height
        {
            get
            {
                if (Kernel.isPlaying)
                    return _height;
                else
                    return Screen.height;
            }
        }

        static Resolution _currentResolution; [WikiDescription("현재 모니터 해상도")] public static Resolution currentResolution
        {
            get
            {
                if (Kernel.isPlaying)
                    return _currentResolution;
                else
                    return Screen.currentResolution;
            }
        }
        static Resolution[] _resolutions;  [WikiDescription("현재 모니터가 지원하는 모든 전체화면 해상도")] public static Resolution[] resolutions
        {
            get
            {
                if (Kernel.isPlaying)
                    return _resolutions;
                else
                    return Screen.resolutions;
            }
        }

        void Awake()
        {
            if (SingletonCheck(this))
                ResolutionRefresh();
        }

        void Update()
        {
            _width = Screen.width;
            _height = Screen.height;
        }

        [WikiDescription("해상도 새로고침")]
        public static void ResolutionRefresh()
        {
            _currentResolution = Screen.currentResolution;
            _resolutions = Screen.resolutions;
        }
    }
}
