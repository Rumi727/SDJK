using SCKRM.Renderer;
using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.UI
{
    [ExecuteAlways]
    public sealed class AlphaHitTestMinimumThreshold : UIBase
    {
        public Image image => _image = this.GetComponentFieldSave(_image, ComponentUtility.GetComponentMode.none); [SerializeField] Image _image;
        public SlicedFilledImage slicedFilledImage => _slicedFilledImage = this.GetComponentFieldSave(_slicedFilledImage, ComponentUtility.GetComponentMode.none); [SerializeField] SlicedFilledImage _slicedFilledImage;

        public float alphaHitTestMinimumThreshold
        {
            get => _alphaHitTestMinimumThreshold;
            set
            {
                _alphaHitTestMinimumThreshold = value;

                if (image != null)
                    image.alphaHitTestMinimumThreshold = value;
                else if (slicedFilledImage != null)
                    slicedFilledImage.alphaHitTestMinimumThreshold = value;
            }
        }
        [SerializeField, Range(0, 1)] float _alphaHitTestMinimumThreshold = 0.5f;

        protected override void Awake()
        {
            if (image != null)
                image.alphaHitTestMinimumThreshold = alphaHitTestMinimumThreshold;
            else if (slicedFilledImage != null)
                slicedFilledImage.alphaHitTestMinimumThreshold = alphaHitTestMinimumThreshold;

#if UNITY_2022_1_OR_NEWER
            if (Kernel.isPlaying && image.GetType() != typeof(FixRaycastImage) && alphaHitTestMinimumThreshold != 0)
            {
                CustomSpriteRendererBase renderer = GetComponent<CustomSpriteRendererBase>();
                if (renderer != null && !renderer.forceLocalSprite)
                    Debug.LogWarning("스프라이트 강제 로드가 켜져있지 않으면 아틀라스 기능으로 인해 알파 히트가 제대로 작동하지 않습니다");
            }
#endif
        }

        protected override void OnDestroy()
        {
            if (image != null)
                image.alphaHitTestMinimumThreshold = 0;
            else if (slicedFilledImage != null)
                slicedFilledImage.alphaHitTestMinimumThreshold = 0;
        }
    }
}
