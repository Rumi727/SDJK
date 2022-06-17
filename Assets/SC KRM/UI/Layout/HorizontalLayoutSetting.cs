using UnityEngine;

namespace SCKRM.UI
{
    [AddComponentMenu("SC KRM/UI/Layout/Horizontal Layout Setting")]
    public sealed class HorizontalLayoutSetting : UI
    {
        [SerializeField] Mode _mode = Mode.none;
        public Mode mode => _mode;

        public enum Mode
        {
            none,
            center,
            right
        }
    }
}