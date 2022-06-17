using K4.Threading;
using SCKRM.Threads;
using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.Renderer
{
    [AddComponentMenu("SC KRM/Renderer/UI/Image")]
    [RequireComponent(typeof(Image))]
    public class CustomImageRenderer : CustomAllSpriteRenderer
    {
        [SerializeField, HideInInspector] Image _image; public Image image => _image = this.GetComponentFieldSave(_image);

        public override async void Refresh()
        {
            Sprite sprite = SpriteReload(type, path, index, nameSpace);

            if (ThreadManager.isMainThread)
                image.sprite = sprite;
            else
                await K4UnityThreadDispatcher.Execute(() => image.sprite = sprite);
        }
    }
}