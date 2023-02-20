using SDJK.Effect;
using SDJK.Ruleset.SuperHexagon.Renderer;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.Effect
{
    public class SuperHexagonYukiModeEffect : YukiModeEffect
    {
        [SerializeField] GLRenderInvoker _glRenderInvoker; public GLRenderInvoker glRenderInvoker => _glRenderInvoker;

        [SerializeField] Field _field; public Field field => _field;
        [SerializeField] RegularPolygonRenderer _regularPolygonRenderer; public RegularPolygonRenderer regularPolygonRenderer => _regularPolygonRenderer;
    }
}
