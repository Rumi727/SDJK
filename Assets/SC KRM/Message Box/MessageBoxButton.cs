using SCKRM.Renderer;
using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.UI.Overlay.MessageBox
{
    [WikiDescription("메시지 박스의 버튼을 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/Message Box/UI/Object Pooling/Message Box Button")]
    public sealed class MessageBoxButton : UIObjectPooling
    {
        [SerializeField] Button _button; public Button button => _button;
        [SerializeField] CustomTextRendererBase _text; public CustomTextRendererBase text => _text;

        [WikiDescription("버튼의 인덱스")]
        public int index { get; set; }
    }
}
