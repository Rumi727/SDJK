using SCKRM.Renderer;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SCKRM.Tooltip
{
    [WikiDescription("표시된 툴팁을 관리하기 위한 클래스 입니다")]
    [AddComponentMenu("SC KRM/Tooltip/Tooltip", 0)]
    public sealed class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public NameSpacePathPair nameSpacePathPair
        {
            get => new NameSpacePathPair(nameSpace, text);
            set
            {
                nameSpace = value.nameSpace;
                text = value.path;
            }
        }

        [SerializeField] string _nameSpace = ""; public string nameSpace { get => _nameSpace; set => _nameSpace = value; }
        [SerializeField] string _text = ""; public string text { get => _text; set => _text = value; }

        public void OnPointerEnter(PointerEventData eventData) => TooltipManager.Show(text, nameSpace);

        public void OnPointerExit(PointerEventData eventData) => TooltipManager.Hide();
    }
}
