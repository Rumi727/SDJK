using K4.Threading;
using SCKRM.Threads;
using UnityEngine;

namespace SCKRM.Renderer
{
    [WikiDescription("텍스트 렌더러")]
    [AddComponentMenu("SC KRM/Renderer/Text/Text")]
    [RequireComponent(typeof(UnityEngine.UI.Text))]
    public sealed class CustomTextRenderer : CustomTextRendererBase
    {
        [SerializeField, HideInInspector] UnityEngine.UI.Text _text; public UnityEngine.UI.Text text => _text = this.GetComponentFieldSave(_text);

        [WikiDescription("새로고침")]
        public override void Refresh()
        {
            if (ThreadManager.isMainThread)
                text.text = GetText();
            else
                K4UnityThreadDispatcher.Execute(() => text.text = GetText());
        }
    }
}