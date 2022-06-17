using UnityEngine;

namespace SCKRM.UI
{
    [AddComponentMenu("SC KRM/UI/Layout/Vertical Layout Setting")]
    public sealed class VerticalLayoutSetting : UI
    {
        [SerializeField] Mode _mode = Mode.none;
        public Mode mode => _mode;

        public enum Mode
        {
            none,
            center,
            down
        }
    }
}