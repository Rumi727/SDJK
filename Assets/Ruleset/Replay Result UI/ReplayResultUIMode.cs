using SCKRM.Renderer;
using SCKRM.Tooltip;
using SCKRM.UI;
using UnityEngine;

namespace SDJK.Ruleset.ReplayResult
{
    public class ReplayResultUIMode : UIObjectPooling
    {
        [SerializeField] CustomSpriteRendererBase _customSpriteRendererBase; public CustomSpriteRendererBase customSpriteRendererBase => _customSpriteRendererBase;
        [SerializeField] Tooltip _tooltip; public Tooltip tooltip => _tooltip;
    }
}
