using K4.Threading;
using SCKRM.Threads;
using UnityEngine;

namespace SCKRM.Renderer
{
    [AddComponentMenu("SC KRM/Renderer/Text/Text Mesh")]
    [RequireComponent(typeof(TextMesh))]
    public sealed class CustomTextMeshRenderer : CustomAllTextRenderer
    {
        [SerializeField, HideInInspector] TextMesh _textMesh; public TextMesh text => _textMesh = this.GetComponentFieldSave(_textMesh);

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