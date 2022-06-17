using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.UI
{
    [AddComponentMenu("SC KRM/UI/Dropdown/Dropdown Item")]
    public sealed class DropdownItem : UIObjectPooling
    {
        [SerializeField, NotNull] TMP_Text _label;
        public TMP_Text label { get => _label; }

        [SerializeField, NotNull] Toggle _toggle;
        public Toggle toggle { get => _toggle; }
    }
}