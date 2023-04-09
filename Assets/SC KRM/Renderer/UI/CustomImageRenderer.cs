using K4.Threading;
using SCKRM.Threads;
using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.Renderer
{
    [WikiDescription("이미지 렌더러")]
    [AddComponentMenu("SC KRM/Renderer/UI/Image")]
    [RequireComponent(typeof(Image))]
    public class CustomImageRenderer : CustomSpriteRendererBase
    {
        [SerializeField, HideInInspector] Image _image; public Image image => _image = this.GetComponentFieldSave(_image);

        [WikiDescription("새로고침")]
        public override void Refresh()
        {
            if (ThreadManager.isMainThread)
                image.sprite = GetSprite();
            else
                K4UnityThreadDispatcher.Execute(() => image.sprite = GetSprite());
        }
    }
}