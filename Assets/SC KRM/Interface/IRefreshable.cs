namespace SCKRM
{
    public interface IRefreshable
    {
        /// <summary>
        /// CustomRendererManager에서 다른 스레드가 호출 할 수 있는 함수입니다. 유니티 API를 사용해선 안됩니다. (플레이 모드가 아닐땐 제외)
        /// You should not use the Unity API (Except when not in play mode)
        /// </summary>
        void Refresh();
    }

    public interface IRendererRefreshable : IRefreshable
    {

    }

    public interface ITextRefreshable : IRefreshable
    {

    }
}
