using SCKRM.Renderer;
using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.UI.Overlay.MessageBox
{
    [AddComponentMenu("SC KRM/Message Box/UI/Object Pooling/Message Box Button")]
    public sealed class MessageBoxButton : UIObjectPooling
    {
        [SerializeField] Button _button; public Button button => _button;
        [SerializeField] CustomAllTextRenderer _text; public CustomAllTextRenderer text => _text;

        public int index { get; set; }
    }
}
