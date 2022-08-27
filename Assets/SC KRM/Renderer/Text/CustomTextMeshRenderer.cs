using K4.Threading;
using SCKRM.Threads;
using UnityEngine;

namespace SCKRM.Renderer
{
    [WikiDescription("텍스트 메쉬 렌더러")]
    [AddComponentMenu("SC KRM/Renderer/Text/Text Mesh")]
    [RequireComponent(typeof(TextMesh))]
    public sealed class CustomTextMeshRenderer : CustomAllTextRenderer
    {
        [SerializeField, HideInInspector] TextMesh _textMesh; public TextMesh text => _textMesh = this.GetComponentFieldSave(_textMesh);

        [WikiDescription("새로고침")]
        public override void Refresh()
        {
            string text = GetText();
            if (ThreadManager.isMainThread)
                this.text.text = text;
            else
                K4UnityThreadDispatcher.Execute(() => this.text.text = text);
        }
    }
}