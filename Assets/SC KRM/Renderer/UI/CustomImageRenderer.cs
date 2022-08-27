using K4.Threading;
using SCKRM.Threads;
using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.Renderer
{
    [WikiDescription("이미지 렌더러")]
    [AddComponentMenu("SC KRM/Renderer/UI/Image")]
    [RequireComponent(typeof(Image))]
    public class CustomImageRenderer : CustomAllSpriteRenderer
    {
        [SerializeField, HideInInspector] Image _image; public Image image => _image = this.GetComponentFieldSave(_image);

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