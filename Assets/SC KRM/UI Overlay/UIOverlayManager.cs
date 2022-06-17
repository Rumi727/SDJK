using System.Collections.Generic;

namespace SCKRM.UI.Overlay
{
    public sealed class UIOverlayManager : Manager<UIOverlayManager>
    {
        public static List<IUIOverlay> showedOverlays { get; } = new List<IUIOverlay>();
        public static bool isOverlayShow => showedOverlays.Count > 0;
    }

    public interface IUIOverlay
    {

    }
}
