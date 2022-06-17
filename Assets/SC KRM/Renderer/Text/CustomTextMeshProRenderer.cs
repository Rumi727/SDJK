using K4.Threading;
using SCKRM.Threads;
using TMPro;
using UnityEngine;

namespace SCKRM.Renderer
{
    [AddComponentMenu("SC KRM/Renderer/Text/Text Mesh Pro")]
    [RequireComponent(typeof(TMP_Text))]
    public sealed class CustomTextMeshProRenderer : CustomAllTextRenderer
    {
        [SerializeField, HideInInspector] TMP_Text _textMeshPro; public TMP_Text textMeshPro => _textMeshPro = this.GetComponentFieldSave(_textMeshPro);

        public override void Refresh()
        {
            string text = GetText();
            if (ThreadManager.isMainThread)
                textMeshPro.text = text;
            else
                K4UnityThreadDispatcher.Execute(() => textMeshPro.text = text);
        }
    }
}