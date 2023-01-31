using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SCKRM.SaveLoad.UI
{
    [AddComponentMenu("SC KRM/Save Load/UI/Name Drag (Save file linkage)")]
    public sealed class SaveLoadUINameDrag : MonoBehaviour, IDragHandler
    {
        [SerializeField] UnityEvent _onDrag = new UnityEvent();
        public UnityEvent onDrag { get => _onDrag; }

        public void OnDrag(PointerEventData eventData) => onDrag.Invoke();
    }
}