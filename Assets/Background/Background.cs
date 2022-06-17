using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.Resource;
using SCKRM.UI;
using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK
{
    [RequireComponent(typeof(Image)), RequireComponent(typeof(CanvasGroup))]
    public sealed class Background : UIObjectPooling
    {
        public Image image => this.GetComponentFieldSave(_image); [SerializeField] Image _image;
        public CanvasGroup canvasGroup => this.GetComponentFieldSave(_canvasGroup); [SerializeField] CanvasGroup _canvasGroup;

        public override void OnCreate()
        {
            base.OnCreate();

            transform.SetSiblingIndex(0);

            SDJKMap map = MapManager.selectedMap;
            Texture2D texture = ResourceManager.GetTexture(PathTool.Combine(map.mapFilePathParent, map.info.backgroundFile));
            if (texture != null)
                image.sprite = ResourceManager.GetSprite(texture);
            else
                Remove();
        }

        public async UniTaskVoid PadeOut()
        {
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha = canvasGroup.alpha.MoveTowards(0, 0.05f * Kernel.fpsUnscaledDeltaTime);
                await UniTask.NextFrame();

                if (this == null)
                    return;
            }

            Remove();
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            canvasGroup.alpha = 1;

            if (image.sprite != null)
            {
                Destroy(image.sprite.texture);
                Destroy(image.sprite);
            }

            return true;
        }
    }
}
