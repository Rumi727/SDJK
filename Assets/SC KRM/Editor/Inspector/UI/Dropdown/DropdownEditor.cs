using SCKRM.UI;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Dropdown))]
    public class DropdownEditor : UIEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawLine();

            UseProperty("_options", "종류");
            UseProperty("_customLabel", "이름 교체");

            DrawLine(); 

            UseProperty("_value", "선택된 인덱스");

            DrawLine();

            UseProperty("_onValueChanged");

            DrawLine();

            UseProperty("label");
            UseProperty("listRectTransform");
            UseProperty("listTargetSizeFitter");
            UseProperty("template");
            UseProperty("viewport");
            UseProperty("content");
            UseProperty("scrollbar");
            UseProperty("scrollbarHandle");
        }
    }
}