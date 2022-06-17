using System.Collections.Generic;
using UnityEngine;

namespace SCKRM.UI.SideBar
{
    [RequireComponent(typeof(RectTransform))]
    public static class SideBarManager
    {
        public static List<SideBarAni> showedSideBars { get; } = new List<SideBarAni>();
        public static bool isSideBarShow => showedSideBars.Count > 0;

        public static void AllHide()
        {
            for (int i = 0; i < showedSideBars.Count; i++)
                showedSideBars[i].Hide();
        }
    }
}