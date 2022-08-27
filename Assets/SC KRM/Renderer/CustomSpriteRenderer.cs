using K4.Threading;
using SCKRM.Threads;
using UnityEngine;

namespace SCKRM.Renderer
{
    [WikiDescription("스프라이트 렌더러")]
    [AddComponentMenu("SC KRM/Renderer/Sprite Renderer")]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class CustomSpriteRenderer : CustomAllSpriteRenderer
    {
        SpriteRenderer _spriteRenderer; public SpriteRenderer spriteRenderer => _spriteRenderer = this.GetComponentFieldSave(_spriteRenderer);



        [SerializeField] Vector2 _size = Vector2.zero;
        public Vector2 size { get => _size; set => _size = value; }

        [SerializeField] SpriteDrawMode _drawMode = SpriteDrawMode.Simple;
        public SpriteDrawMode drawMode { get => _drawMode; set => _drawMode = value; }

        public override void Refresh()
        {
            Sprite sprite = GetSprite(type, path, index, nameSpace);
            if (ThreadManager.isMainThread)
            {
                spriteRenderer.sprite = sprite;
                spriteRenderer.drawMode = drawMode;
                spriteRenderer.size = size;
            }
            else
            {
                K4UnityThreadDispatcher.Execute(() =>
                {
                    spriteRenderer.sprite = sprite;
                    spriteRenderer.drawMode = drawMode;
                    spriteRenderer.size = size;
                });
            }
        }
    }
}