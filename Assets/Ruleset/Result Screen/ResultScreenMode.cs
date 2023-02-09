using SCKRM.Renderer;
using SCKRM.Tooltip;
using SCKRM.UI;
using UnityEngine;

namespace SDJlK.Ruleset.ResultScreen
{
    public class ResultScreenMode : UIObjectPooling
    {
        [SerializeField] CustomSpriteRendererBase _customSpriteRendererBase; public CustomSpriteRendererBase customSpriteRendererBase => _customSpriteRendererBase;
        [SerializeField] Tooltip _tooltip; public Tooltip tooltip => _tooltip;
    }
}
