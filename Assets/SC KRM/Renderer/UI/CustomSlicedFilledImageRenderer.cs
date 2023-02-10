using K4.Threading;
using SCKRM.Threads;
using SCKRM.UI;
using UnityEngine;

namespace SCKRM.Renderer
{
    [WikiDescription("채워지며 슬라이스 된 이미지 렌더러")]
    [AddComponentMenu("SC KRM/Renderer/UI/Sliced Filled Image")]
    [RequireComponent(typeof(SlicedFilledImage))]
    public class CustomSlicedFilledImageRenderer : CustomSpriteRendererBase
    {
        [SerializeField, HideInInspector] SlicedFilledImage _image; public SlicedFilledImage image => _image = this.GetComponentFieldSave(_image);

        [WikiDescription("새로고침")]
        public override void Refresh()
        {
            Sprite sprite = GetSprite(type, path, index, nameSpace);

            if (ThreadManager.isMainThread)
                image.sprite = sprite;
            else
                K4UnityThreadDispatcher.Execute(() => image.sprite = sprite);
        }
    }
}