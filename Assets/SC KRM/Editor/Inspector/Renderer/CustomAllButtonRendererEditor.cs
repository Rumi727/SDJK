using UnityEditor;
using SCKRM.Renderer;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CustomSelectableRenderer), true)]
    public class CustomAllButtonRendererEditor : CustomAllSpriteRendererEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawLine();

            UseProperty("_highlightedSprite");
            UseProperty("_pressedSprite");
            UseProperty("_selectedSprite");
            UseProperty("_disabledSprite");
        }
    }
}