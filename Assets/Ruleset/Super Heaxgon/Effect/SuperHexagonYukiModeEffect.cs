using SCKRM;
using SDJK.Effect;
using SDJK.Ruleset.SuperHexagon.Renderer;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.Effect
{
    public class SuperHexagonYukiModeEffect : YukiModeEffect
    {
        [SerializeField, NotNull] GLRenderInvoker _glRenderInvoker; public GLRenderInvoker glRenderInvoker => _glRenderInvoker;

        [SerializeField, NotNull] Field _field; public Field field => _field;
        [SerializeField, NotNull] RegularPolygonRenderer _regularPolygonRenderer; public RegularPolygonRenderer regularPolygonRenderer => _regularPolygonRenderer;
    }
}
