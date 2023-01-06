using SCKRM.Renderer;
using SCKRM.UI;
using UnityEngine;

namespace SDJK.MainMenu
{
    public sealed class MapPackListRulesetIcon : UIObjectPooling
    {
        [SerializeField] CustomAllSpriteRenderer _icon; public CustomAllSpriteRenderer icon => _icon;
    }
}
