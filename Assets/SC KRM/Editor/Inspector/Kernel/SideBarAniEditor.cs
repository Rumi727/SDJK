using SCKRM.UI.SideBar;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SideBarAni))]
    public class SideBarAniEditor : UIAniEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawLine();

            UseProperty("_showControlKey", "단축키");
            UseProperty("_inputLockName", "입력 잠금 키");

            Space();

            UseProperty("_showEvent");
            UseProperty("_hideEvent");
            UseProperty("_backEvent");

            Space();

            UseProperty("_viewPort");
            UseProperty("_content");
            UseProperty("_scrollBarParentRectTransform");
            UseProperty("_scrollBar");
        }
    }
}