using UnityEditor;
using SCKRM.Renderer;
using SCKRM.Resource;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CustomSpriteRendererBase), true)]
    public class CustomAllSpriteRendererEditor : CustomInspectorEditor
    {
        CustomSpriteRendererBase editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (CustomSpriteRendererBase)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            editor.nameSpace = UsePropertyAndDrawNameSpace("_nameSpace", "네임스페이스", editor.nameSpace);

            string[] types = ResourceManager.GetSpriteTypes(editor.nameSpace);
            if (types == null)
                return;

            editor.type = UsePropertyAndDrawStringArray("_type", "타입", editor.type, types);

            string[] spriteKeys = ResourceManager.GetSpriteKeys(editor.type, editor.nameSpace);
            if (spriteKeys == null)
                return;

            editor.path = UsePropertyAndDrawStringArray("_path", "이름", editor.path, spriteKeys);

            UseProperty("_spriteTag", "태그");

            EditorGUILayout.Space();

            UseProperty("_index", "스프라이트 인덱스");

            SpriteProjectSetting.DrawGUI(editor.nameSpace, editor.type, editor.path, editor.spriteTag, editor.index);
        }
    }
}