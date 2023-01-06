using SCKRM.Renderer;
using SCKRM.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.MainMenu
{
    public sealed class MapPackListRulesetIcon : UIObjectPooling
    {
        [SerializeField] Graphic _iconGraphic; public Graphic iconGraphic => _iconGraphic;
        [SerializeField] CustomAllSpriteRenderer _icon; public CustomAllSpriteRenderer icon => _icon;
    }
}
