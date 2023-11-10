using SCKRM.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AlphaHitTestMinimumThreshold))]
    public class AlphaHitTestMinimumThresholdEditor : UIEditor
    {
        [System.NonSerialized] AlphaHitTestMinimumThreshold editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (AlphaHitTestMinimumThreshold)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UseProperty("_alphaHitTestMinimumThreshold");
            if (GUI.changed)
                editor.alphaHitTestMinimumThreshold = editor.alphaHitTestMinimumThreshold;
            
            if (editor.image != null && editor.image.GetType() != typeof(FixRaycastImage) && GUILayout.Button("레이캐스트 코드가 수정된 이미지 컴포넌트로 교채"))
            {
                Sprite sprite = editor.image.sprite;
                Color color = editor.image.color;
                Material material = editor.image.material;
                bool raycastTarget = editor.image.raycastTarget;
                Vector4 raycastPadding = editor.image.raycastPadding;
                bool maskable = editor.image.maskable;
                Image.Type type = editor.image.type;
                bool useSpriteMesh = editor.image.useSpriteMesh;
                bool preserveAspect = editor.image.preserveAspect;
                bool fillCenter = editor.image.fillCenter;
                bool fillClockwise = editor.image.fillClockwise;
                float fillAmount = editor.image.fillAmount;
                Image.FillMethod fillMethod = editor.image.fillMethod;
                int fillOrigin = editor.image.fillOrigin;
                float pixelsPerUnitMultiplier = editor.image.pixelsPerUnitMultiplier;

                DestroyImmediate(editor.image);

                FixRaycastImage fixRaycastImage = editor.gameObject.AddComponent<FixRaycastImage>();    
                fixRaycastImage.sprite = sprite;
                fixRaycastImage.color = color;
                fixRaycastImage.material = material;
                fixRaycastImage.raycastTarget = raycastTarget;
                fixRaycastImage.raycastPadding = raycastPadding;
                fixRaycastImage.maskable = maskable;
                fixRaycastImage.type = type;
                fixRaycastImage.useGUILayout = useSpriteMesh;
                fixRaycastImage.preserveAspect = preserveAspect;
                fixRaycastImage.fillCenter = fillCenter;
                fixRaycastImage.fillClockwise = fillClockwise;
                fixRaycastImage.fillAmount = fillAmount;
                fixRaycastImage.fillMethod = fillMethod;
                fixRaycastImage.fillOrigin = fillOrigin;
                fixRaycastImage.pixelsPerUnitMultiplier = pixelsPerUnitMultiplier;
            }
        }
    }
}