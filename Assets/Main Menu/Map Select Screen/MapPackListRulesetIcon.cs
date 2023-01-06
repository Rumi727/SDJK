using SCKRM.Object;
using SCKRM.Renderer;
using UnityEngine;

namespace SDJK.MainMenu
{
    public sealed class MapPackListRulesetIcon : ObjectPooling
    {
        [SerializeField] CustomAllSpriteRenderer _icon; public CustomAllSpriteRenderer icon => _icon;
    }
}
